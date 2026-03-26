using System.Net;
using Microsoft.Extensions.Logging;

namespace SynthStudio.Desktop.Services;

public sealed class LocalWebServerService
{
    private readonly ILogger<LocalWebServerService> _logger;
    private HttpListener? _listener;
    private CancellationTokenSource? _cts;
    private Task? _loopTask;

    public string BaseAddress { get; private set; } = string.Empty;

    public LocalWebServerService(ILogger<LocalWebServerService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync()
    {
        if (_listener is not null)
        {
            return Task.CompletedTask;
        }

        var port = GetFreePort();
        BaseAddress = $"http://127.0.0.1:{port}/";

        _listener = new HttpListener();
        _listener.Prefixes.Add(BaseAddress);
        _listener.Start();

        _cts = new CancellationTokenSource();
        _loopTask = Task.Run(() => ListenLoopAsync(_cts.Token));

        _logger.LogInformation("Local web server listening on {BaseAddress}", BaseAddress);
        return Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        if (_listener is null)
        {
            return;
        }

        _cts?.Cancel();
        _listener.Stop();
        _listener.Close();

        if (_loopTask is not null)
        {
            await _loopTask;
        }

        _listener = null;
        _cts?.Dispose();
        _cts = null;
        _loopTask = null;
    }

    private async Task ListenLoopAsync(CancellationToken cancellationToken)
    {
        var root = ResolveFrontendRoot();

        while (!cancellationToken.IsCancellationRequested && _listener is not null && _listener.IsListening)
        {
            HttpListenerContext? context = null;
            try
            {
                context = await _listener.GetContextAsync();
                await HandleRequestAsync(context, root, cancellationToken);
            }
            catch (HttpListenerException)
            {
                break;
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Local web server error.");
                if (context is not null)
                {
                    context.Response.StatusCode = 500;
                    context.Response.Close();
                }
            }
        }
    }

    private static async Task HandleRequestAsync(HttpListenerContext context, string root, CancellationToken cancellationToken)
    {
        var relative = context.Request.Url?.AbsolutePath.TrimStart('/') ?? string.Empty;
        if (string.IsNullOrWhiteSpace(relative))
        {
            relative = "index.html";
        }

        var requestedPath = Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(requestedPath))
        {
            requestedPath = Path.Combine(root, "index.html");
        }

        var bytes = await File.ReadAllBytesAsync(requestedPath, cancellationToken);
        context.Response.ContentType = GetContentType(requestedPath);
        context.Response.ContentLength64 = bytes.Length;
        await context.Response.OutputStream.WriteAsync(bytes, cancellationToken);
        context.Response.Close();
    }

    private static string ResolveFrontendRoot()
    {
        // Production expectation: copy built frontend into a sibling folder named "frontend-dist".
        // Fallback: use the included lightweight public folder.
        var dist = Path.Combine(AppContext.BaseDirectory, "frontend-dist");
        if (Directory.Exists(dist))
        {
            return dist;
        }

        return Path.Combine(AppContext.BaseDirectory, "frontend-public");
    }

    private static int GetFreePort()
    {
        var listener = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private static string GetContentType(string path)
    {
        return Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".html" => "text/html; charset=utf-8",
            ".js" => "application/javascript; charset=utf-8",
            ".css" => "text/css; charset=utf-8",
            ".json" => "application/json; charset=utf-8",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream"
        };
    }
}
