using System;
using System.Collections.Generic;
using System.Linq;
using GreenLeafMobileService.DataObjects;
using GreenLeafMobileService.Models;

namespace GreenLeafMobileService.Repositories {
    public class ItemRepository : GenericRepository<Item> {
        public ItemRepository ( GreenLeafMobileContext context ) : base( context ) {
        }

        public override string TableName => "[dbo].[Items]";
        public string LocationId => "LocationId";
        public string InvoiceId => "InvoiceId";
        public string Price => "Price";

        public void UpsertBulk ( List<Item> entities ) {
            using ( var connection = OpenConnection() ) {
                using ( var transaction = connection.BeginTransaction() ) {
                    try {
                        const string replicaTableName = "#Items";

                        CreateTempReplicaTable( TableName, replicaTableName, connection, transaction );

                        var mergePropertyName = new List<string> {OriginalId};

                        var propertiesToUpdate = CommonReflector.GetDbPropertiesNamesExcept( new List<string> {
                            Id, Price, LocationId, InvoiceId, OriginalId, CreatedAt, Deleted
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

        public void SetDefaultColumns ( List<Item> entities ) {
            entities.ForEach( entity => {
                entity.Id = Guid.NewGuid().ToString();
                entity.CreatedAt = DateTimeOffset.UtcNow;
                entity.UpdatedAt = DateTimeOffset.UtcNow;
                entity.Deleted = false;
            } );
        }
    }
}