namespace ChessSDK.Models.ChessConcepts.Formatters;

using ChessSDK.Models.Boards;

/// <summary>Standard algebraic notation with english piece letters: "Nbd2", "exd5", "e8=Q+".</summary>
public sealed class EnglishSanFormatter : SanFormatterBase
{
	private static readonly Dictionary<PieceModel, string> letters = new()
	{
		[PieceModel.Knight] = "N",
		[PieceModel.Bishop] = "B",
		[PieceModel.Rook] = "R",
		[PieceModel.Queen] = "Q",
		[PieceModel.King] = "K"
	};

	public EnglishSanFormatter()
		: base(letters)
	{
	}
}
