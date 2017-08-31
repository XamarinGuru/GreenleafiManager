using System.Collections.Specialized;
using System.Linq;

namespace ShopifyClient.Http.Extensions {
    public static class NameValueCollectionExtensions {
        public static string ToQueryString ( this NameValueCollection nameValueCollection ) {
            return string.Join( "&", nameValueCollection.AllKeys.Select( key => $"{key}={nameValueCollection[key]}" ) );
        }
    }
}