namespace ChessSDK.Models.ChessConcepts.Formatters;

/// <summary>
/// Creates the notation formatter matching a notation key.
/// </summary>
public static class MoveNotationFormatterFactory
{
	public const string EnglishSan = "san-en";
	public const string SpanishSan = "san-es";
	public const string Figurine = "figurine";
	public const string LongAlgebraic = "lan";

	public static IReadOnlyList<string> AvailableNotations { get; } =
		new[] { EnglishSan, SpanishSan, Figurine, LongAlgebraic };

	public static IMoveNotationFormatter Create(string? notation)
		=> notation?.Trim().ToLowerInvariant() switch
		   {
			   SpanishSan or "es" or "spanish" => new SpanishSanFormatter(),
			   Figurine or "fig" => new FigurineSanFormatter(),
			   LongAlgebraic or "long" => new LanFormatter(),
			   _ => new EnglishSanFormatter()
		   };
}

