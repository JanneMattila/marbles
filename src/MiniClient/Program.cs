using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Mini;
using System.Security.Cryptography.X509Certificates;

Console.WriteLine("Mini Client");

var builder = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
#if DEBUG
    .AddUserSecrets<Program>()
#endif
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var configuration = builder.Build();

var server = configuration.GetValue<string>("server", "https://localhost:5001");
var caCertFilePath = configuration.GetValue<string>("ca_cert_path");

if (!File.Exists(caCertFilePath))
{
    Console.WriteLine($"CA certificate file not found: {caCertFilePath}");
    return;
}

try
{
    var httpClient = new HttpClient(new HttpClientHandler
    {
        CheckCertificateRevocationList = false,
        ServerCertificateCustomValidationCallback = (message, cert, chain, _) =>
        {
            ArgumentNullException.ThrowIfNull(chain);
            ArgumentNullException.ThrowIfNull(cert);
            chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
            chain.ChainPolicy.CustomTrustStore.Add(new X509Certificate2(caCertFilePath));
            return chain.Build(cert);
        }
    });
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