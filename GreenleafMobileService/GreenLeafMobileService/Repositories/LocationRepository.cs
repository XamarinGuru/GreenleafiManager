using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using GreenLeafMobileService.DataObjects;
using GreenLeafMobileService.Models;
using GreenLeafMobileService.ViewModels;

namespace GreenLeafMobileService.Repositories {
    public class LocationRepository : GenericRepository<Location> {
        public LocationRepository ( GreenLeafMobileContext context ) : base( context ) {
        }

        public override string TableName => "[dbo].[Locations]";
        public string Tax => "Tax";
        public string Name => "Name";

        public void UpsertBulk ( List<Location> entities ) {
            using ( var connection = OpenConnection() ) {
                using ( var transaction = connection.BeginTransaction() ) {
                    try {
                        const string replicaTableName = "#Locations";

                        CreateTempReplicaTable( TableName, replicaTableName, connection, transaction );

                        var mergePropertyName = new List<string> {OriginalId};

                        var propertiesToUpdate = CommonReflector.GetDbPropertiesNamesExcept( new List<string> {
                            Id, Tax, OriginalId, CreatedAt, Deleted
                        } ).ToList();

                        var propertiesToInsert = CommonReflector.GetDbPropertiesNamesExcept().ToList();

                        BulkCopy( entities, connection, transaction, replicaTableName );
                        MergeTables( TableName, replicaTableName, mergePropertyName,
                            propertiesToUpdate, propertiesToInsert, connection, transaction );

                        transaction.Commit();
                    } catch ( Exception ) {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void SetDefaultColumns ( List<Location> entities ) {
            entities.ForEach( entity => {
                entity.Id = Guid.NewGuid().ToString();
                entity.CreatedAt = DateTimeOffset.UtcNow;
                entity.UpdatedAt = DateTimeOffset.UtcNow;
                entity.Deleted = false;
            } );
        }

        public List<LocationModel> GetAll () {
            var result = new DataTable();
            using ( var connection = OpenConnection() ) {
                var sql = $"SELECT [{Id}],[{Name}] FROM {TableName}";
                using ( var command = new SqlCommand( sql, connection ) ) {
                    using ( var da = new SqlDataAdapter( command ) ) {
                        da.Fill( result );
                        return result.AsEnumerable().Select( l => new LocationModel {
                            Id = l["Id"].ToString(),
                            Name = l["Name"].ToString()
                        } ).ToList();
                    }
                }
            }
        }
    }
}