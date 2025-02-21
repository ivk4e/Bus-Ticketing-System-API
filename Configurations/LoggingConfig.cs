using Serilog;
using Serilog.Debugging;

namespace BusifyAPI.Configurations
{
    public static class LoggingConfig
    {
        public static void ConfigureLogging(WebApplicationBuilder builder)
        {
            var config = builder.Configuration.GetSection("Logging:BetterStack");
            var sourceToken = config["SourceToken"];
            var betterStackEndpoint = config["Endpoint"];

            SelfLog.Enable(Console.Out);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.BetterStack(
                    sourceToken: sourceToken,
                    betterStackEndpoint: betterStackEndpoint
                )
                .MinimumLevel.Information()
                .CreateLogger();

            builder.Host.UseSerilog();
        }
    }
}
