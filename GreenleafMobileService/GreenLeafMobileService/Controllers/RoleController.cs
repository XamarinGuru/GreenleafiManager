using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using GreenLeafMobileService.DataObjects;
using GreenLeafMobileService.Models;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using System;

namespace GreenLeafMobileService.Controllers {
    //[AuthorizeLevel ( AuthorizationLevel.Role )]
    public class RoleController : TableController<Role> {
        protected override void Initialize ( HttpControllerContext controllerContext ) {
            base.Initialize( controllerContext );
            var context = new GreenLeafMobileContext();
            DomainManager = new EntityDomainManager<Role>(context, Request);//, Services );
        }

        // GET tables/Role
        public IQueryable<Role> GetAllRole () {
            try
            {
                return Query();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        // GET tables/Role/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Role> GetRole ( string id ) {
            return Lookup( id );
        }

        // PATCH tables/Role/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Role> PatchRole ( string id, Delta<Role> patch ) {
            return UpdateAsync( id, patch );
        }

        // POST tables/Role
        public async Task<IHttpActionResult> PostRole ( Role item ) {
            var current = await InsertAsync( item );
            return CreatedAtRoute( "Tables", new {id = current.Id}, current );
        }

        // DELETE tables/Role/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteRole ( string id ) {
            return DeleteAsync( id );
        }
    }
}