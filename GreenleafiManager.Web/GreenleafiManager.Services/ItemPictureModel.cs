using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenleafiManager.Services
{
    public class ItemPictureModel
    {
        public string Id { get; set; }
        public string OriginalId { get; set; }
        public string ShopifyImageId { get; set; }
        public bool MainPic { get; set; }
        public string position { get; set; }
        public string PictureURL { get; set; }
        public string PictureName { get; set; }
    }
}
