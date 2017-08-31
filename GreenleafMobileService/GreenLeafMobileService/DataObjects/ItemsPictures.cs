using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenLeafMobileService.DataObjects
{
    public class ItemsPictures
    {
        public ItemsPictures()
        {
            Id = Guid.NewGuid();
            MainPic = false;
        }
        public Guid Id { get; set; }
        public string OriginalId { get; set; }
        public string ShopifyImageId { get; set; }
        public bool MainPic { get; set; }
        public string position { get; set; }

        [NotMapped]
        public string PictureURL
        {
            get
            {
                return "http://greenleafpictures.blob.core.windows.net/greenleafitempictures/" + ShopifyImageId + "_" + OriginalId + ".png";
            }
        }
        [NotMapped]
        public string PictureName
        {
            get
            {
                return ShopifyImageId + "_" + OriginalId + ".png";
            }
        }
    }
}