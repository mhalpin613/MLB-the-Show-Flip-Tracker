namespace FlipTracker.Models;

public class PlayerStats
{
    public string Name { get; set; } = "";
    public double AVGvL { get; set; }
    public double AVGvR { get; set; }
    public double ISOvR { get; set; }
    public double ISOvL { get; set; }
    public double Kper { get; set; }
    public double BBper { get; set; }
    public int PAvL { get; set; }
    public int PAvR { get; set; }
    public int PAwRISP { get; set; }
    public double AVGwRISP { get; set; }
}