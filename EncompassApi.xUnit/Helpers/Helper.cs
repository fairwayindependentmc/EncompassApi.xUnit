using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace EncompassApi.xUnit
{
    public static class Helper
    {
        public static JObject[] GetArray(string fileName)
        {
            var fullPath = $"Payloads/{fileName}.json";

            // Get the absolute path to the JSON file
            var path = Path.IsPathRooted(fullPath)
                ? fullPath
                : Directory.GetCurrentDirectory() + "/" + fullPath;
            Assert.True(File.Exists(path), $"FIle {fileName}.json doesn't exist!");
            var fileData = File.ReadAllText(fullPath);
            return JsonConvert.DeserializeObject<JObject[]>(fileData);
        }

        public static JObject[] GetLoanDocuments() => GetArray("LoanDocuments");
        public static JObject[] GetLoanAttachments() => GetArray("LoanAttachments");

        public static JObject Get(string fileName)
        {
            var fullPath = $"Payloads/LoanDocuments.json";

            // Get the absolute path to the JSON file
            var path = Path.IsPathRooted(fullPath)
                ? fullPath
                : Directory.GetCurrentDirectory() + "/" + fullPath;
            Assert.True(File.Exists(path), $"FIle {fileName}.json doesn't exist!");
            var fileData = File.ReadAllText(fullPath);
            var objs = JsonConvert.DeserializeObject<JObject[]>(fileData);
            return objs[0];
        }

        public static JObject GetLoanDocument() => Get("LoanDocuments");
        public static JObject GetMediaUrlObject() => Get("MediaUrlObject");
    }
}
