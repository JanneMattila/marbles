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

if (udpPort != 0)
{
    using var client = new UdpClient(server, udpPort);
    while (true)
    {
        var start = DateTime.UtcNow.Ticks;
        var data = new byte[4] { 0xFF, 0xFF, 0xFF, 0xFF };
        await client.SendAsync(data, data.Length);
        var udpReceiveResult = await client.ReceiveAsync();
        long end = DateTime.UtcNow.Ticks;
        var time = TimeSpan.FromTicks(end - start);
        Console.WriteLine($"UDP Ping: {time.TotalMilliseconds} ms");
    }
}
