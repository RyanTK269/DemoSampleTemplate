using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DemoSampleTemplate.Core.DataObjects.File
{
    public class Document
    {
        public string Code { get; set; }
        public string Filename { get; set; }
        public DateTime Date { get; set; }
        public string Size { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public byte[] Content { get; set; }

        public Document()
        {

        }

        public Document(Stream stream, string fileName)
        {
            Filename = fileName;
            Date = DateTime.Now;
            Name = fileName;
            Code = "NEWS";
            using (Stream inputStream = stream)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    using (memoryStream = new MemoryStream())
                    {
                        inputStream.CopyTo(memoryStream);
                    }
                }
                Content = memoryStream.ToArray();
            }
        }
    }
}
