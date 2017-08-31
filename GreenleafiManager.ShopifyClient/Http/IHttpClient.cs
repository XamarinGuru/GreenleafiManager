using System.Collections.Specialized;
using System.Threading.Tasks;

namespace ShopifyClient.Http {
    internal interface IHttpClient {
        Task<object> GetAsync ( string path, NameValueCollection parameters = null );
        Task<object> PostAsync ( string path, object data );
        Task<object> PutAsync ( string path, object data );
        Task<object> DeleteAsync ( string path );
    }

    internal interface IHttpClientGeneric {
        Task<T> GetAsync<T> ( string path, NameValueCollection parameters = null );
        Task<T> PostAsync<T> ( string path, T data );
        Task<T> PutAsync<T> ( string path, T data );
        Task<T> DeleteAsync<T> ( string path );
    }
}