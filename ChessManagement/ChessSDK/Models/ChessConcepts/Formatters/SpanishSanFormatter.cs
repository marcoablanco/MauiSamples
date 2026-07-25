namespace ChessSDK.Models.ChessConcepts.Formatters;

using ChessSDK.Models.Boards;

/// <summary>
/// Algebraic notation with spanish piece letters: caballo, alfil, torre, dama, rey.
/// Careful: "R" means king here and rook in english notation.
/// </summary>
public sealed class SpanishSanFormatter : SanFormatterBase
{
	private static readonly Dictionary<PieceModel, string> letters = new()
	{
		[PieceModel.Knight] = "C",
		[PieceModel.Bishop] = "A",
		[PieceModel.Rook] = "T",
		[PieceModel.Queen] = "D",
		[PieceModel.King] = "R"
	};

	public SpanishSanFormatter()
		: base(letters)
	{
	}
}
