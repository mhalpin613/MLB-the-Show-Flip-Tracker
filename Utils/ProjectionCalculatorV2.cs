using System;
using System.Collections.Generic;
using System.Linq;
using FlipTracker.Models;

namespace FlipTracker.Utils;

public static class ProjectionCalculatorV2
{
    private static int LookupProjected(int current, int expected)
    {
        int diff = expected - current;

        int move = Convert.ToInt32(diff / 2.25); // Normal move 1/3 toward target
        
        if (move > 15)
            move = 15;
        else if (move < -15)
            move = -15;

        return current + move;
    }


    private static int LookupRating(Dictionary<double, int> table, double stat, bool inverse = false)
    {
        if (inverse)
        {
            // Inverse stat (lower is better), like K% for Vision
            foreach (var entry in table.OrderBy(x => x.Key))
            {
                if (stat <= entry.Key)
                    return entry.Value;
            }
        }
        else
        {
            // Normal stat (higher is better), like AVG, BB%, ISO
            foreach (var entry in table.OrderByDescending(x => x.Key))
            {
                if (stat >= entry.Key)
                    return entry.Value;
            }
        }

        // Fallback if stat is worse than any threshold
        return table.Last().Value;
    }

    public static CurrentAttributes Project(PlayerStats stats, CurrentAttributes current)
    {
        // Use ISOvR and ISOvL directly (not calculated from SLG anymore)
        var expectedContactVsR = LookupRating(AttributeLookupTables.ContactTable, stats.AVGvR);
        var expectedContactVsL = LookupRating(AttributeLookupTables.ContactTable, stats.AVGvL);
        var expectedPowerVsR = LookupRating(AttributeLookupTables.ISOTable, stats.ISOvR);
        var expectedPowerVsL = LookupRating(AttributeLookupTables.ISOTable, stats.ISOvL);
        var expectedVision = LookupRating(AttributeLookupTables.VisionTable, stats.Kper, inverse: true);
        var expectedDiscipline = LookupRating(AttributeLookupTables.DisciplineTable, stats.BBper);
        var expectedClutch = LookupRating(AttributeLookupTables.ClutchTable, stats.AVGwRISP);

        return new CurrentAttributes
        {
            ContactVsRight = LookupProjected(current.ContactVsRight, expectedContactVsR),
            ContactVsLeft = LookupProjected(current.ContactVsLeft, expectedContactVsL),
            PowerVsRight = LookupProjected(current.PowerVsRight, expectedPowerVsR),
            PowerVsLeft = LookupProjected(current.PowerVsLeft, expectedPowerVsL),
            Vision = LookupProjected(current.Vision, expectedVision),
            Discipline = LookupProjected(current.Discipline, expectedDiscipline),
            Clutch = LookupProjected(current.Clutch, expectedClutch)
        };
    }
}
