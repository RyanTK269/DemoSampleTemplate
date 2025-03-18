using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DemoSampleTemplate.Core.Extensions.DataType
{
    public static class DataTableExtensions
    {
        public static DataTable ConvertToDataTable<T>(this IEnumerable<T> enumerable, string tableName, IEnumerable<string>? orderedColumnNames = null)
        {
            var dataTable = new DataTable(tableName);
            if (typeof(T).IsValueType || typeof(T).FullName.Equals("System.String"))
            {
                var columnName = orderedColumnNames == null || !orderedColumnNames.Any() ? "NONAME" : orderedColumnNames.First();
                dataTable.Columns.Add(columnName, typeof(T));
                foreach (T obj in enumerable)
                {
                    var row = dataTable.NewRow();
                    row[columnName] = obj;
                    dataTable.Rows.Add(row);
                }
            }
            else
            {
                PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo[] readableProperties = properties.Where(w => w.CanRead).ToArray();
                if (readableProperties.Length > 1 && (orderedColumnNames == null || !orderedColumnNames.Any()))
                    throw new ArgumentException("Ordered list of column names must be provided.");

                var columnNames = (orderedColumnNames ?? readableProperties.Select(s => s.Name)).ToArray();

                foreach (string name in columnNames)
                {
                    dataTable.Columns.Add(name, readableProperties.Single(s => s.Name.Equals(name)).PropertyType);
                }

                foreach (T obj in enumerable)
                {
                    dataTable.Rows.Add(columnNames.Select(s => readableProperties.Single(s2 => s2.Name.Equals(s)).GetValue(obj)).ToArray());
                }
            }
            return dataTable;
        }
    }
}
