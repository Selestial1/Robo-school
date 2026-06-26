using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace RoboSchool.Data;

public static class DatabaseConfiguration
{
    public static string ResolveConnectionString(IConfiguration configuration)
    {
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        if (!string.IsNullOrWhiteSpace(databaseUrl))
            return ParseDatabaseUrl(databaseUrl);

        return configuration.GetConnectionString("Default") ?? "Data Source=../data/robo.db";
    }

    public static bool IsPostgreSql(string connectionString) =>
        connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase) ||
        connectionString.StartsWith("postgres", StringComparison.OrdinalIgnoreCase);

    public static void ConfigureDbContext(DbContextOptionsBuilder options, string connectionString)
    {
        if (IsPostgreSql(connectionString))
            options.UseNpgsql(connectionString);
        else
            options.UseSqlite(connectionString);
    }

    private static string ParseDatabaseUrl(string databaseUrl)
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':', 2);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = uri.AbsolutePath.TrimStart('/'),
            Username = Uri.UnescapeDataString(userInfo[0]),
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "",
            SslMode = SslMode.Require,
        };

        return UseDirectNeonHost(builder).ConnectionString;
    }

    private static NpgsqlConnectionStringBuilder UseDirectNeonHost(NpgsqlConnectionStringBuilder builder)
    {
        // Neon pooler не поддерживает CREATE TABLE — нужно прямое подключение.
        if (builder.Host?.Contains("-pooler.", StringComparison.Ordinal) == true)
            builder.Host = builder.Host.Replace("-pooler.", ".", StringComparison.Ordinal);

        return builder;
    }
}
