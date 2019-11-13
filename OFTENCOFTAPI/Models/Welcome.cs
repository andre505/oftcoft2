using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.Models
{
    public class Welcome
    {
        public static Dictionary<string, string>[] FromJson(string json) => JsonConvert.DeserializeObject<Dictionary<string, string>[]>(json, OFTENCOFTAPI.Models.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Dictionary<string, string>[] self) => JsonConvert.SerializeObject(self, OFTENCOFTAPI.Models.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            //MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
