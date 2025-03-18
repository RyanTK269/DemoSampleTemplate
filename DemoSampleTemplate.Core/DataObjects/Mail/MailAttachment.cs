using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DemoSampleTemplate.Core.DataObjects.Mail
{
    public class MailAttachment
    {
        public byte[] Bytes { get; set; }
        public string FileName { get; set; }

        public string Base64Content
        {
            get
            {
                string str = string.Empty;
                if (Bytes != null && Bytes.Length != 0)
                {
                    str = Convert.ToBase64String(Bytes);
                }
                return str;
            }
        }

        public MailAttachment()
        {
        }

        public MailAttachment(string fileName, MemoryStream ms)
        {
            FileName = fileName;
            Bytes = ms.ToArray();
        }

        public static List<MailAttachment> GetListEmailAttachment(List<string> filesNames)
        {
            List<MailAttachment> listEmailAttachment = null;
            if (filesNames != null && filesNames.Any())
            {
                listEmailAttachment = new List<MailAttachment>();
                foreach (string fileName in filesNames)
                {
                    MemoryStream ms = new MemoryStream();
                    using (FileStream fs = System.IO.File.OpenRead(fileName))
                    {
                        fs.CopyTo(ms);
                        fs.Close();
                    }
                    listEmailAttachment.Add(new MailAttachment(new FileInfo(fileName).Name, ms));
                }
            }
            return listEmailAttachment;
        }
    }
}
