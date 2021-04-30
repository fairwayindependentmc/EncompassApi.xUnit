using FluentAssertions.Primitives;
using Newtonsoft.Json.Linq;

namespace EncompassApi.xUnit.Extensions
{
    public static class JObjectAssertionsExtensions
    {
        public static ObjectAssertions Should<TObject>(this JObject jobject)
        {
            var jsn = jobject.ToString();
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TObject>(jsn);
            return new ObjectAssertions(obj);
        }
    }
}
