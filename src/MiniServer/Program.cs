using MiniServer.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

var gamePort = builder.Configuration.GetValue("gameport", 5050);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(gamePort, configure =>
    {
        configure.Protocols = HttpProtocols.Http2;
        configure.UseHttps();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "No web here. Go gRPC.");

app.Run();
