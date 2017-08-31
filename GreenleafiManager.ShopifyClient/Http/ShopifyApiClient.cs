using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ShopifyClient.DataTranslators;
using ShopifyClient.Http.Extensions;

namespace ShopifyClient.Http {
    /// <summary>
    ///     This class is used to make Shopify API calls
    /// </summary>
    /// <remarks>
    ///     You will first need to use the ShopifyAPIAuthorizer to obtain the required authorization.
    /// </remarks>
    /// <seealso cref="http://api.shopify.com/" />
    public class ShopifyApiClient : IHttpClient, IHttpClientGeneric {
        private static readonly string DefaultContentType = "application/json";
        private readonly ShopifyAuthorizationState _authorizationState;
        private readonly IDataTranslator _dataTranslator;

        /// <summary>
        ///     Creates an instance of this class for use with making API Calls
        /// </summary>
        /// <param name="authorizationState">the authorization state required to make the API Calls</param>
        public ShopifyApiClient ( ShopifyAuthorizationState authorizationState ) {
            _authorizationState = authorizationState;
        }

        /// <summary>
        ///     Creates an instance of this class for use with making API Calls
        /// </summary>
        /// <param name="authorizationState">the authorization state required to make the API Calls</param>
        /// <param name="dataTranslator">the translator used to transform the data between your C# Client code and the Shopify API</param>
        public ShopifyApiClient ( ShopifyAuthorizationState authorizationState, IDataTranslator dataTranslator ) {
            _authorizationState = authorizationState;
            _dataTranslator = dataTranslator;
        }

        private string BaseUrl => $"https://{_authorizationState.ShopName}.myshopify.com";

        /// <summary>
        ///     Make a Get method HTTP request to the Shopify API
        /// </summary>
        /// <param name="path">the path where the API call will be made.</param>
        /// <param name="parameters">the query string parameters</param>
        /// <seealso cref="http://api.shopify.com/" />
        /// <returns>the server response</returns>
        public async Task<object> GetAsync ( string path, NameValueCollection parameters = null ) {
            var request = GetHttpGetRequest( path, parameters );
            return await MakeRequest( request );
        }

        /// <summary>
        ///     Make a Post method HTTP request to the Shopify API
        /// </summary>
        /// <param name="path">the path where the API call will be made.</param>
        /// <param name="data">the data that this path will be expecting</param>
        /// <seealso cref="http://api.shopify.com/" />
        /// <returns>the server response</returns>
        public async Task<object> PostAsync ( string path, object data ) {
            var request = await GetHttpPostRequest( path, data );
            return await MakeRequest( request );
        }

        /// <summary>
        ///     Make a Put method HTTP request to the Shopify API
        /// </summary>
        /// <param name="path">the path where the API call will be made.</param>
        /// <param name="data">the data that this path will be expecting</param>
        /// <seealso cref="http://api.shopify.com/" />
        /// <returns>the server response</returns>
        public async Task<object> PutAsync ( string path, object data ) {
            var request = await GetHttpPutRequest( path, data );
            return await MakeRequest( request );
        }

        /// <summary>
        ///     Make a Delete method HTTP request to the Shopify API
        /// </summary>
        /// <param name="path">the path where the API call will be made.</param>
        /// <seealso cref="http://api.shopify.com/" />
        /// <returns>the server response</returns>
        public async Task<object> DeleteAsync ( string path ) {
            var request = GetHttpDeleteRequest( path );
            return await MakeRequest( request );
        }

        public async Task<T> GetAsync<T> ( string path, NameValueCollection parameters = null ) {
            var request = GetHttpGetRequest( path, parameters );
            return await MakeRequest<T>( request );
        }

        public async Task<T> PostAsync<T> ( string path, T data ) {
            var request = await GetHttpPostRequest( path, data );
            return await MakeRequest<T>( request );
        }

        public async Task<T> PutAsync<T> ( string path, T data ) {
            var request = await GetHttpPutRequest( path, data );
            return await MakeRequest<T>( request );
        }

        public async Task<T> DeleteAsync<T> ( string path ) {
            var request = GetHttpDeleteRequest( path );
            return await MakeRequest<T>( request );
        }

        private async Task<object> MakeRequest ( HttpWebRequest webRequest ) {
            var response = (HttpWebResponse) await webRequest.GetResponseAsync();
            string result;
            using ( var responseStream = response.GetResponseStream() ) {
                if ( responseStream == null ) {
                    return null;
                }
                using ( var streamReader = new StreamReader( responseStream ) ) {
                    result = await streamReader.ReadToEndAsync();
                    streamReader.Close();
                }
            }
            if ( string.IsNullOrWhiteSpace( result ) ) {
                return null;
            }
            if ( _dataTranslator != null ) {
                return _dataTranslator.Decode( result );
            }
            return result;
        }

        private async Task<T> MakeRequest<T> ( HttpWebRequest webRequest ) {
            if ( _dataTranslator == null ) {
                throw new InvalidCastException( "Could not cast HTTP response to specified type. Missing data translator." );
            }

            var response = (HttpWebResponse) await webRequest.GetResponseAsync();
            string result;
            using ( var responseStream = response.GetResponseStream() ) {
                if ( responseStream == null ) {
                    return default(T);
                }
                using ( var streamReader = new StreamReader( responseStream ) ) {
                    result = await streamReader.ReadToEndAsync();
                    streamReader.Close();
                }
            }
            if ( string.IsNullOrWhiteSpace( result ) ) {
                return default(T);
            }

            return _dataTranslator.Decode<T>( result );
        }

        #region CommonHttpRequests

        private HttpWebRequest GetHttpGetRequest ( string path, NameValueCollection parameters = null ) {
            var url = BaseUrl + path;
            if ( parameters != null ) {
                url += $"?{parameters.ToQueryString()}";
            }
            var request = GetBaseRequest( url );
            request.Method = HttpMethods.Get;
            return request;
        }

        private async Task<HttpWebRequest> GetHttpPostRequest ( string path, object data ) {
            var request = GetBaseRequest( BaseUrl + path );
            request.Method = HttpMethods.Post;
            await AttachRequestBody( request, data );
            return request;
        }

        private async Task<HttpWebRequest> GetHttpPutRequest ( string path, object data ) {
            var request = GetBaseRequest( BaseUrl + path );
            request.Method = HttpMethods.Put;
            await AttachRequestBody( request, data );
            return request;
        }

        private HttpWebRequest GetHttpDeleteRequest ( string path ) {
            var request = GetBaseRequest( BaseUrl + path );
            request.Method = HttpMethods.Delete;
            return request;
        }

        private HttpWebRequest GetBaseRequest ( string url ) {
            var request = (HttpWebRequest) WebRequest.Create( url );
            request.Headers.Add( "X-Shopify-Access-Token", _authorizationState.AccessToken );
            request.ContentType = _dataTranslator?.ContentType ?? DefaultContentType;
            return request;
        }

        private async Task AttachRequestBody ( HttpWebRequest request, object data ) {
            var requestBody = _dataTranslator == null
                ? data.ToString()
                : _dataTranslator.Encode( data );

            if ( !string.IsNullOrEmpty( requestBody ) ) {
                using ( var streamWriter = new StreamWriter( request.GetRequestStream() ) ) {
                    await streamWriter.WriteAsync( requestBody );
                    streamWriter.Close();
                }
            }
        }

        #endregion
    }
}