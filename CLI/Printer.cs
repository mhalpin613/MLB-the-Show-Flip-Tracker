namespace FlipTracker.CLI;
using FlipTracker.Utils;
using FlipTracker.Models;

public static class Printer
{
    public static void PrintFlipsTable(List<Flip> flips, bool topFlips = false)
    {
        if (topFlips) Console.WriteLine("\n📈 Top 25 Flips\n");
        Console.WriteLine($"{"Name",-22} {"Team",-14} {"Buy",10} {"Sell",10} {"Margin",10}");
        Console.WriteLine(new string('-', 70));

        foreach (var flip in flips)
        {
            string buy = $"{Colors.BLUE}{flip.Buy,10}{Colors.RESET}";
            string sell = $"{Colors.RED}{flip.Sell,10}{Colors.RESET}";
            string margin = $"{Colors.GREEN}{flip.Margin,10}{Colors.RESET}";

            Console.WriteLine($"{flip.Name,-22} {flip.Team,-14} {buy} {sell} {margin}");
        }
        Console.WriteLine(new string('-', 70));
    }

    public static void PrintHeader()
    {
        Console.WriteLine($"{"Name",-25}{"Rarity",-12}{"OVR",-7}{"Team",-12}{"Buy Now",10}{"Sell Now",10}");
        Console.WriteLine(new string('-', 76));
    }

    public static void PrintTableRow(string name, string rarity, int ovr, string team, int buy, int sell)
    {
        // Optional: adjust width/padding
        string line = $"{name,-25}{rarity,-12}{ovr,-7}{team,-12}{buy,10}{sell,10}";

        // Color by rarity
        string color = rarity.ToLower() switch
        {
            "diamond" => Colors.CYAN, // cyan
            "gold" => Colors.YELLOW,    // yellow
            "silver" => Colors.WHITE,  // white
            "bronze" => Colors.RED,  // red
            "common" => Colors.GRAY,  // gray
            _ => ""
        };

        Console.WriteLine($"{color}{line}{Colors.RESET}");
    }
}