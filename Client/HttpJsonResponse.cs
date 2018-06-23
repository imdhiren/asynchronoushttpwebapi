using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace APICore
{
    public class HttpJsonResponse<T> : HttpResponse
    {
        public HttpJsonResponse(HttpResponseMessage msg) : base(msg)
        {
        }

        public T BodyDeserialized => DeserializeObject(this, this.Body);

        public static T DeserializeObject(HttpResponse request, string body)
        {
            string AcceptEncoding = request.Headers.Contains("Accept-Encoding") ? request.Headers.GetValues("Accept-Encoding").FirstOrDefault() : string.Empty;

            if ((!string.IsNullOrEmpty(AcceptEncoding) && (AcceptEncoding.Contains("gzip") || AcceptEncoding.Contains("deflate"))))
            {
                return JsonExtensions.DeserializeCompressed<T>(new MemoryStream(Encoding.UTF8.GetBytes(body ?? "")));
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(body);
            }
        }
    }

    public static class JsonExtensions
    {

        public static void SerializeCompressed(object value, Stream stream, JsonSerializerSettings settings = null)
        {
            using (var compressor = new GZipStream(stream, CompressionMode.Compress))
            using (var writer = new StreamWriter(compressor))
            {
                var serializer = JsonSerializer.CreateDefault(settings);
                serializer.Serialize(writer, value);
            }
        }

        public static T DeserializeFromFileCompressed<T>(string path, JsonSerializerSettings settings = null)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                return DeserializeCompressed<T>(fs, settings);
        }

        public static T DeserializeCompressed<T>(Stream stream, JsonSerializerSettings settings = null)
        {
            using (var compressor = new GZipStream(stream, CompressionMode.Decompress))
            using (var reader = new StreamReader(compressor))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var serializer = JsonSerializer.CreateDefault(settings);
                return serializer.Deserialize<T>(jsonReader);
            }
        }
    }
}