using Microsoft.Extensions.Configuration;
using System.Net.Sockets;
using System.Net;

var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddJsonFile("appsettings.json", optional: true);

var configuration = builder.Build();

var udpPort = configuration.GetValue("udpPort", 3501);

Console.WriteLine($"UDP Port: {udpPort}");

var listener = new UdpClient(udpPort);
var remoteEndpoint = new IPEndPoint(IPAddress.Any, udpPort);

while (true)
{
    var receivedBytes = listener.Receive(ref remoteEndpoint);
    Console.Write("+");
}
