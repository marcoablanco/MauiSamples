namespace ChessSDK.Notation;

using System.Text;
using ChessSDK.Enums;
using ChessSDK.Models.Boards;
using ChessSDK.Models.ChessConcepts;
using ChessSDK.Models.ChessConcepts.Formatters;

/// <summary>
/// Exports a game in Portable Game Notation: the seven mandatory tags followed by the move text
/// in english SAN, wrapped so no line goes over 80 characters.
/// </summary>
public sealed class PgnFormatter
{
	private const int MaxLineLength = 80;

	private readonly IMoveNotationFormatter formatter;

	public PgnFormatter()
		: this(new EnglishSanFormatter())
	{
	}

	public PgnFormatter(IMoveNotationFormatter formatter)
	{
		ArgumentNullException.ThrowIfNull(formatter);

		this.formatter = formatter;
	}

	/// <summary>Result tag of a finished or unfinished game, seen from the given final position.</summary>
	public static string FormatResult(GameResultEnum result, GameColorModel sideToMove)
	{
		ArgumentNullException.ThrowIfNull(sideToMove);

		return result switch
			   {
				   // The side that has to move is the one that has been mated, or the one that resigned.
				   GameResultEnum.Checkmate or GameResultEnum.Resigned => sideToMove.IsWhite ? "0-1" : "1-0",
				   GameResultEnum.Stalemate
					   or GameResultEnum.InsufficientMaterial
					   or GameResultEnum.ThreefoldRepetition
					   or GameResultEnum.FiftyMoveRule => "1/2-1/2",
				   _ => "*"
			   };
	}

	public string Format(GameSessionModel session, PgnHeadersModel? headers = null)
	{
		ArgumentNullException.ThrowIfNull(session);

		return Format(session.History, session.StartingPosition, session.Result, headers);
	}

	public string Format(
		IReadOnlyList<MoveModel> moves,
		PositionModel startingPosition,
		GameResultEnum result,
		PgnHeadersModel? headers = null)
	{
		ArgumentNullException.ThrowIfNull(moves);
		ArgumentNullException.ThrowIfNull(startingPosition);

		var tags = headers ?? new PgnHeadersModel();
		var position = startingPosition;
		var tokens = new List<string>(moves.Count + moves.Count / 2 + 1);
		var moveNumber = startingPosition.FullMoveNumber;
		var blackStarts = !startingPosition.SideToMove.IsWhite;

		for (var i = 0; i < moves.Count; i++)
		{
			var isWhiteTurn = blackStarts ? i % 2 == 1 : i % 2 == 0;

			if (isWhiteTurn)
				tokens.Add($"{moveNumber}.");
			else if (i == 0)
				tokens.Add($"{moveNumber}...");

			tokens.Add(formatter.Format(moves[i], position));

			if (!isWhiteTurn)
				moveNumber++;

			position = position.Apply(moves[i]);
		}

		var resultTag = FormatResult(result, position.SideToMove);
		tokens.Add(resultTag);

		var builder = new StringBuilder();

		builder.AppendLine($"[Event \"{tags.Event}\"]");
		builder.AppendLine($"[Site \"{tags.Site}\"]");
		builder.AppendLine($"[Date \"{tags.Date}\"]");
		builder.AppendLine($"[Round \"{tags.Round}\"]");
		builder.AppendLine($"[White \"{tags.White}\"]");
		builder.AppendLine($"[Black \"{tags.Black}\"]");
		builder.AppendLine($"[Result \"{resultTag}\"]");

		if (!startingPosition.Equals(PositionModel.StartingPosition))
		{
			builder.AppendLine("[SetUp \"1\"]");
			builder.AppendLine($"[FEN \"{new FenSerializer().Serialize(startingPosition)}\"]");
		}

		builder.AppendLine();
		builder.Append(Wrap(tokens));

		return builder.ToString();
	}

	private static string Wrap(IReadOnlyList<string> tokens)
	{
		var builder = new StringBuilder();
		var lineLength = 0;

		foreach (var token in tokens)
		{
			if (lineLength > 0 && lineLength + 1 + token.Length > MaxLineLength)
			{
				builder.AppendLine();
				lineLength = 0;
			}
			else if (lineLength > 0)
			{
				builder.Append(' ');
				lineLength++;
			}

			builder.Append(token);
			lineLength += token.Length;
		}

		return builder.ToString();
	}
}
