using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ShopifyClient.DataTranslators {
    /// <summary>
    ///     This class is used to translate to and from C# object and JSON strings
    /// </summary>
    public class JsonDataTranslator : IDataTranslator {
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore
        };

        /// <summary>
        ///     The content type used by JSON
        /// </summary>
        public string ContentType => "application/json";

        /// <summary>
        ///     Given a C# object, return a JSON string that can be used by the Shopify API
        /// </summary>
        /// <param name="data">A C# object that maps to a JSON object</param>
        /// <returns>JSON string</returns>
        public string Encode ( object data ) {
            return JsonConvert.SerializeObject( data, Formatting.None, _serializerSettings );
        }

        /// <summary>
        ///     Given a JSON String, return a corresponding C# object
        /// </summary>
        /// <param name="encodedData">JSON string return from the Shopify API</param>
        /// <returns>C# object corresponding to the JSON data return from the Shopify API</returns>
        public object Decode ( string encodedData ) {
            return JObject.Parse( encodedData );
        }

        /// <summary>
        ///     Given a JSON String, return a corresponding C# object.
        /// </summary>
        /// <param name="encodedData">JSON string return from the Shopify API</param>
        /// <returns>C# object corresponding to the JSON data return from the Shopify API</returns>
        public T Decode<T> ( string encodedData ) {
            return JsonConvert.DeserializeObject<T>( encodedData );
        }
    }
}