using EncompassApi.Configuration;
using EncompassApi.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Contrib.HttpClient;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Faker;

namespace EncompassApi.xUnit.Services
{
    public interface IMockedEncompassHttpClientService
    {
        Uri BaseAddress { get; }
        IHttpClientFactory HttpFactory { get; }
        HttpClient MockedClient { get; }
        EncompassApiService MockedEncompassClient { get; }
        Mock<HttpMessageHandler> MockedHandler { get; }

        IMockedEncompassHttpClientService SetOptions(Action<Mock<IHttpClientOptions>> options);
        IMockedEncompassHttpClientService SetOptions(Mock<IHttpClientOptions> options);
        IMockedEncompassHttpClientService AddDefaultRequestHeaders();
        IMockedEncompassHttpClientService SetupResponseMessage(Action<HttpResponseMessage> action, Action callBack, KeyValuePair<string, string>? testHeader = null);
        IMockedEncompassHttpClientService SetupResponseMessage(Action<HttpResponseMessage> action, KeyValuePair<string, string>? testHeader = null);
        MockedEncompassHttpClientService SetClientApiResponse(EventHandler<ApiResponseEventArgs> callBack);
        Webhook.Webhook SetWebhookApiResponseCallback(EventHandler<ApiResponseEventArgs> action);
        EncompassApi.Loans.Documents.LoanDocuments SetDocumentsApiResponseCallback(string mockedLoanId, EventHandler<ApiResponseEventArgs> action);
    }
    public class MockedEncompassHttpClientService : IMockedEncompassHttpClientService
    {
        private readonly ILogger<MockedEncompassHttpClientService> _logger;

        public Mock<HttpMessageHandler> MockedHandler { get; private set; }
        public IHttpClientFactory HttpFactory { get; private set; }
        public HttpClient MockedClient { get; private set; }
        public EncompassApiService MockedEncompassClient { get; private set; }

        public Uri BaseAddress { get; private set; }
        public Mock<IHttpClientOptions> Options { get; private set; }

        public IMockedEncompassHttpClientService SetOptions(Action<Mock<IHttpClientOptions>> options)
        {
            Options = new Mock<IHttpClientOptions>();
            Options.SetupProperty(m => m.CompressionOptions);
            options(Options);
            return this;
        }

        public IMockedEncompassHttpClientService SetOptions(Mock<IHttpClientOptions> options)
        {
            Options = options;
            return this;
        }

        public IMockedEncompassHttpClientService AddDefaultRequestHeaders()
        {
            MockedClient.AddDefaultRequestHeaders(Options.Object);
            return this;
        }

        public MockedEncompassHttpClientService(ILogger<MockedEncompassHttpClientService> logger)
        {
            _logger = logger;
            MockedHandler = new Mock<HttpMessageHandler>();
            HttpFactory = MockedHandler.CreateClientFactory();
            MockedClient = HttpFactory.CreateClient("EncompassClient");
            BaseAddress = new Uri("https://www.example.com/api"); 
            MockedClient.BaseAddress = BaseAddress;
            MockedEncompassClient = new EncompassApiService(MockedClient, new ClientParameters());
        }

        public IMockedEncompassHttpClientService SetupResponseMessage(Action<HttpResponseMessage> action, Action callBack, KeyValuePair<string, string>? testHeader = null)
        {
            _logger?.LogInformation("Setup a response message for testing.");
            var respMsg = new HttpResponseMessage();
            action(respMsg);

            if (testHeader.HasValue)
                respMsg.Headers.Add(testHeader.Value.Key, testHeader.Value.Value);

            var returnsResult = MockedHandler.Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(respMsg);
            if(callBack != null)
                returnsResult.Callback(callBack);
            return this;
        }

        public IMockedEncompassHttpClientService SetupResponseMessage(Action<HttpResponseMessage> action, KeyValuePair<string, string>? testHeader = null)
            => SetupResponseMessage(action, null, testHeader);

        public MockedEncompassHttpClientService SetClientApiResponse(EventHandler<ApiResponseEventArgs> callBack)
        {
            if (callBack != null)
                MockedEncompassClient.ApiResponse += callBack;
            return this;
        }
        public Webhook.Webhook SetWebhookApiResponseCallback(EventHandler<ApiResponseEventArgs> action)
        {
            _logger?.LogInformation("Attaching a Response callback and returning Webhook object.");

            MockedEncompassClient.Webhook.ApiResponseEventHandler += action;
            return MockedEncompassClient.Webhook;
        }

        public Loans.Documents.LoanDocuments SetDocumentsApiResponseCallback(string mockedLoanId, EventHandler<ApiResponseEventArgs> action)
        {
            _logger?.LogInformation("Attaching a Response callback and returning LoanDocuments object.");

            var docs = MockedEncompassClient.Loans.GetLoanApis(mockedLoanId).Documents;
            docs.ApiResponseEventHandler += action;
            return docs;

        }

    }
}
