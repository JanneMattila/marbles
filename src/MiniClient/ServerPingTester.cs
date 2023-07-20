using System.Net.Sockets;

namespace MiniClient;

/// <summary>
/// Uses https://playfab.github.io/thundernetes/howtos/latencyserver.html
/// deployed using https://raw.githubusercontent.com/PlayFab/thundernetes/main/samples/latencyserver/latencyserver.yaml
/// </summary>
public class ServerPingTester
{
    public async Task<List<ServerPingResult>> ExecuteTests(List<string> servers)
    {
        var results = new List<ServerPingResult>();
        foreach (var server in servers)
        {
            var result = await ExecuteTestAsync(server);
            results.Add(result);
        }
        return results;
    }

    private async Task<ServerPingResult> ExecuteTestAsync(string server)
    {
        using var client = new UdpClient(server, 3075);
        var start = DateTime.UtcNow.Ticks;
        var data = new byte[4] { 0xFF, 0xFF, 0xFF, 0xFF };
        await client.SendAsync(data, data.Length);
        var udpReceiveResult = await client.ReceiveAsync();
        long end = DateTime.UtcNow.Ticks;
        return new ServerPingResult
        {
            Region = server,
            AvarageLatency = (int)((end - start) / TimeSpan.TicksPerMillisecond)
        };
    }
}
