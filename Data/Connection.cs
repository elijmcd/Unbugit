using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unbugit.Data
{
    public class Connection
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        public static string BuildConnectionString(string databaseUrl)
        {
            var DatabaseUri = new Uri(databaseUrl);
            var userInfo = DatabaseUri.UserInfo.Split(":");

            return new NpgsqlConnectionStringBuilder()
            {
                Host = DatabaseUri.Host,
                Port = DatabaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = DatabaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            }.ToString();
        }
    }
}
