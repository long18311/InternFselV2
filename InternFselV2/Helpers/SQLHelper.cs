using InternFselV2.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text.RegularExpressions;

namespace InternFselV2.Helpers
{
    public static class SQLHelper
    {

        
        
        private static readonly HashSet<Type> NumericType = new HashSet<Type>
        {
            typeof(byte), typeof(sbyte), typeof(short), typeof(ushort),
            typeof(int), typeof(uint), typeof(long), typeof(ulong),
            typeof(float), typeof(double), typeof(decimal)
        };
        private static IConfigurationRoot _configuration { get; set; }
        static SQLHelper()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            _configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
               .Build();

        }

        public static string GetSQLCreateEntity<T>(T entity, IQueryable<T> queryable) where T : Entity
        {
            string sql = $"INSERT INTO [{GetDatabaseName()}].[dbo].{GetTableName(queryable)}";
            if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => !p.IsDefined(typeof(NotMappedAttribute), true) && (!typeof(IEnumerable).IsAssignableFrom(p.PropertyType) || p != typeof(string)) && !typeof(Entity).IsAssignableFrom(p.PropertyType));
            List<string> fields = new List<string>();
            List<string> values = new List<string>();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(entity);
                if (value != null)
                {
                    fields.Add($"[{prop.Name}]");
                    if (NumericType.Contains(prop.PropertyType))
                    {
                        values.Add(value.ToString()!);
                    }
                    else if (prop.PropertyType == typeof(string) || prop.PropertyType.IsEnum)
                    {
                        value = value.ToString()?.Replace("'", "''");
                        values.Add($"N'{value}'");
                    }
                    else
                    {
                        values.Add($"'{value}'");
                    }

                }
            }
            sql = sql + '(' + string.Join(',', fields.Where(x => !string.IsNullOrEmpty(x)).ToList()) + ") VALUES (" + string.Join(',', values.Where(x => !string.IsNullOrEmpty(x)).ToList()) + ")";
            return sql;
        }
        private static string GetTableName<T>(IQueryable<T> queryable) where T : Entity
        {
            string sql = queryable.ToQueryString();
            Regex regex = new Regex(@"FROM\s+(?<table>.+)\s+AS");
            Match match = regex.Match(sql);

            string table = match.Groups["table"].Value;
            return table;
        }

        public static string GetDatabaseName()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var databaseName = new SqlConnectionStringBuilder(connectionString).InitialCatalog;
            return databaseName;
        }
    }
}
