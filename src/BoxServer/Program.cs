using Microsoft.Extensions.Configuration;
using System.Net.Sockets;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;

var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddJsonFile("appsettings.json", optional: true);

var configuration = builder.Build();

var udpPort = configuration.GetValue("udpPort", 3501);

Console.WriteLine($"UDP Port: {udpPort}");

var boxes = new Dictionary<IPEndPoint, Box>();
var listener = new UdpClient(udpPort);
var remoteEndpoint = new IPEndPoint(IPAddress.Any, udpPort);

var messages = 0;
var data = new byte[8];

while (true)
{
    var result = await listener.ReceiveAsync().ConfigureAwait(false);
    var now = DateTime.Now;
    Console.WriteLine($"{DateTime.Now.ToString("ss.fffff")}: {result.RemoteEndPoint}");

    Box box;
    if (!boxes.TryGetValue(result.RemoteEndPoint, out box))
    {
        box = new Box
        {
            Address = result.RemoteEndPoint,
            Created = now
        };
        boxes.Add(result.RemoteEndPoint, box);
    }

    box.LastUpdated = now;

    for (var index = 0; index < result.Buffer.Length; index += 8)
    {
        Buffer.BlockCopy(result.Buffer, index, data, 0, 8);
        var x = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, index)) / 1_000f;
        var y = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, index + 4)) / 1_000f;

        // TODO: Add received sequence number to received items list.

        box.X = x;
        box.Y = y;
    }

    foreach (var b in boxes)
    {
        //if (remoteEndpoint.Equals(b.Key)) continue;

        await listener.SendAsync(data, data.Length, b.Key).ConfigureAwait(false);
    }

    messages++;

    if (messages % 1_000 == 0)
    {
        var toRemove = new List<IPEndPoint>();
        foreach (var b in boxes)
        {
            if (now - b.Value.LastUpdated > TimeSpan.FromSeconds(5))
            {
                toRemove.Add(b.Key);
            }
        }

        foreach (var r in toRemove)
        {
            boxes.Remove(r);
        }
    }
}

class Box
{
    public IPEndPoint Address { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastUpdated { get; set; }

    public float X { get; set; }
    public float Y { get; set; }
}