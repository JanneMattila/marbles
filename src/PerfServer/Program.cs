using Microsoft.Extensions.Configuration;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

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
            await listener.SendAsync(sendBytes, sendBytes.Length, remoteEndpoint).ConfigureAwait(false);
        }
    }).Start();
}


if (tcpPort != 0)
{
    new Thread(async () =>
    {
        var buffer = new byte[1024];
        var listener = new TcpListener(IPAddress.Any, tcpPort);
        listener.Start();

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
            if (client == null) continue;

            new Thread(async () =>
            {
                try
                {
                    using var stream = client.GetStream();
                    while (true)
                    {
                        var count = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                        await stream.WriteAsync(buffer, 0, count).ConfigureAwait(false);
                        await stream.FlushAsync().ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }).Start();
        }
    }).Start();
}
