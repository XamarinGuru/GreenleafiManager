using System.Collections.Generic;
using Microsoft.Azure.Mobile.Server;

namespace GreenleafiManager.Api.DataObjects {
    public class Role : EntityData {
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}