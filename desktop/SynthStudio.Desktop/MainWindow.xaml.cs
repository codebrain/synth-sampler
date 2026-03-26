using System;
using System.Text.Json;
using System.Windows;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using SynthStudio.Desktop.Bridge;
using SynthStudio.Desktop.Services;

namespace SynthStudio.Desktop;

public partial class MainWindow : Window
{
    private readonly LocalWebServerService _webServer;
    private readonly BridgeMessageDispatcher _dispatcher;
    private readonly ILogger<MainWindow> _logger;

    public MainWindow(
        LocalWebServerService webServer,
        BridgeMessageDispatcher dispatcher,
        ILogger<MainWindow> logger)
    {
        InitializeComponent();
        _webServer = webServer;
        _dispatcher = dispatcher;
        _logger = logger;

        Loaded += OnLoaded;
        Closing += OnClosing;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await _webServer.StartAsync();

        await Browser.EnsureCoreWebView2Async();
        Browser.CoreWebView2.WebMessageReceived += async (_, args) =>
        {
            var raw = args.WebMessageAsJson;
            var response = await _dispatcher.DispatchAsync(raw);
            if (response is not null)
            {
                Browser.CoreWebView2.PostWebMessageAsJson(response);
            }
        };

        _dispatcher.HostEventProduced += (_, json) =>
        {
            Dispatcher.Invoke(() =>
            {
                Browser.CoreWebView2?.PostWebMessageAsJson(json);
            });
        };

        Browser.Source = new Uri(_webServer.BaseAddress);
        _logger.LogInformation("WebView2 navigated to {Address}", _webServer.BaseAddress);
    }

    private async void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        try
        {
            await _webServer.StopAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error shutting down local web server.");
        }
    }
}
