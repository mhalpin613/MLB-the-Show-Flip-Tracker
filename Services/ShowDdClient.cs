using FlipTracker.Models;
using System.Net.Http;
using System.Text.Json;

namespace FlipTracker.Services;

public class ShowDdClient
{
    private readonly HttpClient _client;
    private const string BaseUrl = "https://mlb25.theshow.com/apis/listings.json";

    public ShowDdClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<List<JsonElement>> GetListingsAsync(
        string type = "mlb_card",
        List<string>? rarities = null,
        string name = "",
        int? minBuy = null,
        int? maxBuy = null,
        int? minSell = null,
        int? maxSell = null
    )
    {
        var listings = new List<JsonElement>();
        var rarityList = rarities ?? new List<string> { "" };

        foreach (var rarity in rarityList)
        {
            var urlParams = new List<string> { $"type={type}" };
            if (!string.IsNullOrWhiteSpace(rarity)) urlParams.Add($"rarity={rarity}");
            if (!string.IsNullOrWhiteSpace(name)) urlParams.Add($"name={Uri.EscapeDataString(name)}");
            if (minBuy.HasValue) urlParams.Add($"min_best_buy_price={minBuy}");
            if (maxBuy.HasValue) urlParams.Add($"max_best_buy_price={maxBuy}");
            if (minSell.HasValue) urlParams.Add($"min_best_sell_price={minSell}");
            if (maxSell.HasValue) urlParams.Add($"max_best_sell_price={maxSell}");

            string query = string.Join("&", urlParams);
            var firstResponse = await _client.GetStringAsync($"{BaseUrl}?{query}&page=1");
            using var firstJson = JsonDocument.Parse(firstResponse);
            int totalPages = firstJson.RootElement.GetProperty("total_pages").GetInt32();

            var tasks = Enumerable.Range(1, totalPages).Select(async page =>
            {
                string url = $"{BaseUrl}?{query}&page={page}";
                var response = await _client.GetStringAsync(url);
                using var json = JsonDocument.Parse(response);
                return json.RootElement.GetProperty("listings")
                    .EnumerateArray()
                    .Select(e => e.Clone())
                    .ToList();
            });
            var results = await Task.WhenAll(tasks);
            listings.AddRange(results.SelectMany(p => p));
        }
        return listings;
    }

    public async Task<List<Player>> SearchPlayersByNameAsync(string query)
    {
        bool exact = query.Trim().Contains(' ');
        var jsonListings = await GetListingsAsync(name: query);

        var players = new List<Player>();

        foreach (var listing in jsonListings)
        {
            var item = listing.GetProperty("item");
            string name = item.GetProperty("name").GetString() ?? "";

            if ((exact && name.Equals(query, StringComparison.OrdinalIgnoreCase)) ||
                (!exact && name.Contains(query, StringComparison.OrdinalIgnoreCase)))
            {
                int buy = listing.GetProperty("best_buy_price").GetInt32();
                int sell = listing.GetProperty("best_sell_price").GetInt32();

                players.Add(new Player(
                    name,
                    item.GetProperty("rarity").GetString() ?? "",
                    item.GetProperty("ovr").GetInt32(),
                    item.GetProperty("team").GetString() ?? "",
                    buy,
                    sell
                ));
            }
        }

        return players;
    }
}