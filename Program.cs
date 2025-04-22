using System.Net.Http;
using FlipTracker.CLI;
using FlipTracker.Services;

namespace FlipTracker;

public class Program
{
    public static async Task Main(string[] args)
    {
        DatabaseService.InitializeDatabase();
        using var client = new HttpClient();
        await Menu.ShowMainMenu(client);
    }
}