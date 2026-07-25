namespace ChessSDK.Models.ChessConcepts.Formatters;

using ChessSDK.Models.Boards;

/// <summary>
/// Algebraic notation with unicode piece symbols, which reads the same in any language.
/// The white symbols are used for both sides, as is customary.
/// </summary>
public sealed class FigurineSanFormatter : SanFormatterBase
{
	private static readonly Dictionary<PieceModel, string> letters = new()
	{
		[PieceModel.Knight] = "\u2658",
		[PieceModel.Bishop] = "\u2657",
		[PieceModel.Rook] = "\u2656",
		[PieceModel.Queen] = "\u2655",
		[PieceModel.King] = "\u2654"
	};

	public FigurineSanFormatter()
		: base(letters)
	{
	}
}
