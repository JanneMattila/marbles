using Microsoft.Extensions.Configuration;
using System.Net.Sockets;
using System.Net;

var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddJsonFile("appsettings.json", optional: true);

var configuration = builder.Build();

var server = configuration.GetValue("server", "127.0.0.1");
var tcpPort = configuration.GetValue("tcpPort", 3500);
var udpPort = configuration.GetValue("udpPort", 3501);

Console.WriteLine($"TCP Port: {tcpPort}");
Console.WriteLine($"UDP Port: {udpPort}");

var data = new byte[4] { 0xFF, 0xFF, 0xFF, 0xFF };
var buffer = new byte[1024];
var count = 100;

if (udpPort != 0)
{
    var totalTime = new List<TimeSpan>();
    using var client = new UdpClient(server, udpPort);
    for (int i = 0; i < count; i++)
    {
        var start = DateTime.UtcNow.Ticks;
        await client.SendAsync(data, data.Length);
        var udpReceiveResult = await client.ReceiveAsync();
        long end = DateTime.UtcNow.Ticks;
        var time = TimeSpan.FromTicks(end - start);
        totalTime.Add(time);
        Console.WriteLine($"UDP Ping: {time.TotalMilliseconds} ms");
    }

    totalTime.Sort((a, b) => a.CompareTo(b));
    Console.WriteLine($"UDP Median: {totalTime[totalTime.Count / 2].TotalMilliseconds} ms");
    Console.WriteLine($"UDP Average: {totalTime.Average(o => o.TotalMilliseconds)} ms");
}

if (tcpPort != 0)
{
    var totalTime = new List<TimeSpan>();
    using var client = new TcpClient(server, tcpPort);
    using var stream = client.GetStream();
    for (int i = 0; i < count; i++)
    {
        var start = DateTime.UtcNow.Ticks;
        await stream.WriteAsync(data, 0, data.Length);
        var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
        long end = DateTime.UtcNow.Ticks;
        var time = TimeSpan.FromTicks(end - start);
        totalTime.Add(time);
        Console.WriteLine($"TCP Ping: {time.TotalMilliseconds} ms");
    }

    totalTime.Sort((a, b) => a.CompareTo(b));
    Console.WriteLine($"TCP Median: {totalTime[totalTime.Count / 2].TotalMilliseconds} ms");
    Console.WriteLine($"TCP Average: {totalTime.Average(o => o.TotalMilliseconds)} ms");
}