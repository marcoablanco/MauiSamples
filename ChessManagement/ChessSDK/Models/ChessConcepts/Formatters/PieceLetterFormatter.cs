namespace ChessSDK.Models.ChessConcepts.Formatters;

using ChessSDK.Enums;
using ChessSDK.Models.Boards;

/// <summary>
/// Single character that stands for a piece when drawing a board.
/// Unlike the SAN dialects, the pawn does have a letter here and the color matters: letter
/// alphabets use uppercase for white and lowercase for black, while figurine has a distinct
/// symbol per color.
/// </summary>
public sealed class PieceLetterFormatter
{
	public const string SpanishKey = "es";
	public const string EnglishKey = "en";
	public const string FigurineKey = "figurine";

	private static readonly IReadOnlyDictionary<char, char> spanishLetters = new Dictionary<char, char>
	{
		['P'] = 'P',
		['N'] = 'C',
		['B'] = 'A',
		['R'] = 'T',
		['Q'] = 'D',
		['K'] = 'R'
	};

	private static readonly IReadOnlyDictionary<char, char> whiteFigurines = new Dictionary<char, char>
	{
		['P'] = '\u2659',
		['N'] = '\u2658',
		['B'] = '\u2657',
		['R'] = '\u2656',
		['Q'] = '\u2655',
		['K'] = '\u2654'
	};

	private static readonly IReadOnlyDictionary<char, char> blackFigurines = new Dictionary<char, char>
	{
		['P'] = '\u265F',
		['N'] = '\u265E',
		['B'] = '\u265D',
		['R'] = '\u265C',
		['Q'] = '\u265B',
		['K'] = '\u265A'
	};

	/// <summary>Every style key accepted by <see cref="TryParseStyle" />, for error messages.</summary>
	public static IReadOnlyList<string> StyleKeys { get; } = new[] { SpanishKey, EnglishKey, FigurineKey };

	public static bool TryParseStyle(string? language, out PieceLetterStyleEnum style)
	{
		switch (language?.Trim().ToLowerInvariant())
		{
			case SpanishKey or "spanish" or "es-es":
				style = PieceLetterStyleEnum.Spanish;

				return true;
			case EnglishKey or "english" or "en-us" or "en-gb":
				style = PieceLetterStyleEnum.English;

				return true;
			case FigurineKey or "uni" or "unicode":
				style = PieceLetterStyleEnum.Figurine;

				return true;
			default:
				style = PieceLetterStyleEnum.Spanish;

				return false;
		}
	}

	public char Format(PlacedPieceModel placed, PieceLetterStyleEnum style)
	{
		ArgumentNullException.ThrowIfNull(placed);

		var symbol = placed.Piece.Symbol;

		if (style == PieceLetterStyleEnum.Figurine)
			return placed.Color.IsWhite ? whiteFigurines[symbol] : blackFigurines[symbol];

		var letter = style == PieceLetterStyleEnum.Spanish ? spanishLetters[symbol] : symbol;

		return placed.Color.IsWhite ? letter : char.ToLowerInvariant(letter);
	}
}
