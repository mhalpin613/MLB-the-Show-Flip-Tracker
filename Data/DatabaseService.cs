using Microsoft.Data.Sqlite;

namespace FlipTracker.Services;

public static class DatabaseService
{
    private const string DbPath = "Data/FlipTracker.db";

    public static SqliteConnection GetConnection()
    {
        Directory.CreateDirectory("Data");
        return new SqliteConnection($"Data Source={DbPath}");
    }

    public static void InitializeDatabase()
    {
        using var conn = GetConnection();
        conn.Open();

        string createTable = @"
        CREATE TABLE IF NOT EXISTS Flips (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Team TEXT,
            Buy INTEGER NOT NULL,
            Sell INTEGER NOT NULL,
            Profit INTEGER NOT NULL,
            Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
        );";

        using var cmd = conn.CreateCommand();
        cmd.CommandText = createTable;
        cmd.ExecuteNonQuery();
    }
}