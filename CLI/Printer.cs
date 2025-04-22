using FlipTracker.Models;
using Spectre.Console;

namespace FlipTracker.CLI;

public static class Printer
{
    public static void PrintFlipsTable(List<Flip> flips)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[bold]Name[/]")
            .AddColumn("Team")
            .AddColumn("Buy")
            .AddColumn("Sell")
            .AddColumn("Margin");

        foreach (var flip in flips)
        {
            table.AddRow(
                flip.Name,
                flip.Team,
                $"[cyan]{flip.Buy.ToString()}[/]",
                $"[red]{flip.Sell.ToString()}[/]",
                $"[green]{flip.Margin}[/]"
            );
        }
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public static void PrintPlayerResults(List<Player> players)
    {
        var table = new Table().Border(TableBorder.Rounded);

        table.AddColumn("[bold white]Name[/]");
        table.AddColumn("[bold cyan]Rarity[/]");
        table.AddColumn("[bold]OVR[/]");
        table.AddColumn("[bold]Team[/]");
        table.AddColumn("[blue]Buy Now[/]");
        table.AddColumn("[red]Sell Now[/]");

        foreach (var p in players)
        {
            string rarityColor = p.Rarity switch
            {
                "Diamond" => "cyan",
                "Gold" => "gold1",
                "Silver" => "silver",
                "Bronze" => "darkorange3",
                _ => "grey62"
            };

            table.AddRow(
                p.Name,
                $"[{rarityColor}]{p.Rarity}[/]",
                p.Ovr.ToString(),
                p.Team,
                $"[blue]{p.Buy}[/]",
                $"[red]{p.Sell}[/]"
            );
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public static void PrintProfitLog(List<Flip> flips)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("Flip Profit Log")
            .AddColumn("Name")
            .AddColumn("Team")
            .AddColumn(new TableColumn("Buy").RightAligned())
            .AddColumn(new TableColumn("Sell").RightAligned())
            .AddColumn(new TableColumn("Margin").RightAligned());

        foreach (var flip in flips)
        {
            table.AddRow(
                flip.Name,
                flip.Team,
                $"[blue]{flip.Buy}[/]",
                $"[red]{flip.Sell}[/]",
                $"[green]{flip.Margin}[/]"
            );
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public static void PrintSectionHeader(string title)
    {
        AnsiConsole.Write(new Rule($"[bold yellow]{title}[/]").RuleStyle("grey").Centered());
    }

    public static void PrintSuccess(string message)
    {
        AnsiConsole.MarkupLine($"[green]✔ {message}[/]");
    }

    public static void PrintWarning(string message)
    {
        AnsiConsole.MarkupLine($"[yellow]! {message}[/]");
    }

    public static void PrintError(string message)
    {
        AnsiConsole.MarkupLine($"[red]✖ {message}[/]");
    }
}