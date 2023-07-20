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

if (udpPort != 0)
{
    using var client = new UdpClient(server, udpPort);
    for (int i = 0; i < 100; i++)
    {
        var start = DateTime.UtcNow.Ticks;
        await client.SendAsync(data, data.Length);
        var udpReceiveResult = await client.ReceiveAsync();
        long end = DateTime.UtcNow.Ticks;
        var time = TimeSpan.FromTicks(end - start);
        Console.WriteLine($"UDP Ping: {time.TotalMilliseconds} ms");
    }
}

if (tcpPort != 0)
{
    using var client = new TcpClient(server, tcpPort);
    using var stream = client.GetStream();
    for (int i = 0; i < 100; i++)
    {
        var start = DateTime.UtcNow.Ticks;
        await stream.WriteAsync(data, 0, data.Length);
        var count = await stream.ReadAsync(buffer, 0, buffer.Length);
        long end = DateTime.UtcNow.Ticks;
        var time = TimeSpan.FromTicks(end - start);
        Console.WriteLine($"TCP Ping: {time.TotalMilliseconds} ms");
    }
}