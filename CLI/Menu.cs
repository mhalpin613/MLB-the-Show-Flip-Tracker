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

        string prompt = """
        MLB The Show 25 Daily-Flips
        [1] Check Player Price
        [2] Find Today's Best Flips
        [3] Find Budget Flips
        [4] Exit
        Choose Option > 
        """;

        bool loop = true;

        while (loop)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();

            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine("❌ Invalid input. Please enter 1–4.\n");
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
            minProfit: 300,
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

    private static async Task HandleTopFlipFinder(Flipper flipper)
    {
        var topFlips = await flipper.GetFlipsAsync();
        Printer.PrintFlipsTable(topFlips, topFlips: true);
    }
}