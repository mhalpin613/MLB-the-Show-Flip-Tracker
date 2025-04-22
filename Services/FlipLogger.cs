using FlipTracker.Models;
using FlipTracker.CLI;
using Microsoft.Data.Sqlite;

namespace FlipTracker.Services;

public class FlipLogger
{
    public void LogFlip(string name, string team, int buy, int sell)
    {
        int profit = (int)(sell * 0.9 - buy);

        using var conn = DatabaseService.GetConnection();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Flips (Name, Team, Buy, Sell, Profit)
            VALUES (@name, @team, @buy, @sell, @profit);";

        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@team", team);
        cmd.Parameters.AddWithValue("@buy", buy);
        cmd.Parameters.AddWithValue("@sell", sell);
        cmd.Parameters.AddWithValue("@profit", profit);

        cmd.ExecuteNonQuery();
    }

    public void ShowProfitLedger()
    {
        var flips = new List<Flip>();
        int totalProfit = 0;

        using var conn = DatabaseService.GetConnection();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Name, Team, Buy, Sell, Profit FROM Flips";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var flip = new Flip(
                reader.GetString(0),  // Name
                reader.GetString(1),  // Team
                reader.GetInt32(2),   // Buy
                reader.GetInt32(3),   // Sell
                reader.GetInt32(4)    // Profit (Margin)
            );
            flips.Add(flip);
            totalProfit += flip.Margin;
        }
        Console.WriteLine("\n📝 Flips Ledger 📝\n");
        Printer.PrintFlipsTable(flips);
        Console.WriteLine($"📈 Total Profit: {totalProfit} stubs\n");
    }
}