namespace ChessSDK.Mcp.Tools;

using System.ComponentModel;
using ChessSDK.Models.ChessConcepts;
using ChessSDK.Models.ChessConcepts.Formatters;
using ChessSDK.Services;
using ModelContextProtocol.Server;

[McpServerToolType]
public sealed class ChessGameTools
{
	private readonly IGameStoreService store;

	public ChessGameTools(IGameStoreService store)
	{
		this.store = store;
	}

	private static string NotFound(string gameId)
		=> $"ERROR: no existe la partida '{gameId}'. Usa list_games o crea una con new_game.";

	[McpServerTool(Name = "new_game")]
	[Description("Crea una partida nueva desde la posicion inicial y devuelve su identificador, el FEN y el tablero.")]
	public string NewGame(
		[Description("Color que llevara el usuario: 'white' o 'black'. Por defecto 'white'.")]
		string humanColor = "white")
	{
		GameColorModel color;

		try
		{
			color = humanColor;
		}
		catch (ArgumentException)
		{
			return "ERROR: el color debe ser 'white' o 'black'.";
		}

		var session = store.Create(color);

		return $"""
				Partida creada.
				gameId: {session.Id}
				Usuario: {session.HumanColor} | Mueven: {session.SideToMove}
				FEN: {session.ToFen()}

				{session.ToAscii()}
				""";
	}

	[McpServerTool(Name = "get_position", ReadOnly = true)]
	[Description("Devuelve el FEN, el turno y el tablero ASCII de una partida.")]
	public string GetPosition([Description("Identificador devuelto por new_game.")] string gameId)
	{
		var session = store.Find(gameId);

		if (session is null)
			return NotFound(gameId);

		return $"""
				gameId: {session.Id} | Mueven: {session.SideToMove} | Jugada {session.FullMoveNumber}
				FEN: {session.ToFen()}

				{session.ToAscii()}
				""";
	}

	[McpServerTool(Name = "make_move")]
	[Description("Aplica un movimiento en notacion larga (por ejemplo 'e2e4' o 'e7e8q') y devuelve la posicion resultante.")]
	public string MakeMove(
		[Description("Identificador de la partida.")] string gameId,
		[Description("Movimiento en notacion larga: casilla de origen + casilla de destino + pieza de promocion opcional.")]
		string move)
	{
		var session = store.Find(gameId);

		if (session is null)
			return NotFound(gameId);

		if (!session.TryApplyMove(move, out var error))
			return $"ERROR: {error}";

		return $"""
				Movimiento aplicado: {session.History[^1]}
				Mueven ahora: {session.SideToMove}
				FEN: {session.ToFen()}

				{session.ToAscii()}
				""";
	}

	[McpServerTool(Name = "get_history", ReadOnly = true)]
	[Description("Devuelve la lista de movimientos jugados en la notacion indicada.")]
	public string GetHistory(
		[Description("Identificador de la partida.")] string gameId,
		[Description("Notacion: 'san-en', 'san-es', 'figurine' o 'lan'. Por defecto 'san-en'.")]
		string notation = "san-en")
	{
		var session = store.Find(gameId);

		if (session is null)
			return NotFound(gameId);

		if (session.History.Count == 0)
			return "Todavia no se ha jugado ningun movimiento.";

		var formatter = new MoveHistoryFormatter(MoveNotationFormatterFactory.Create(notation));

		return formatter.Format(session.History);
	}

	[McpServerTool(Name = "list_games", ReadOnly = true)]
	[Description("Lista las partidas activas del servidor.")]
	public string ListGames()
	{
		var games = store.All();

		if (games.Count == 0)
			return "No hay partidas activas. Usa new_game para empezar una.";

		return string.Join(
			Environment.NewLine,
			games.Select(g => $"{g.Id} | usuario: {g.HumanColor} | mueven: {g.SideToMove} | jugadas: {g.History.Count}"));
	}

	[McpServerTool(Name = "resign_game", Destructive = true)]
	[Description("Abandona y elimina una partida activa.")]
	public string ResignGame([Description("Identificador de la partida.")] string gameId)
		=> store.Remove(gameId)
			   ? $"Partida {gameId} finalizada y eliminada."
			   : NotFound(gameId);
}

