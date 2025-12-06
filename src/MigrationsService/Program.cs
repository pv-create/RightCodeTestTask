using Microsoft.Extensions.Configuration;
using Npgsql;
using Shared.Options;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var dbOptions = new DatabaseOptions();

configuration.GetSection(DatabaseOptions.SectionName).Bind(dbOptions);

await using var connection = new NpgsqlConnection(dbOptions.ToConnectionString());
await connection.OpenAsync();

var commands = new[]
{
    """
    CREATE TABLE IF NOT EXISTS users (
        id uuid PRIMARY KEY,
        name varchar(128) NOT NULL UNIQUE,
        passwordhash text NOT NULL
    );
    """,
    """
    CREATE TABLE IF NOT EXISTS user_favorites (
        id uuid PRIMARY KEY,
        userid uuid NOT NULL REFERENCES users(id) ON DELETE CASCADE,
        currencycode varchar(16) NOT NULL
    );
    """,
    """
    CREATE UNIQUE INDEX IF NOT EXISTS ix_user_favorites_userid_currencycode ON user_favorites(userid, currencycode);
    """,
    """
    CREATE TABLE IF NOT EXISTS currency (
        id uuid PRIMARY KEY,
        code varchar(16) NOT NULL UNIQUE,
        name varchar(256) NOT NULL,
        rate numeric(18,6) NOT NULL,
        updatedat timestamp with time zone NOT NULL
    );
    """,
    """
    CREATE UNIQUE INDEX IF NOT EXISTS ix_currency_code ON currency(code);
    """
};

foreach (var sql in commands)
{
    await using var cmd = new NpgsqlCommand(sql, connection);
    await cmd.ExecuteNonQueryAsync();
}

Console.WriteLine("SQL migrations applied");
