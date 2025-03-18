using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DemoSampleTemplate.Core.Extensions.EntityFramework
{
    public static class TempTableExtensions
    {
        public static async Task BulkInsertIntoTempTableAsync<T>(this DbContext ctx, IEnumerable<T> data, CancellationToken cancellationToken = default) where T : class
        {
            ValidateArguments(ctx);

            var sql = GetCreateTableSql(ctx, typeof(T));

            try
            {
                await ctx.Database.ExecuteSqlRawAsync(sql, cancellationToken);
                using SqlBulkCopy bulkCopy = ctx.GetSqlBulkCopy(typeof(T));
                await bulkCopy.WriteToServerAsync(GetDataTable(data), cancellationToken);
            }
            catch (Exception)
            {
                if (ctx.Database.GetDbConnection().State != ConnectionState.Closed)
                {
                    ctx.Database.CloseConnection();
                }
                throw;
            }
        }

        private static string GetCreateTableSql(DbContext ctx, Type tempTableEntityType)
        {
            var sqlGenHelper = ctx.GetService<ISqlGenerationHelper>();

            var entity = ctx.Model.FindEntityType(tempTableEntityType);
            var tableName = entity.GetTableName();
            var escapedTableName = sqlGenHelper.DelimitIdentifier(tableName);

            var properties = tempTableEntityType.GetProperties();

            var columns = new List<string>();
            foreach (var p in properties)
            {
                var property = entity.FindProperty(p.Name);
                var columnName = property.GetColumnName();
                var escapedColumnName = sqlGenHelper.DelimitIdentifier(columnName);
                var columnType = property.GetColumnType();
                var nullability = property.IsNullable ? "NULL" : "NOT NULL";
                var pKey = property.IsPrimaryKey() ? "PRIMARY KEY" : null;

                columns.Add($"{escapedColumnName} {columnType} {pKey} {nullability}");
            }

            var sql = $@"CREATE TABLE {escapedTableName}
            (
                {string.Join(",", columns)}
            );";
            return sql;
        }

        private static SqlBulkCopy GetSqlBulkCopy(this DbContext ctx, Type tempTableEntityType)
        {
            var sqlCon = ctx.Database.GetDbConnection() as SqlConnection;
            var sqlTx = ctx.Database.CurrentTransaction?.GetDbTransaction() as SqlTransaction;

            var sqlGenHelper = ctx.GetService<ISqlGenerationHelper>();
            var entity = ctx.Model.FindEntityType(tempTableEntityType);
            var tableName = entity.GetTableName();
            var escapedTableName = sqlGenHelper.DelimitIdentifier(tableName);

            var sqlBulkCopy = new SqlBulkCopy(sqlCon, SqlBulkCopyOptions.Default, sqlTx) { DestinationTableName = escapedTableName };

            var properties = tempTableEntityType.GetProperties();
            foreach (var p in properties)
            {
                sqlBulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping { SourceColumn = p.Name, DestinationColumn = p.Name });
            }

            return sqlBulkCopy;
        }

        private static void ValidateArguments(DbContext ctx)
        {
            if (ctx.Database.GetDbConnection().State != ConnectionState.Open)
            {
                throw new ArgumentException("Connection is not open. Please open it first!");
            }
        }

        private static DataTable GetDataTable<T>(IEnumerable<T> data)
        {
            var dataTable = new DataTable();

            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                dataTable.Columns.Add(property.Name, property.PropertyType);
            }

            foreach (var item in data)
            {
                var row = dataTable.NewRow();
                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(item);
                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

        public static void ConfigureTempTableEntity<TEntity>(this ModelBuilder modelBuilder) where TEntity : class
        {
            modelBuilder.Entity<TEntity>().HasNoKey();
        }

    }
}
