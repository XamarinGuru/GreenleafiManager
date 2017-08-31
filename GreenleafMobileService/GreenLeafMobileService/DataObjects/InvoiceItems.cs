using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GreenLeafMobileService.DataObjects
{
    public class InvoiceItems : EntityData
    {
        public string Invoice_Id { get; set; }
        public string Item_Id { get; set; }
    }
}