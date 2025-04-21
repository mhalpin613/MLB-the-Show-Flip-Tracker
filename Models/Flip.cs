namespace FlipTracker.Models;

public record Flip(
    string Name,
    string Team,
    int Buy,
    int Sell,
    int Margin,
    Double SalesPerMin = 0,
    Double BuysPerMin = 0,
    Double SellsPerMin = 0,
    Double ProfitPerMin = 0,
    Double DayTrend = 0,
    Double WeekTrend = 0,
    Double MonthTrend = 0,
    Double BuysPerHour = 0,
    Double SalesPerHour = 0
);