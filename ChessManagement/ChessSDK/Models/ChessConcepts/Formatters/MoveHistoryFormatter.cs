namespace ChessSDK.Models.ChessConcepts.Formatters;

using System.Text;

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

		if (moves.Count == 0)
			return string.Empty;

		var builder = new StringBuilder();

		for (var i = 0; i < moves.Count; i++)
		{
			if (i % 2 == 0)
				builder.Append(i / 2 + 1).Append(". ");

			builder.Append(formatter.Format(moves[i])).Append(' ');
		}

		return builder.ToString().TrimEnd();
	}
}

