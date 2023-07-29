using Microsoft.Extensions.Configuration;
using System.Net.Sockets;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;
using System.Diagnostics;
using System;

var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddJsonFile("appsettings.json", optional: true);

var configuration = builder.Build();

var udpPort = configuration.GetValue("udpPort", 3501);

Console.WriteLine($"UDP Port: {udpPort}");

var boxes = new Dictionary<IPEndPoint, Box>();
var listener = new UdpClient(udpPort);
var remoteEndpoint = new IPEndPoint(IPAddress.Any, udpPort);

var stopwatch = new Stopwatch();
stopwatch.Start();

while (true)
{
    byte[] receivedBytes;

    try
    {
        receivedBytes = listener.Receive(ref remoteEndpoint);
    }
    catch (SocketException ex)
    {
        Debug.WriteLine("SocketException caught: {0}", ex.Message);
        continue;
    }

    var now = DateTime.UtcNow;

    Box box;
    if (!boxes.TryGetValue(remoteEndpoint, out box))
    {
        box = new Box
        {
            ID = (byte)(boxes.Count + 1),
            Address = remoteEndpoint,
            Created = now
        };
        boxes.Add(remoteEndpoint, box);
    }

    box.LastUpdated = now;

    if (receivedBytes.Length != 8)
    {
        throw new ApplicationException($"Received {receivedBytes.Length} bytes, expected 8.");
    }

    var x = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(receivedBytes, 0)) / 1_000f;
    var y = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(receivedBytes, 4)) / 1_000f;

    // TODO: Add received sequence number to received items list.

    box.X = x;
    box.Y = y;

    foreach (var b in boxes)
    {
        if (b.Value.ID == box.ID) continue;
        listener.Send(receivedBytes, receivedBytes.Length, b.Key);
    }

    box.Messages++;

    if (stopwatch.ElapsedMilliseconds >= 1_000)
    {
        var lastUpdateThreshold = now.AddSeconds(-5);

        Console.WriteLine($"{boxes.Count} clients:");
        var toRemove = new List<IPEndPoint>();
        foreach (var b in boxes)
        {
            Console.WriteLine($"{b.Key}: {b.Value.Messages} packets, X: {b.Value.X}, Y: {b.Value.Y}");
            if (b.Value.LastUpdated < lastUpdateThreshold)
            {
                toRemove.Add(b.Key);
            }

            b.Value.Messages = 0;
        }

        foreach (var r in toRemove)
        {
            boxes.Remove(r);
        }
        stopwatch.Restart();
    }
}

class Box
{
    public byte ID { get; set; }
    public IPEndPoint Address { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastUpdated { get; set; }
    public int Messages { get; set; }

    public float X { get; set; }
    public float Y { get; set; }
}