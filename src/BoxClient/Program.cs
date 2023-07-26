using Microsoft.Extensions.Configuration;

namespace BoxClient;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true);

        var configuration = builder.Build();

        var server = configuration.GetValue("server", "127.0.0.1");
        var port = configuration.GetValue("port", 3501);

        ArgumentNullException.ThrowIfNull(server);
        Application.Run(new MainForm(server, port));
    }
}
