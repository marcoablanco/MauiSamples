using ChessSDK.Mcp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// El canal MCP viaja por stdout: todos los logs deben ir a stderr.
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Services.AddSingleton<IGameStoreService, InMemoryGameStoreService>();

builder.Services
	   .AddMcpServer(options => options.ServerInfo = new() { Name = "chess-mcp", Version = "0.1.0" })
	   .WithStdioServerTransport()
	   .WithToolsFromAssembly()
	   .WithResourcesFromAssembly()
	   .WithPromptsFromAssembly();

await builder.Build().RunAsync();
