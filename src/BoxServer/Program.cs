using Microsoft.Extensions.Configuration;
using System.Net.Sockets;
using System.Net;

var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddJsonFile("appsettings.json", optional: true);

var configuration = builder.Build();

var udpPort = configuration.GetValue("udpPort", 3501);

Console.WriteLine($"UDP Port: {udpPort}");

var boxes = new Dictionary<IPEndPoint, Box>();
var listener = new UdpClient(udpPort);
var remoteEndpoint = new IPEndPoint(IPAddress.Any, udpPort);

while (true)
{
    var receivedBytes = listener.Receive(ref remoteEndpoint);
    var remote = remoteEndpoint.ToString();
    var remoteipv4address = remoteEndpoint.Address.Address;
    var remotePort = remoteEndpoint.Port;

    Console.WriteLine($"{DateTime.Now.ToString("ss.fffff")}: {remote}");

    Box box;
    if (boxes.TryGetValue(remoteEndpoint, out box))
    {
        box.LastUpdated = DateTime.Now;
        box.X = BitConverter.ToUInt32(receivedBytes, 0);
        box.Y = BitConverter.ToUInt32(receivedBytes, 4);
    }
    else
    {
        box = new Box
        {
            Address = remoteEndpoint,
            Created = DateTime.Now,
            LastUpdated = DateTime.Now,
            X = BitConverter.ToUInt32(receivedBytes, 0),
            Y = BitConverter.ToUInt32(receivedBytes, 4)
        };
        boxes.Add(remoteEndpoint, box);
    }

    foreach (var b in boxes)
    {
        if (b.Key == remoteEndpoint) continue;

        await listener.SendAsync(receivedBytes, receivedBytes.Length, b.Value.Address).ConfigureAwait(false);
    }
}

class Box
{
    public IPEndPoint Address { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastUpdated { get; set; }

    public uint X { get; set; }
    public uint Y { get; set; }
}