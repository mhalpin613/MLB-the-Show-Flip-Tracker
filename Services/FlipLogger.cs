using FlipTracker.Models;
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

    public int GetTotalProfit()
    {
        using var conn = DatabaseService.GetConnection();
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Name, Team, Buy, Sell, Profit, Timestamp FROM Flips ORDER BY Timestamp DESC;";
        
        using var reader = cmd.ExecuteReader();

        Console.WriteLine("\n📈 Flip History:\n");
        Console.WriteLine($"{"Name",-22} {"Team",-12} {"Buy",6} {"Sell",6} {"Profit",7} {"Timestamp",20}");
        Console.WriteLine(new string('-', 78));

        int totalProfit = 0;

        while (reader.Read())
        {
            string name = reader.GetString(0);
            string team = reader.GetString(1);
            int buy = reader.GetInt32(2);
            int sell = reader.GetInt32(3);
            int profit = reader.GetInt32(4);
            string timestamp = reader.GetDateTime(5).ToString("yyyy-MM-dd HH:mm");

            totalProfit += profit;

            Console.WriteLine($"{name,-22} {team,-12} {buy,6} {sell,6} {profit,7} {timestamp,20}");
        }

        Console.WriteLine(new string('-', 78));

        return totalProfit;
    }

}