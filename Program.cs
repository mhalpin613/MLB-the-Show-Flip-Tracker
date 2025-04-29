using FlipTracker.CLI;
using FlipTracker.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FlipTracker;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddHttpClient();
                services.AddSingleton<ShowDdClient>();
                services.AddSingleton<ProjectionReader>();
            })
            .Build();

        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        var showDdClient = services.GetRequiredService<ShowDdClient>();
        var projectionReader = services.GetRequiredService<ProjectionReader>();

        await Menu.ShowMainMenu(showDdClient, projectionReader);
    }
}