namespace ChessSDK.Models.ChessConcepts.Formatters;

/// <summary>
/// One-line summary of how a game stands: outcome, check and how many moves are available.
/// Any front end wants to show this, so it belongs here and not in a particular adapter.
/// </summary>
public sealed class GameStatusFormatter
{
	private readonly GameResultFormatter resultFormatter;

	public GameStatusFormatter()
		: this(new GameResultFormatter())
	{
	}

	public GameStatusFormatter(GameResultFormatter resultFormatter)
	{
		ArgumentNullException.ThrowIfNull(resultFormatter);

		this.resultFormatter = resultFormatter;
	}

	public string Format(GameSessionModel session)
	{
		ArgumentNullException.ThrowIfNull(session);

		var state = $"Estado: {resultFormatter.Format(session)}";

		// Check and move count say nothing once the game is over.
		if (session.IsOver)
			return state;

		return $"{state} | Jaque: {(session.IsInCheck ? "si" : "no")} | Movimientos legales: {session.LegalMoves().Count}";
	}
}
