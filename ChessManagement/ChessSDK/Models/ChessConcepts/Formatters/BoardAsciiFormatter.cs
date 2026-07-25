namespace ChessSDK.Models.ChessConcepts.Formatters;

using System.Text;
using ChessSDK.Enums;
using ChessSDK.Models.Boards;

/// <summary>
/// Draws a position as a boxed grid. It is presentation of a domain concept, so it lives in the
/// SDK: a MAUI app or a web front end would need exactly the same drawing.
/// </summary>
public sealed class BoardAsciiFormatter
{
	private const int CellWidth = 3;

	private static readonly string topBorder = BuildBorder('\u250C', '\u252C', '\u2510');
	private static readonly string middleBorder = BuildBorder('\u251C', '\u253C', '\u2524');
	private static readonly string bottomBorder = BuildBorder('\u2514', '\u2534', '\u2518');

	private readonly PieceLetterFormatter letterFormatter = new();

	/// <summary>Board seen from white, with spanish letters.</summary>
	public string Format(PositionModel position)
		=> Format(position, PieceLetterStyleEnum.Spanish, GameColorModel.White);

	public string Format(PositionModel position, PieceLetterStyleEnum style, GameColorModel perspective)
	{
		ArgumentNullException.ThrowIfNull(position);
		ArgumentNullException.ThrowIfNull(perspective);

		var files = FileOrder(perspective);
		var ranks = RankOrder(perspective);
		var header = BuildHeader(files);

		var builder = new StringBuilder();

		builder.AppendLine(header);
		builder.AppendLine(topBorder);

		for (var row = 0; row < ranks.Length; row++)
		{
			AppendRank(builder, position, style, files, ranks[row]);

			builder.AppendLine(row == ranks.Length - 1 ? bottomBorder : middleBorder);
		}

		builder.Append(header);

		return builder.ToString();
	}

	private static string BuildBorder(char left, char middle, char right)
	{
		var cell = new string('\u2500', CellWidth);

		return $"  {left}{string.Join(middle.ToString(), Enumerable.Repeat(cell, 8))}{right}";
	}

	/// <summary>Files left to right, so that the letters sit right above their column.</summary>
	private static string BuildHeader(IReadOnlyList<FileModel> files)
	{
		var builder = new StringBuilder("  ");

		foreach (var file in files)
			builder.Append("  ").Append(file.Name).Append(' ');

		return builder.ToString().TrimEnd();
	}

	/// <summary>From black's side the board is turned around, so h is on the left.</summary>
	private static FileModel[] FileOrder(GameColorModel perspective)
	{
		var files = BoardModel.AllFiles.ToArray();

		if (!perspective.IsWhite)
			Array.Reverse(files);

		return files;
	}

	/// <summary>Top row first: rank 8 for white, rank 1 for black.</summary>
	private static RankModel[] RankOrder(GameColorModel perspective)
	{
		var ranks = BoardModel.AllRanks.Reverse().ToArray();

		if (!perspective.IsWhite)
			Array.Reverse(ranks);

		return ranks;
	}

	private void AppendRank(StringBuilder builder, PositionModel position, PieceLetterStyleEnum style,
							IReadOnlyList<FileModel> files, RankModel rank)
	{
		builder.Append(rank.Name).Append(" \u2502");

		foreach (var file in files)
		{
			var placed = position.PieceAt(rank.Index * 8 + file.Index);

			builder.Append(' ')
				   .Append(placed is null ? ' ' : letterFormatter.Format(placed, style))
				   .Append(" \u2502");
		}

		builder.Append(' ').Append(rank.Name).AppendLine();
	}
}
