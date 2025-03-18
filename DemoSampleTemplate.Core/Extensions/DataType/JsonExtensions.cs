using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace DemoSampleTemplate.Core.Extensions.DataType
{
    public static class JsonExtensions
    {
        public static string JsonSerialize<T>(this T dataToSerialize)
        {
            return JsonConvert.SerializeObject(dataToSerialize);
        }

        public static T JsonDeserialise<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(obj.GetType());
                obj = (T)serializer.ReadObject(ms);
            }
            return obj;
        }
    }
}
