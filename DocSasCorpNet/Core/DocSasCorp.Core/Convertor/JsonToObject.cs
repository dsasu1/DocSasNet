using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocSasCorp.Core.Convertor
{
    /// <summary>
    /// JsonToObject
    /// </summary>
    public class JsonToObject
    {
        /// <summary>
        /// ToJson
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToJson(object value)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter());
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// FromJson
        /// </summary>
        /// <typeparam name="Tresult"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Tresult FromJson<Tresult>(string value)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter());
            return JsonConvert.DeserializeObject<Tresult>(value);
        }
    }
}
