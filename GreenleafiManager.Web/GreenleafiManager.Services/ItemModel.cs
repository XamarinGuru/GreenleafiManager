using System;
using System.Collections.Generic;

namespace GreenleafiManager.Services
{
    public class ItemModel
    {
        public Guid Id { get; set; }
        public string OriginalId { get; set; }
        public string GlItemCode { get; set; }
        public string MetalCode { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public decimal TagPrice { get; set; }
        public string Description { get; set; }
        public string SecretCode { get; set; }
        public string Info1 { get; set; }
        public string Info2 { get; set; }
        public string Info3 { get; set; }
        public bool IsSold { get; set; }
        public string GlCode { get; set; }
        public decimal? GlCost { get; set; }
        public string VendorCode { get; set; }
        public string VariantId { get; set; }
        public string InvoiceDescription { get; set; }
        public string LocationId { get; set; }
        public string InvoiceId { get; set; }
        public bool ShowOnWebsite { get; set; }
        public DateTime LastCheckedDate { get; set; }
        public decimal ShopifyPrice { get; set; }


        public ItemPictureModel First { get; set; }
        public ItemPictureModel Second { get; set; }
        public ItemPictureModel Third { get; set; }


        public void addPictures(List<ItemPictureModel> list)
        {
            First = new ItemPictureModel();
            Second = new ItemPictureModel();
            Third = new ItemPictureModel();
            if (list.Count == 1)
                SendToProperPosition(list[0]);
            else if (list.Count == 2)
            {
                SendToProperPosition(list[0]);
                SendToProperPosition(list[1]);
            }
            else if (list.Count == 3)
            {
                SendToProperPosition(list[0]);
                SendToProperPosition(list[1]);
                SendToProperPosition(list[2]);
            }
        }

        private void SendToProperPosition(ItemPictureModel picture)
        {
            if (picture.position == "1")
                First = picture;
            else if (picture.position == "2")
                Second = picture;
            else if (picture.position == "3")
                Third = picture;
        }
    }
}
