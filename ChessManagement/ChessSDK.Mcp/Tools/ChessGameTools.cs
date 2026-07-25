namespace ChessSDK.Mcp.Tools;

using System.ComponentModel;
using ChessSDK.Models.ChessConcepts;
using ChessSDK.Models.ChessConcepts.Formatters;
using ChessSDK.Services;
using ModelContextProtocol.Server;

[McpServerToolType]
public sealed class ChessGameTools
{
	private static readonly GameResultFormatter resultFormatter = new();
	private static readonly GameStatusFormatter statusFormatter = new();
	private static readonly LegalMovesFormatter legalMovesFormatter = new();

	private readonly IGameStoreService store;

	public ChessGameTools(IGameStoreService store)
	{
		this.store = store;
	}

	private static string NotFound(string gameId)
		=> $"ERROR: no existe la partida '{gameId}'. Usa list_games o crea una con new_game.";

	private static string StatusLine(GameSessionModel session) => statusFormatter.Format(session);

	[McpServerTool(Name = "new_game")]
	[Description("Crea una partida nueva desde la posicion inicial y devuelve su identificador, el FEN y el tablero.")]
	public string NewGame(
		[Description("Color que llevara el usuario: 'white' o 'black'. Por defecto 'white'.")]
		string humanColor = "white")
	{
		if (!GameColorModel.TryParse(humanColor, out var color))
			return "ERROR: el color debe ser 'white' o 'black'.";

		var session = store.Create(color);

		return $"""
				Partida creada.
				gameId: {session.Id}
				Usuario: {session.HumanColor} | Mueven: {session.SideToMove}
				{StatusLine(session)}
				FEN: {session.ToFen()}

				{session.ToAscii()}
				""";
	}

	[McpServerTool(Name = "get_position", ReadOnly = true)]
	[Description("Devuelve el FEN, el turno, el estado (jaque, mate, tablas), el numero de movimientos legales y el tablero ASCII.")]
	public string GetPosition([Description("Identificador devuelto por new_game.")] string gameId)
	{
		var session = store.Find(gameId);

		if (session is null)
			return NotFound(gameId);

		return $"""
				gameId: {session.Id} | Mueven: {session.SideToMove} | Jugada {session.FullMoveNumber}
				{StatusLine(session)}
				FEN: {session.ToFen()}

				{session.ToAscii()}
				""";
	}

	[McpServerTool(Name = "make_move")]
	[Description("Aplica un movimiento y devuelve la posicion resultante. Acepta notacion larga ('e2e4', 'e7e8q') y algebraica ('Nf3', 'exd5', 'O-O', 'e8=Q').")]
	public string MakeMove(
		[Description("Identificador de la partida.")] string gameId,
		[Description("Movimiento en notacion larga (origen+destino+promocion opcional) o algebraica estandar.")]
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
				{StatusLine(session)}
				FEN: {session.ToFen()}

				{session.ToAscii()}
				""";
	}

	[McpServerTool(Name = "get_legal_moves", ReadOnly = true)]
	[Description("Lista TODOS los movimientos legales de la posicion actual, agrupados por pieza, en notacion algebraica y larga. Consultala antes de mover.")]
	public string GetLegalMoves(
		[Description("Identificador de la partida.")] string gameId,
		[Description("Casilla de origen opcional, por ejemplo 'g1', para ver solo los movimientos de esa pieza.")]
		string? from = null)
	{
		var session = store.Find(gameId);

		return session is null ? NotFound(gameId) : legalMovesFormatter.Format(session, from);
	}

	[McpServerTool(Name = "undo_move")]
	[Description("Deshace los ultimos movimientos jugados y devuelve la posicion resultante.")]
	public string UndoMove(
		[Description("Identificador de la partida.")] string gameId,
		[Description("Cuantos medios movimientos deshacer. Por defecto 1; usa 2 para deshacer tu jugada y la respuesta.")]
		int plies = 1)
	{
		var session = store.Find(gameId);

		if (session is null)
			return NotFound(gameId);

		if (plies < 1)
			return "ERROR: hay que deshacer al menos un movimiento.";

		if (session.ResignedBy is not null)
			return "ERROR: la partida se abandono y no se puede deshacer. Empieza otra con new_game.";

		if (session.History.Count == 0)
			return "No se ha jugado ningun movimiento todavia; no hay nada que deshacer.";

		var undone = session.Undo(plies);

		var notice = undone < plies
						 ? $"Solo se han podido deshacer {undone} de los {plies} movimientos pedidos."
						 : $"Movimientos deshechos: {undone}.";

		return $"""
				{notice}
				Mueven ahora: {session.SideToMove} | Jugada {session.FullMoveNumber}
				{StatusLine(session)}
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

		return formatter.Format(session.History, session.StartingPosition);
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
			games.Select(g => $"{g.Id} | usuario: {g.HumanColor} | mueven: {g.SideToMove} | jugadas: {g.History.Count} | {resultFormatter.Format(g)}"));
	}

	[McpServerTool(Name = "resign_game")]
	[Description("Abandona una partida. La partida se conserva con su historial y queda marcada como perdida por quien abandona.")]
	public string ResignGame(
		[Description("Identificador de la partida.")] string gameId,
		[Description("Color que abandona: 'white' o 'black'. Por defecto, el color del usuario.")]
		string? color = null)
	{
		var session = store.Find(gameId);

		if (session is null)
			return NotFound(gameId);

		var resigning = session.HumanColor;

		if (!string.IsNullOrWhiteSpace(color) && !GameColorModel.TryParse(color, out resigning))
			return "ERROR: el color debe ser 'white' o 'black'.";

		if (!session.TryResign(resigning, out var error))
			return $"ERROR: {error}";

		return $"""
				Abandona {resigning}.
				{StatusLine(session)}
				FEN: {session.ToFen()}
				""";
	}

	[McpServerTool(Name = "delete_game", Destructive = true)]
	[Description("Elimina una partida del servidor. El historial se pierde; para rendirse conservando la partida usa resign_game.")]
	public string DeleteGame([Description("Identificador de la partida.")] string gameId)
		=> store.Remove(gameId)
			   ? $"Partida {gameId} eliminada."
			   : NotFound(gameId);
}

