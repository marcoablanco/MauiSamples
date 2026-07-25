using System.Text;
using ChessSDK.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// El tablero usa caracteres de recuadro Unicode. Sin esto, la consola de Windows los destroza.
// UTF8Encoding(false) para no emitir BOM, que corromperia el flujo JSON-RPC.
Console.OutputEncoding = new UTF8Encoding(false);

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
