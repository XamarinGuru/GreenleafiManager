using System;
using System.Collections.Generic;
using System.Linq;
using GreenleafiManager.Api.DataObjects;
using GreenleafiManager.Api.Models;

namespace GreenleafiManager.Api.Repositories {
    public class CustomerRepository : GenericRepository<Customer> {
        public CustomerRepository ( MobileServiceContext context ) : base( context ) {
        }

        public override string TableName => "[dbo].[Customers]";

        public void UpsertBulk ( List<Customer> entities ) {
            using ( var connection = OpenConnection() ) {
                using ( var transaction = connection.BeginTransaction() ) {
                    try {
                        const string replicaTableName = "#Customers";

                        CreateTempReplicaTable( TableName, replicaTableName, connection, transaction );

                        var mergePropertyName = new List<string> {OriginalId};

                        var propertiesToUpdate = CommonReflector.GetDbPropertiesNamesExcept( new List<string> {
                            Id, OriginalId, CreatedAt, Deleted
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

        public void SetDefaultColumns ( List<Customer> entities ) {
            entities.ForEach( entity => {
                entity.Id = Guid.NewGuid().ToString();
                entity.CreatedAt = DateTimeOffset.UtcNow;
                entity.UpdatedAt = DateTimeOffset.UtcNow;
                entity.Deleted = false;
            } );
        }
    }
}