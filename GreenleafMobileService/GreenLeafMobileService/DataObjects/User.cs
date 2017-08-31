using System.Collections.Generic;
using Microsoft.Azure.Mobile.Server;

namespace GreenLeafMobileService.DataObjects {
    public class User : EntityData {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public string RoleId { get; set; }
        public virtual Role Role { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}