using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DemoSampleTemplate.Core.Extensions.Database
{
    /// <summary>
    /// Extension for SqlDataReader
    /// </summary>
    public static class SqlDataReaderExtensions
    {
        /// <summary>
        /// V2 du GetValue, sans le try-catch et le GetOrdinal bullshit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public static T GetValueUnsafe<T>(this SqlDataReader reader, string colName) => ConvertReaderValue<T>(reader[colName]);

        /// <summary>
        /// GetValue avec un vrai test d'existence de la colonne, mais du coup beaucoup plus lourd
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public static T GetValueSafe<T>(this SqlDataReader reader, string colName)
        {
            // Get the column names
            var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
            // If the column exists in the reader
            if (columns.Any(c => c == colName))
            {
                return ConvertReaderValue<T>(reader[colName]);
            }
            return default(T);
        }

        /// <summary>
        /// V2 du GetValue, sans le try-catch et le GetOrdinal bullshit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public static T GetValueUnsafe<T>(IDataReader reader, string colName) => ConvertReaderValue<T>(reader[colName]);

        /// <summary>
        /// GetValue avec un vrai test d'existence de la colonne, mais du coup beaucoup plus lourd
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public static T GetValueSafe<T>(IDataReader reader, string colName)
        {
            // Get the column names
            var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
            // If the column exists in the reader
            if (columns.Any(c => c == colName))
            {
                return ConvertReaderValue<T>(reader[colName]);
            }
            return default(T);
        }

        /// <summary>
        /// Convertit la valeur récupéré du DbReader (hopefully)dans le bon type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ConvertReaderValue<T>(object value)
        {
            try
            {
                // if the generic type is nullable and the value in the DB is null, we return the default value
                if (value == null || value == DBNull.Value)
                    return default(T);
                else if (typeof(T) == typeof(bool))
                {
                    // Cas particulier des bit SQL qui se castent mal
                    return (T)(object)Convert.ToBoolean(value);
                }
                else if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
                {
                    // Cas particulier des bit SQL qui se castent mal
                    return (T)(object)Convert.ToInt32(value);
                }
                else if (typeof(T) == typeof(string))
                {
                    return (T)(object)Convert.ToString(value);
                }
                else if (typeof(T) == typeof(double) || typeof(T) == typeof(double?))
                {
                    return (T)(object)Convert.ToDouble(value);
                }
                else if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?))
                {
                    return (T)(object)Convert.ToDecimal(value);
                }
                else
                {
                    return (T)value;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = "Canno't cast value with type " + value.GetType().Name + " to type " + typeof(T).Name;
                throw;
            }
        }
    }
}
