using Microsoft.Extensions.Configuration;
using System.Net.Sockets;
using System.Net;

var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddJsonFile("appsettings.json", optional: true);

var configuration = builder.Build();

var tcpPort = configuration.GetValue("tcpPort", 3500);
var udpPort = configuration.GetValue("udpPort", 3501);

Console.WriteLine($"TCP Port: {tcpPort}");
Console.WriteLine($"UDP Port: {udpPort}");

if (udpPort != 0)
{
    new Thread(async () =>
    {
        var listener = new UdpClient(udpPort);
        var remoteEndpoint = new IPEndPoint(IPAddress.Any, udpPort);

        while (true)
        {
            var receivedBytes = listener.Receive(ref remoteEndpoint);
            var sendBytes = new byte[4] { 0xFF, 0xFF, 0xFF, 0xFF };
            await listener.SendAsync(sendBytes, sendBytes.Length, remoteEndpoint);
        }
    }).Start();
}
