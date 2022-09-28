using Grpc.Net.Client;
using Mini;

Console.WriteLine("Mini Client");

var server = "https://localhost:7201";

try
{
    var httpClient = new HttpClient();
    var channel = GrpcChannel.ForAddress(server,
        new GrpcChannelOptions
        {
            HttpClient = httpClient
        });
    var client = new Greeter.GreeterClient(channel);
    var reply = await client.SayHelloAsync(
        new HelloRequest
        {
            Name = "GreeterClient"
        });

    Console.WriteLine("Greeting: " + reply.Message);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}