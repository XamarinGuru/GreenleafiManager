namespace ShopifyClient.DataTranslators {
    /// <summary>
    ///     An instance of this interface would translate the data between c# and the Shopify API.
    /// </summary>
    public interface IDataTranslator {
        /// <summary>
        ///     The Content Type (Mime Type) used by this translator.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        ///     Encode the data in a way that is expected by the Shopify API.
        /// </summary>
        /// <param name="data">Data that should be encoded for the Shopify API.</param>
        string Encode ( object data );

        /// <summary>
        ///     Decode the data returned by the Shopify API.
        /// </summary>
        /// <param name="encodedData">Data encoded by the Shopify API.</param>
        object Decode ( string encodedData );

        /// <summary>
        ///     Decode the data returned by the Shopify API.
        /// </summary>
        /// <typeparam name="T">Type of the decoded object.</typeparam>
        /// <param name="encodedData">Data encoded by the Shopify API.</param>
        T Decode<T> ( string encodedData );
    }
}