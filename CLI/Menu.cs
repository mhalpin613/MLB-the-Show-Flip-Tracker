using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using FlipTracker.Services;
using FlipTracker.Models;
using FlipTracker.Utils;
using FlipTracker.CLI;
using Spectre.Console;

namespace FlipTracker.CLI;

public static class Menu
{
    public static async Task ShowMainMenu(ShowDdClient api, ProjectionReader reader)
    {
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
                        "6. Player Projection",
                        "7. Exit"
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
                    HandleRosterPrediction(reader);
                    break;
                case '7':
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

    private static async Task HandlePlayerLookup(ShowDdClient api)
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
    
    private static void HandleRosterPrediction(ProjectionReader reader)
    {
        var name = AnsiConsole.Ask<string>("Enter [green]player name[/]:");

        var stats = reader.FindPlayerStats(name);
        if (stats == null)
        {
            AnsiConsole.MarkupLine("[red]❌ No projection found for that player.[/]");
            return;
        }
        
        var debugTable = new Table();
        debugTable.AddColumn("Stat");
        debugTable.AddColumn("Value");

        foreach (var prop in stats.GetType().GetProperties())
        {
            debugTable.AddRow(prop.Name, prop.GetValue(stats)?.ToString() ?? "null");
        }

        AnsiConsole.Write(debugTable);

        /*var table = new Table();
        table.HideHeaders();
        table.AddColumn(new TableColumn("Field").LeftAligned());
        table.AddColumn(new TableColumn("Value").LeftAligned());

        table.AddRow("Team", "Cubs");
        table.AddRow("Name", "Pete Crow-Armstrong");
        table.AddRow("Rarity", "Gold");
        table.AddRow("Ovr", "82");
        table.AddRow("PA vs R", player.PAvR.ToString("00"));
        table.AddRow("PA vs L", player.PAvL.ToString("00"));
        table.AddRow("PA w RISP", player.PAwRisp.ToString("00"));
        table.AddRow("Contact v R", player.CvR.ToString("00"));
        table.AddRow("Power v R", player.PvR.ToString("00"));
        table.AddRow("Contact v L", player.CvL.ToString("00"));
        table.AddRow("Power v L", player.PvL.ToString("00"));
        table.AddRow("Clutch", player.Clutch.ToString("00"));
        table.AddRow("Vision", player.Vis.ToString("00"));
        table.AddRow("Discipline", player.Disc.ToString("00"));
        table.AddRow("Price", "0");

        var panel = new Panel(table)
        {
            Header = new PanelHeader("Player Info", Justify.Center),
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 1, 1, 1)
        };

        AnsiConsole.Write(panel);*/
        
        /*var stats2 = new PlayerStats
        {
            AVGvR = 0.310,
            AVGvL = 0.214,
            ISOvR = 0.262,
            ISOvL = 0.179,
            Kper = 23.0,
            BBper = 4.9,
            AVGwRISP = 0.313
        };

        var current = new CurrentAttributes
        {
            ContactVsRight = 51,
            ContactVsLeft = 57,
            PowerVsRight = 50,
            PowerVsLeft = 34,
            Vision = 52,
            Discipline = 40,
            Clutch = 75
        };*/
        
        var stats2 = new PlayerStats
        {
            AVGvR = 0.226,
            AVGvL = 0.353,
            ISOvR = 0.226,
            ISOvL = 0.000,
            Kper = 24.7,
            BBper = 7.8,
            AVGwRISP = 0.333
        };

        var current = new CurrentAttributes
        {
            ContactVsRight = 59,
            ContactVsLeft = 54,
            PowerVsRight = 60,
            PowerVsLeft = 56,
            Vision = 44,
            Discipline = 49,
            Clutch = 89
        };

        var projected = ProjectionCalculatorV2.Project(stats2, current);
        CLI.Printer.ShowPlayerProjection(current, projected);
    }
}