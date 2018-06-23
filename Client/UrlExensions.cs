using System;
using System.Collections;
using System.Linq;

namespace APICore
{
    internal static class UrlExtensions
    {
            public static string ToQueryString<T>(this T request)
            {
                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                // Get all properties on the object
                var properties = typeof(T).GetProperties()
                    .Where(x => x.CanRead)
                    .ToDictionary(x => x.Name, x => x.GetValue(request));

                // Get names for all IEnumerable properties (excl. string)
                var collectionProperties = properties
                    .Where(x => !(x.Value is string) && x.Value is IEnumerable)
                    .ToList();

                // Concat all IEnumerable properties into a comma separated string
                foreach (var key in collectionProperties)
                {
                    var enumerable = (IEnumerable)key.Value;

                    properties[key.Key] = string.Join(",", enumerable);
                }

                // Concat all key/value pairs into a string separated by ampersand
                return string.Join("&", properties
                    .Select(x => string.Concat(
                        Uri.EscapeDataString(x.Key), "=",
                        Uri.EscapeDataString(x.Value?.ToString() ?? ""))));
            }
        }
}
