namespace ShopifyClient {
    public class ShopifyAuthorizationState {
        public ShopifyAuthorizationState ( string shopName, string accessToken ) {
            ShopName = shopName;
            AccessToken = accessToken;
        }

        public string ShopName { get; }
        public string AccessToken { get; }
    }
}