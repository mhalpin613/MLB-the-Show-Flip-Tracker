using System.Net.Http;
using FlipTracker.CLI;

namespace FlipTracker;

public class Program
{
    public static async Task Main(string[] args)
    {
        using var client = new HttpClient();
        await Menu.ShowMainMenu(client);
    }
}