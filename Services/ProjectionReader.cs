using System.Text.Json;
using FlipTracker.Models;

namespace FlipTracker.Services;

public class ProjectionReader
{
    private List<PlayerStats> _projections = [];

    public ProjectionReader()
    {
        LoadProjections();
    }

    private void LoadProjections()
    {
        // For now, assume projections.json lives next to your executable
        if (!File.Exists("projections.json"))
        {
            Console.WriteLine("⚠️ projections.json not found. No projections loaded.");
            return;
        }

        string json = File.ReadAllText("projections.json");
        _projections = JsonSerializer.Deserialize<List<PlayerStats>>(json) ?? [];
    }

    public PlayerStats? FindPlayerStats(string name)
    {
        return _projections.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}