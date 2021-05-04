using EncompassApi.EFolder;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EncompassApi.xUnit.Converter
{
    public static  class JsonMockConverter
    {
        public static string ToJson<TObject>(this Mock<TObject> mock) where TObject : class
        {
            var objectProperties = typeof(TObject).GetProperties();
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (var prop in objectProperties)
            {
                sb.Append($"\"{prop.Name}\":\"{mock.Object.GetType().GetProperty(prop.Name).GetValue(mock.Object)}\",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            return sb.ToString();
        }
    }
}
