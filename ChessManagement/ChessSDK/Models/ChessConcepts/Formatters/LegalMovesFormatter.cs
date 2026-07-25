namespace ChessSDK.Models.ChessConcepts.Formatters;

using System.Text;
using ChessSDK.Models.Boards;

/// <summary>
/// Writes the legal moves of a position grouped by piece, in algebraic and long notation:
/// <c>caballos: Nc3 (b1c3), Na3 (b1a3)</c>.
/// Deciding what to say when the asked square is empty, holds an enemy piece or has no legal
/// moves is part of explaining the position, so it lives here and every front end reuses it.
/// </summary>
public sealed class LegalMovesFormatter
{
	private readonly IMoveNotationFormatter moveFormatter;
	private readonly PieceNameFormatter pieceNameFormatter;
	private readonly GameResultFormatter resultFormatter;

	public LegalMovesFormatter()
		: this(new EnglishSanFormatter())
	{
	}

	public LegalMovesFormatter(IMoveNotationFormatter moveFormatter)
		: this(moveFormatter, new PieceNameFormatter(), new GameResultFormatter())
	{
	}

	public LegalMovesFormatter(
		IMoveNotationFormatter moveFormatter,
		PieceNameFormatter pieceNameFormatter,
		GameResultFormatter resultFormatter)
	{
		ArgumentNullException.ThrowIfNull(moveFormatter);
		ArgumentNullException.ThrowIfNull(pieceNameFormatter);
		ArgumentNullException.ThrowIfNull(resultFormatter);

		this.moveFormatter = moveFormatter;
		this.pieceNameFormatter = pieceNameFormatter;
		this.resultFormatter = resultFormatter;
	}

	/// <summary>Every legal move of the side to move.</summary>
	public string Format(GameSessionModel session)
	{
		ArgumentNullException.ThrowIfNull(session);

		if (session.IsOver)
			return $"La partida ha terminado: {resultFormatter.Format(session)}. No hay movimientos legales.";

		return Write(session, session.LegalMoves());
	}

	/// <summary>
	/// Legal moves of the piece standing on <paramref name="from" />, or an explanation of why
	/// there are none.
	/// </summary>
	public string Format(GameSessionModel session, string? from)
	{
		ArgumentNullException.ThrowIfNull(session);

		if (string.IsNullOrWhiteSpace(from))
			return Format(session);

		if (session.IsOver)
			return $"La partida ha terminado: {resultFormatter.Format(session)}. No hay movimientos legales.";

		if (!CoordinateModel.TryParse(from, out var origin))
			return $"La casilla '{from.Trim()}' no existe. Usa una casilla del tablero, por ejemplo 'g1'.";

		var placed = session.PieceAt(origin);

		if (placed is null)
			return $"No hay ninguna pieza en '{origin}'.";

		if (placed.Color != session.SideToMove)
			return $"La pieza de '{origin}' es de {placed.Color} y mueven {session.SideToMove}.";

		var moves = session.LegalMovesFrom(origin);

		if (moves.Count == 0)
			return $"{pieceNameFormatter.FormatWithArticle(placed.Piece)} de '{origin}' no tiene ningun movimiento legal.";

		return Write(session, moves);
	}

	private string Write(GameSessionModel session, IReadOnlyList<MoveModel> moves)
	{
		var builder = new StringBuilder();

		builder.AppendLine($"Movimientos legales de {session.SideToMove} ({moves.Count}):");

		foreach (var group in moves.GroupBy(move => move.Piece).OrderBy(group => DisplayOrder(group.Key)))
		{
			var written = group.Select(move => $"{moveFormatter.Format(move, session.Position)} ({move.ToLongAlgebraic()})");

			builder.AppendLine($"{pieceNameFormatter.FormatPlural(group.Key)}: {string.Join(", ", written)}");
		}

		return builder.ToString().TrimEnd();
	}

	/// <summary>
	/// Most important piece first. Material value is not usable here: the king is worth 0 by
	/// convention, which would bury it under the pawns.
	/// </summary>
	private static int DisplayOrder(PieceModel piece)
		=> piece.Symbol switch
		   {
			   'K' => 0,
			   'Q' => 1,
			   'R' => 2,
			   'B' => 3,
			   'N' => 4,
			   _ => 5
		   };
}
