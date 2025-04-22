using System;
using System.Net.Http;
using System.Threading.Tasks;
using FlipTracker.Services;
using FlipTracker.CLI;
using FlipTracker.Models;
using FlipTracker.Utils;

namespace FlipTracker.CLI;

public static class Menu
{
    public static async Task ShowMainMenu(HttpClient client)
    {
        var api = new ShowDDClient(client);
        var flipper = new Flipper(api);
        var logger = new FlipLogger();

        string prompt = """
        MLB The Show 25 Daily-Flips
        [1] Check Player Price
        [2] Find Today's Best Flips
        [3] Find Budget Flips
        [4] Log Flip
        [5] View Profits
        [6] Exit
        Choose Option > 
        """;

        bool loop = true;

        while (loop)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();

            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine("❌ Invalid input. Please enter 1–6.\n");
                continue;
            }

            switch (choice)
            {
                case 1:
                    await HandlePlayerLookup(api);
                    break;
                case 2:
                    await HandleTopFlipFinder(flipper);
                    break;
                case 3:
                    await HandleBudgetFlipFinder(flipper);
                    break;
                case 4:
                    HandleProfitEntry(logger);
                    break;
                case 5:
                    ShowTotalProfit(logger);
                    break;
                case 6:
                    Console.WriteLine("👋 Exiting...");
                    loop = false;
                    break;
                default:
                    Console.WriteLine("❌ Invalid choice. Try again.\n");
                    break;
            }
        }
    }

    public static async Task HandleBudgetFlipFinder(Flipper flipper)
    {
        Console.Write("Enter your max stubs (e.g. 2500): ");
        if (!int.TryParse(Console.ReadLine(), out int maxBuy) || maxBuy <= 0)
        {
            Console.WriteLine("❌ Invalid input. Please enter a positive number.");
            return;
        }

        Console.WriteLine($"\n🔎 Searching for profitable flips under {maxBuy} stubs...\n");

        var flips = await flipper.GetFlipsAsync(
            minProfit: 250,
            maxBuy: maxBuy,
            rarities: new List<string> { "diamond", "gold" }
        );

        if (flips.Count == 0)
        {
            Console.WriteLine("❌ No good flips found under your budget.");
            return;
        }

        Printer.PrintFlipsTable(flips);
    }

    private static async Task HandlePlayerLookup(ShowDDClient api)
    {
        Console.Write("Enter a player name (e.g. 'Joe Ryan') > ");
        var name = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("⚠️ Invalid player name.");
            return;
        }

        Spinner.Start("Searching");
        var matches = await api.SearchPlayersByNameAsync(name);
        Spinner.Stop();

        if (matches.Count == 0)
        {
            Console.WriteLine("❌ No matching listings found.");
        }
        else
        {
            Console.WriteLine($"\n🔎 Results for: {name}\n");
            Printer.PrintHeader();
            foreach (var match in matches)
            {
                Printer.PrintTableRow(
                    match.Name, match.Rarity, match.Ovr, match.Team, match.Buy, match.Sell
                );
            }
            Console.WriteLine(new string('-', 76));
        }
    }

    static void HandleProfitEntry(FlipLogger logger)
    {
        Console.Write("Enter player name: ");
        string? name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("❌ Invalid name.");
            return;
        }

        Console.Write("Enter team: ");
        string? team = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(team))
        {
            Console.WriteLine("❌ Invalid team.");
            return;
        }

        Console.Write("Enter buy price: ");
        if (!int.TryParse(Console.ReadLine(), out int buy) || buy < 0)
        {
            Console.WriteLine("❌ Invalid buy price.");
            return;
        }

        Console.Write("Enter sell price: ");
        if (!int.TryParse(Console.ReadLine(), out int sell) || sell <= 0)
        {
            Console.WriteLine("❌ Invalid sell price.");
            return;
        }

        logger.LogFlip(name.Trim(), team.Trim(), buy, sell);
        int profit = (int)(sell * 0.9 - buy);
        Console.WriteLine($"✅ Logged! Profit: {profit} stubs\n");
    }

    public static void ShowTotalProfit(FlipLogger logger)
    {
        int total = logger.GetTotalProfit();
        Console.WriteLine($"\n💰 Total Profit: {total} stubs\n");
    }

    private static async Task HandleTopFlipFinder(Flipper flipper)
    {
        var topFlips = await flipper.GetFlipsAsync();
        Printer.PrintFlipsTable(topFlips, topFlips: true);
    }
}