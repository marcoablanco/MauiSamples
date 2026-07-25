namespace ChessSDK.Models.ChessConcepts.Formatters;

using System.Text;
using ChessSDK.Models.Boards;

/// <summary>
/// Formats a sequence of moves as a numbered move list: "1. e4 e5 2. Nf3 Nc6".
/// </summary>
public sealed class MoveHistoryFormatter
{
	private readonly IMoveNotationFormatter formatter;

	public MoveHistoryFormatter(IMoveNotationFormatter formatter)
	{
		ArgumentNullException.ThrowIfNull(formatter);

		this.formatter = formatter;
	}

	public string Format(IReadOnlyList<MoveModel> moves)
	{
		ArgumentNullException.ThrowIfNull(moves);

		return FormatCore(moves, null);
	}

	/// <summary>
	/// Formats the move list replaying it from a starting position, so every move gets its
	/// disambiguation and its check or checkmate mark.
	/// </summary>
	public string Format(IReadOnlyList<MoveModel> moves, PositionModel startingPosition)
	{
		ArgumentNullException.ThrowIfNull(moves);
		ArgumentNullException.ThrowIfNull(startingPosition);

		return FormatCore(moves, startingPosition);
	}

	private string FormatCore(IReadOnlyList<MoveModel> moves, PositionModel? startingPosition)
	{
		if (moves.Count == 0)
			return string.Empty;

		var builder = new StringBuilder();
		var position = startingPosition;
		var moveNumber = position?.FullMoveNumber ?? 1;
		var blackStarts = position is not null && !position.SideToMove.IsWhite;

		for (var i = 0; i < moves.Count; i++)
		{
			var isWhiteTurn = blackStarts ? i % 2 == 1 : i % 2 == 0;

			if (isWhiteTurn)
				builder.Append(moveNumber).Append(". ");
			else if (i == 0)
				builder.Append(moveNumber).Append("... ");

			builder.Append(position is null ? formatter.Format(moves[i]) : formatter.Format(moves[i], position)).Append(' ');

			if (!isWhiteTurn)
				moveNumber++;

			position = position?.Apply(moves[i]);
		}

		return builder.ToString().TrimEnd();
	}
}
