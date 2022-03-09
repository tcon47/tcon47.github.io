using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    public static class Utilities
    {
        public static async Task<IEnumerable<TEntity>> GetRemoteDataAsync<TEntity>(bool pluralizeModel = true)
        {
            string modelName = typeof(TEntity).Name;

            if (pluralizeModel == true)
            {
                modelName = $"{modelName}" + "s";
            }

            // new http handler
            using (HttpClientHandler httpClientHandler = new HttpClientHandler())
            {
                // set compression on handler
                httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                // pass handler as paramater in http client
                using (HttpClient httpClient = new HttpClient(httpClientHandler))
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Prefer", @"odata.include-annotations=""*""");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json;odata.metadata=minimal");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "UTF-8");

                    // attempt response from http client
                    using (HttpResponseMessage responseMessage = await httpClient.GetAsync(new Uri($"http://localhost:18316/odata/" + $"{modelName}")))
                    {
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            string jsonString = await responseMessage.Content.ReadAsStringAsync();

                            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings() { DateParseHandling = DateParseHandling.DateTimeOffset };

                            IEnumerable<TEntity> result = Newtonsoft.Json.JsonConvert.DeserializeObject<ODataResponse<IEnumerable<TEntity>>>(jsonString, jsonSerializerSettings).Value;

                            return result;
                        }
                        else if (responseMessage.StatusCode == HttpStatusCode.NotFound || responseMessage.StatusCode == HttpStatusCode.NoContent)
                        {
                            return null;
                        }
                    }
                }
            }

            return null;
        }
    }

    public class ODataResponse<T>
    {
        [Newtonsoft.Json.JsonProperty("@odata.context")]
        public string ODataContext { get; set; }

        [Newtonsoft.Json.JsonProperty("@odata.count")]
        public int? Count { get; set; }


        public T Value { get; set; }

    }
}


