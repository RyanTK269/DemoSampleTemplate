using System;
using System.Collections.Generic;
using System.Text;

namespace DemoSampleTemplate.Core.DataObjects.Config
{
    public class SFTPConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
