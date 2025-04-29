using FlipTracker.Models;

namespace FlipTracker.Utils;

public static class ProjectionCalculator
{
    private static int CalculateSmartBump(double target, int current)
    {
        double diff = target - current;

        if (Math.Abs(diff) < 3)
            return 0; // Ignore tiny differences

        double scaledBump = diff * 0.35;

        // Clamp maximum bump to 15 up or 15 down
        if (scaledBump > 15) scaledBump = 15;
        if (scaledBump < -15) scaledBump = -15;

        return Convert.ToInt32(Math.Round(scaledBump));
    }


    public static CurrentAttributes Project(PlayerStats stats, CurrentAttributes current)
    {
        int targetCONR = Convert.ToInt32(stats.AVGvR * 479.59 - 43.839);
        int targetCONL = Convert.ToInt32(stats.AVGvL * 479.59 - 43.839);
        int targetPOWR = Convert.ToInt32(stats.ISOvR * 269.75 + 29.981);
        int targetPOWL = Convert.ToInt32(stats.ISOvL * 269.75 + 29.981);
        int targetVISION = Convert.ToInt32(stats.Kper * -2.7022 + 131.02);
        int targetDISCIPLINE = Convert.ToInt32(stats.BBper * 6.051 + 23.165);
        int targetCLUTCH = Convert.ToInt32(stats.AVGwRISP * 479.59 - 43.839);

        return new CurrentAttributes
        {
            ContactVsRight = current.ContactVsRight + CalculateSmartBump(targetCONR, current.ContactVsRight),
            ContactVsLeft = current.ContactVsLeft + CalculateSmartBump(targetCONL, current.ContactVsLeft),
            PowerVsRight = current.PowerVsRight + CalculateSmartBump(targetPOWR, current.PowerVsRight),
            PowerVsLeft = current.PowerVsLeft + CalculateSmartBump(targetPOWL, current.PowerVsLeft),
            Vision = current.Vision + CalculateSmartBump(targetVISION, current.Vision),
            Discipline = current.Discipline + CalculateSmartBump(targetDISCIPLINE, current.Discipline),
            Clutch = current.Clutch + CalculateSmartBump(targetCLUTCH, current.Clutch)
        };
    }
}