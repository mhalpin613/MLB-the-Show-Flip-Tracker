using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using FlipTracker.Services;
using FlipTracker.Models;
using FlipTracker.Utils;
using Spectre.Console;

namespace FlipTracker.CLI;

public static class Menu
{
    public static async Task ShowMainMenu(HttpClient client)
    {
        var api = new ShowDDClient(client);
        var flipper = new Flipper(api);
        var logger = new FlipLogger();

        bool loop = true;
        while (loop)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]MLB The Show 25 Daily-Flips[/]")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                        "1. Check Player Price",
                        "2. Today's Best Flips",
                        "3. Find Budget Flips",
                        "4. Log Flip",
                        "5. View Profits",
                        "6. Exit"
                    }));

            switch (choice[0])
            {
                case '1':
                    await HandlePlayerLookup(api);
                    break;
                case '2':
                    await HandleTopFlipFinder(flipper);
                    break;
                case '3':
                    await HandleBudgetFlipFinder(flipper);
                    break;
                case '4':
                    HandleProfitEntry(logger);
                    break;
                case '5':
                    logger.ShowProfitLedger();
                    break;
                case '6':
                    AnsiConsole.MarkupLine("[yellow]👋 Exiting...[/]");
                    loop = false;
                    break;
            }
        }
    }

    public static async Task HandleBudgetFlipFinder(Flipper flipper)
    {
        var maxBuy = AnsiConsole.Ask<int>("Enter your [green]max stubs[/] (e.g. 2500):");

        AnsiConsole.MarkupLine($"\n[cyan]🔎 Searching for profitable flips under {maxBuy} stubs...[/]\n");

        Spinner.Start("Searching");
        var flips = await flipper.GetFlipsAsync(
            minProfit: 250,
            maxBuy: maxBuy,
            rarities: new List<string> { "diamond", "gold" }
        );
        Spinner.Stop();

        if (flips.Count == 0)
        {
            AnsiConsole.MarkupLine("\n[red]❌ No good flips found under your budget.[/]\n");
            return;
        }

        Printer.PrintFlipsTable(flips);
    }

    private static async Task HandlePlayerLookup(ShowDDClient api)
    {
        var name = AnsiConsole.Ask<string>("Enter a [green]player name[/] (e.g. 'Joe Ryan'):");

        Spinner.Start("Searching");
        var matches = await api.SearchPlayersByNameAsync(name);
        Spinner.Stop();

        if (matches.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]❌ No matching listings found.[/]\n");
        }
        else
        {
            AnsiConsole.MarkupLine($"\n[cyan]🔎 Results for: {name}[/]\n");
            Printer.PrintPlayerResults(matches);
        }
    }

    static void HandleProfitEntry(FlipLogger logger)
    {
        var name = AnsiConsole.Ask<string>("Enter [green]player name[/]:");
        var team = AnsiConsole.Ask<string>("Enter [green]team[/]:");
        var buy = AnsiConsole.Ask<int>("Enter [green]buy price[/]:");
        var sell = AnsiConsole.Ask<int>("Enter [green]sell price[/]:");

        logger.LogFlip(name.Trim(), team.Trim(), buy, sell);
        int profit = (int)(sell * 0.9 - buy);
        AnsiConsole.MarkupLine($"[green]✅ Logged! Profit: {profit} stubs[/]\n");
    }

    private static async Task HandleTopFlipFinder(Flipper flipper)
    {
        AnsiConsole.MarkupLine("[yellow]🔥 Today's Best Flips 🔥\n[/]");
        Spinner.Start("Searching");
        var topFlips = await flipper.GetFlipsAsync();
        Spinner.Stop();
        Printer.PrintFlipsTable(topFlips);
    }
}