using Microsoft.Azure.Mobile.Server;

namespace GreenLeafMobileService.DataObjects
{
    public class ItemCode : EntityData
    {
        public string Code { get; set; }
        public string Value { get; set; }
        public string Category { get; set; }
        
        public string ShopifyTags { get; set; }
    }
}