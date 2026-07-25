namespace ChessSDK.Mcp.Resources;

using System.ComponentModel;
using ChessSDK.Services;
using ModelContextProtocol.Server;

[McpServerResourceType]
public sealed class ChessResources
{
	private readonly IGameStoreService store;

	public ChessResources(IGameStoreService store)
	{
		this.store = store;
	}

	[McpServerResource(UriTemplate = "chess://game/{gameId}/fen", Name = "game_fen", MimeType = "text/plain")]
	[Description("FEN de la posicion actual de la partida.")]
	public string Fen(string gameId)
		=> store.Find(gameId)?.ToFen() ?? $"Partida '{gameId}' no encontrada.";

	[McpServerResource(UriTemplate = "chess://game/{gameId}/board", Name = "game_board", MimeType = "text/plain")]
	[Description("Tablero de la partida en formato ASCII.")]
	public string Board(string gameId)
		=> store.Find(gameId)?.ToAscii() ?? $"Partida '{gameId}' no encontrada.";
}

