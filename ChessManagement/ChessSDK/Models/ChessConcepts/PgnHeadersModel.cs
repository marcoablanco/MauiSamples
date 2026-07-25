namespace ChessSDK.Models.ChessConcepts;

/// <summary>
/// Seven tag roster of a PGN game. Unknown values are written as "?", as the standard requires.
/// </summary>
public sealed class PgnHeadersModel
{
	public const string Unknown = "?";

	public string Event { get; init; } = "Casual Game";

	public string Site { get; init; } = Unknown;

	/// <summary>Date in PGN format: "yyyy.MM.dd", with "????.??.??" for unknown dates.</summary>
	public string Date { get; init; } = DateTime.Now.ToString("yyyy.MM.dd");

	public string Round { get; init; } = Unknown;

	public string White { get; init; } = Unknown;

	public string Black { get; init; } = Unknown;
}
