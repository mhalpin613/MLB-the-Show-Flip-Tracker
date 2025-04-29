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

    public static void ShowPlayerProjection(CurrentAttributes current, CurrentAttributes projected)
    {
        var table = new Table()
            .AddColumn(new TableColumn("[bold]Stat[/]").Centered())
            .AddColumn(new TableColumn("[bold]Change[/]").Centered())
            .Border(TableBorder.Rounded);

        void AddStat(string name, int oldVal, int newVal)
        {
            int diff = newVal - oldVal;
            string arrow;
            string diffText;

            if (diff > 0)
            {
                arrow = "➔";
                diffText = $"[green]{newVal} (+{diff})[/]";
            }
            else if (diff < 0)
            {
                arrow = "➔";
                diffText = $"[red]{newVal} ({diff})[/]";
            }
            else
            {
                arrow = "➔";
                diffText = $"{newVal} (None)";
            }

            table.AddRow($"{name}", $"{oldVal} {arrow} {diffText}");
        }

        AddStat("Contact vs R", current.ContactVsRight, projected.ContactVsRight);
        AddStat("Power vs R", current.PowerVsRight, projected.PowerVsRight);
        AddStat("Contact vs L", current.ContactVsLeft, projected.ContactVsLeft);
        AddStat("Power vs L", current.PowerVsLeft, projected.PowerVsLeft);
        AddStat("Clutch", current.Clutch, projected.Clutch);
        AddStat("Vision", current.Vision, projected.Vision);
        AddStat("Discipline", current.Discipline, projected.Discipline);

        AnsiConsole.Write(table);
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