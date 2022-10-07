namespace MiniClient;

public class ServerPingResult
{
    public string Region { get; set; } = string.Empty;
    public int AvarageLatency { get; set; }
    public List<int> Latencies { get; set; } = new();
}
