using System;
using System.Data.Entity;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Tables;

namespace GreenLeafMobileService.Helpers {
    public class SimpleMappedEntityDomainManager<TData, TModel> : MappedEntityDomainManager<TData, TModel>
        where TData : class, ITableData,new()
        where TModel : class {
        private readonly Func<TModel, string> _keyString;

        public SimpleMappedEntityDomainManager ( DbContext context,
                                                 HttpRequestMessage request,
                                                 Func<TModel, string> keyString )
            : base( context, request, true ) {
            _keyString = keyString;
        }

        public override SingleResult<TData> Lookup ( string id ) {
            return LookupEntity( p => _keyString( p ) == id );
        }

        public override Task<TData> UpdateAsync ( string id, Delta<TData> patch ) {
            return UpdateEntityAsync( patch, id );
        }

        public override Task<bool> DeleteAsync ( string id ) {
            return DeleteItemAsync( id );
        }

        public static T ChangeType<T> ( object value ) {
            var t = typeof ( T );

            if ( t.IsGenericType && t.GetGenericTypeDefinition() == typeof ( Nullable<> ) ) {
                if ( value == null ) {
                    return default ( T );
                }

                t = Nullable.GetUnderlyingType( t );
            }

            return ( T ) Convert.ChangeType( value, t );
        }
    }
}