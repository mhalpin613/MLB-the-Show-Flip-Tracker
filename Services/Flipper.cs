using FlipTracker.Models;
using System.Text.Json;

namespace FlipTracker.Services;

public class Flipper
{
    private readonly ShowDdClient _client;

    public Flipper(ShowDdClient client)
    {
        _client = client;
    }

    public async Task<List<Flip>> GetFlipsAsync(
        int limit = 25, 
        int minProfit = 500, 
        int? maxBuy = null,
        List<string>? rarities = null
    )
    {
        var listings = await _client.GetListingsAsync(
            rarities: rarities ?? new List<string> { "diamond" }, 
            maxBuy: maxBuy
        );
        var flips = new List<Flip>();

        foreach (var listing in listings)
        {
            if (!listing.TryGetProperty("best_buy_price", out var buyProp) ||
                !listing.TryGetProperty("best_sell_price", out var sellProp))
                continue;

            int buy = buyProp.GetInt32();
            int sell = sellProp.GetInt32();
            if (buy == 0 || sell == 0) continue;

            int margin = (int)(sell * 0.9 - buy);
            if (margin < minProfit) continue;

            var item = listing.GetProperty("item");
            flips.Add(new Flip(
                item.GetProperty("name").GetString() ?? "Unknown",
                item.GetProperty("team").GetString() ?? "",
                buy,
                sell,
                margin
            ));
        }
        return flips.OrderByDescending(f => f.Margin).Take(limit).ToList();
    }
}