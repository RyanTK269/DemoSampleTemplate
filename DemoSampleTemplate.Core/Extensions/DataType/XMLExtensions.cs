using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace DemoSampleTemplate.Core.Extensions.DataType
{
    public static class XMLExtensions
    {
        public static string XMLSerialize<T>(this T dataToSerialize)
        {
            var stringwriter = new StringWriter();
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stringwriter, dataToSerialize);
            return stringwriter.ToString();
        }

        public static T XMLDeserialize<T>(this string xmlText)
        {
            var stringReader = new StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stringReader);
        }
    }
}
