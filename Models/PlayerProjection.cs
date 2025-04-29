namespace FlipTracker.Models;

public class PlayerProjection
{
    public string Name { get; set; } = "";
    public string Team { get; set; } = "";
    public string Rarity  { get; set; } = "";
    public int Ovr { get; set; }
    public int PAvR { get; set;  }
    public int PAvL { get; set;  }
    public int PAwRisp { get; set;  }
    public int CvR { get; set; }
    public int PvR { get; set; }
    public int CvL { get; set; }
    public int PvL { get; set; }
    public int Disc { get; set; }
    public int Vis { get; set; }
    public int Clutch { get; set; }
    public int Price { get; set; }
    
    public PlayerProjection(
        int pavr,
        int pavl,
        int pawRisp,
        int cvR,
        int pvR,
        int cvL,
        int pvL,
        int disc,
        int vis,
        int clutch
    )
    {
        PAvR = pavr;
        PAvL = pavl;
        PAwRisp = pawRisp;
        CvR = cvR;
        PvR = pvR;
        CvL = cvL;
        PvL = pvL;
        Disc = disc;
        Vis = vis;
        Clutch = clutch;
    }
}