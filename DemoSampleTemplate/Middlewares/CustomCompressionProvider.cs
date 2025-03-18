using Microsoft.AspNetCore.ResponseCompression;

namespace DemoSampleTemplate.Middlewares
{
    public class CustomCompressionProvider : ICompressionProvider
    {
        public string EncodingName => "mycustomcompression";
        public bool SupportsFlush => true;

        public Stream CreateStream(Stream stream)
        {
            // Write your code here to compress the data
            return stream;
        }
    }
}
