namespace FlipTracker.Models;

public record Player(
    string Name,
    string Rarity,
    int Ovr,
    string Team,
    int Buy,
    int Sell
);