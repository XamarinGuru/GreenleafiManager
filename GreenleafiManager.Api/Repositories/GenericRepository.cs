using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using GreenleafiManager.Api.Helpers;
using GreenleafiManager.Api.Models;

namespace GreenleafiManager.Api.Repositories {
    public abstract class GenericRepository<TEntity> where TEntity : class {
        protected readonly Reflector<TEntity> CommonReflector;
        protected readonly MobileServiceContext Context;

        protected GenericRepository ( MobileServiceContext context ) {
            Context = context;
            CommonReflector = new Reflector<TEntity>();
        }

        protected virtual string ConnectionString =>
            ConfigurationManager.ConnectionStrings["GreenleafEntities"].ConnectionString;

        public virtual string TableName => string.Empty;
        protected string OriginalId => "OriginalId";
        protected string Id => "Id";
        protected string CreatedAt => "CreatedAt";
        protected string Deleted => "Deleted";

        protected SqlConnection OpenConnection () {
            var connection = new SqlConnection( ConnectionString );
            connection.Open();
            return connection;
        }

        protected void BulkCopy ( List<TEntity> entities, SqlConnection connection, SqlTransaction transaction, string tempTableName ) {
            var propertyInfo = CommonReflector.GetDbPropertiesInfo().ToList();

            using ( var bulkCopy = new SqlBulkCopy( connection, SqlBulkCopyOptions.Default, transaction ) ) {
                bulkCopy.BatchSize = entities.Count;
                bulkCopy.BulkCopyTimeout = 60000;
                bulkCopy.DestinationTableName = tempTableName;

                var table = new DataTable();
                foreach ( var property in propertyInfo ) {
                    bulkCopy.ColumnMappings.Add( property.Name, property.Name );
                    table.Columns.Add( property.Name, Nullable.GetUnderlyingType( property.PropertyType ) ?? property.PropertyType );
                }

                var values = new object[propertyInfo.Count];
                foreach ( var entity in entities ) {
                    for ( var i = 0; i < values.Length; i++ ) {
                        values[i] = propertyInfo[i].GetValue( entity );
                    }
                    table.Rows.Add( values );
                }

                bulkCopy.WriteToServer( table );
            }
        }

        protected void CreateTempReplicaTable ( string tableName, string tempTableName, SqlConnection connection, SqlTransaction transaction ) {
            var sql = $"SELECT TOP 0 * INTO {tempTableName} FROM {tableName}";
            var command = new SqlCommand( sql, connection, transaction );
            command.ExecuteNonQuery();
        }

        protected void MergeTables ( string tableName, string source, List<string> mergePropertiesNames,
                                     List<string> propertiesToUpdate, List<string> propertiesToInsert,
                                     SqlConnection connection, SqlTransaction transaction ) {
            var sql =
                $@"MERGE {tableName} target
							USING {source} source
							ON {string.Join( " and ", mergePropertiesNames.Select( p => $"target.{p} = source.{p}" ) )} 
							WHEN MATCHED THEN
								UPDATE 
								SET {string.Join( ", ", propertiesToUpdate.Select( p => $"target.{p} = source.{p}" ) )}
							WHEN NOT MATCHED THEN
								INSERT ({string.Join( ", ", propertiesToInsert )})
								VALUES ({string.Join( ", ", propertiesToInsert.Select( p => $"source.{p}" ) )});";

            var command = new SqlCommand( sql, connection, transaction );
            command.ExecuteNonQuery();
        }
    }
}