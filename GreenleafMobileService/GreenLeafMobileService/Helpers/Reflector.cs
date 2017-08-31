using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GreenLeafMobileService.Helpers {
    public class Reflector<TEntity> where TEntity : class {
        public IEnumerable<string> GetDbPropertiesNamesExcept ( List<string> exceptNames = null ) {
            return GetDbPropertiesInfo( exceptNames ).Select( p => p.Name );
        }

        public IEnumerable<PropertyInfo> GetDbPropertiesInfo ( List<string> exceptNames = null ) {
            var legalNamesLambda = exceptNames != null
                ? ( n => !exceptNames.Contains( n ) )
                : ( Func<string, bool> ) ( m => true );

            var t = typeof ( TEntity );
            var propertiesInfo = t.GetProperties()
                .Where( p => legalNamesLambda( p.Name )
                             && ( p.PropertyType.IsPrimitive
                                  || p.PropertyType == typeof ( decimal )
                                  || p.PropertyType == typeof ( decimal? )
                                  || p.PropertyType == typeof ( string )
                                  || p.PropertyType == typeof ( int? )
                                  || p.PropertyType == typeof ( long? )
                                  || p.PropertyType == typeof ( bool? )
                                  || p.PropertyType == typeof ( Guid? )
                                  || p.PropertyType == typeof ( Guid )
                                  || p.PropertyType == typeof ( DateTime )
                                  || p.PropertyType == typeof ( DateTime? )
                                  || p.PropertyType == typeof ( DateTimeOffset )
                                  || p.PropertyType == typeof ( DateTimeOffset? ) ) );
            return propertiesInfo;
        }
    }
}