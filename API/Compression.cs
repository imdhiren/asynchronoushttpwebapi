using System.IO;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace API
{
    public class CompressResponse : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionContext)
        {
            bool isCompressionSupported = CompressionHelper.IsCompressionSupported();

            string acceptEncoding = HttpContext.Current.Request.Headers["Accept-Encoding"];

            if (isCompressionSupported)
            {
                var content = actionContext.Response.Content;

                var byteArray = content == null ? null : content.ReadAsByteArrayAsync().Result;

                MemoryStream memoryStream = new MemoryStream(byteArray);

                if (acceptEncoding.Contains("gzip"))
                {
                    actionContext.Response.Content = new ByteArrayContent(CompressionHelper.Compress(memoryStream.ToArray(), false));
                    actionContext.Response.Content.Headers.Remove("Content-Type");
                    actionContext.Response.Content.Headers.Add("Content-encoding", "gzip");
                    actionContext.Response.Content.Headers.Add("Content-Type", "application/json");
                }
                else
                {
                    actionContext.Response.Content = new ByteArrayContent(CompressionHelper.Compress(memoryStream.ToArray()));
                    actionContext.Response.Content.Headers.Remove("Content-Type");
                    actionContext.Response.Content.Headers.Add("Content-encoding", "deflate");
                    actionContext.Response.Content.Headers.Add("Content-Type", "application/json");
                }
            }

            base.OnActionExecuted(actionContext);

        }
    }

    public class CompressionHelper
    {
        public static byte[] Compress(byte[] data, bool useGZipCompression = true)
        {
            System.IO.Compression.CompressionLevel compressionLevel = System.IO.Compression.CompressionLevel.Fastest;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                if (useGZipCompression)
                {
                    using (System.IO.Compression.GZipStream gZipStream = new System.IO.Compression.GZipStream(memoryStream, compressionLevel, true))
                    {
                        gZipStream.Write(data, 0, data.Length);
                    }
                }
                else
                {
                    using (System.IO.Compression.GZipStream gZipStream = new System.IO.Compression.GZipStream(memoryStream, compressionLevel, true))
                    {
                        gZipStream.Write(data, 0, data.Length);
                    }
                }
                return memoryStream.ToArray();
            }
        }

        public static bool IsCompressionSupported()
        {
            string AcceptEncoding = HttpContext.Current.Request.Headers["Accept-Encoding"];

            return ((!string.IsNullOrEmpty(AcceptEncoding) && (AcceptEncoding.Contains("gzip") || AcceptEncoding.Contains("deflate"))));
        }
    }
}