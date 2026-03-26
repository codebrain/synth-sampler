using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SynthStudio.Desktop.Bridge;
using SynthStudio.Desktop.Services;

namespace SynthStudio.Desktop;

public static class Program
{
    [STAThread]
    public static async Task Main()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .ConfigureServices(services =>
            {
                services.AddSingleton<LocalWebServerService>();
                services.AddSingleton<BridgeMessageDispatcher>();
                services.AddSingleton<ProfileService>();
                services.AddSingleton<IMidiService, StubMidiService>();
                services.AddSingleton<IAudioCaptureService, StubAudioCaptureService>();
                services.AddSingleton<IStorageService, FileStorageService>();
                services.AddSingleton<AutomationJobService>();
                services.AddSingleton<MainWindow>();
            })
            .Build();

        await host.StartAsync();

        var app = new App();
        app.Exit += async (_, _) => await host.StopAsync();

        var window = host.Services.GetRequiredService<MainWindow>();
        app.Run(window);

        host.Dispose();
    }
}
