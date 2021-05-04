using EncompassApi.xUnit.Services;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace EncompassApi.xUnit.Sample
{
    public class SampleTest
    {
        private readonly ITestOutputHelper _outputWriter;
        private readonly IMockedEncompassHttpClientService _mockedEncompassClient;

        public SampleTest(
            ITestOutputHelper outputWriter,
            IMockedEncompassHttpClientService mockedEncompassHttpClient)
        {
            outputWriter.WriteLine("### SampleTest initiating! ###");
            _outputWriter = outputWriter;
            _mockedEncompassClient = mockedEncompassHttpClient;
        }

        [Fact]
        public void TestMe()
        {
            var mockedLoanId = Guid.NewGuid().ToString();
            var mockedDocumentId = Guid.NewGuid().ToString();
            var mockedLoanDocument = Payloads.Helper.GetLoanDocument();
            // Mock client and 
            var source = _mockedEncompassClient
                //.SetClientApiResponse((sender, args) =>
                //{
                //    args.Should().NotBeNull(because: "Response is null!");
                //})
                .SetupResponseMessage(resp =>
                {
                    resp.StatusCode = System.Net.HttpStatusCode.OK;
                    resp.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(mockedLoanDocument));
                }, testHeader: new KeyValuePair<string, string>("TestHeader", Faker.Lorem.GetFirstWord()))
                .SetDocumentsApiResponseCallback(mockedLoanId, (sender, args) =>
                {
                    args.Should().NotBeNull(because: "Response is null!");
                })
                .GetDocumentAsync(mockedDocumentId).ConfigureAwait(false);

            source.Should()
                .NotBeNull();
                
        }
    }
}
