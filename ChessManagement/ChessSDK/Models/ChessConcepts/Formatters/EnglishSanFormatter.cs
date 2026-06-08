namespace ChessSDK.Models.ChessConcepts.Formatters;

using ChessSDK.Models.Boards;

public sealed class EnglishSanFormatter : IMoveNotationFormatter
{
	public string Format(MoveModel move)
	{
		var piece = FormatPiece(move.Piece);

		var pawnFile =
			move.Piece == PieceModel.Pawn && move.IsCapture
				? move.From.ToString()[0].ToString()
				: string.Empty;

		var capture = move.IsCapture ? "x" : "";
		var dest = move.To.ToString();
		var promo = move.IsPromotion ? $"={FormatPiece(move.Promotion!)}" : "";

		return $"{pawnFile}{piece}{capture}{dest}{promo}";
	}

	private static string FormatPiece(PieceModel piece)
	{
		return piece switch
			   {
				   _ when piece == PieceModel.Knight => "N",
				   _ when piece == PieceModel.Bishop => "B",
				   _ when piece == PieceModel.Rook   => "R",
				   _ when piece == PieceModel.Queen  => "Q",
				   _ when piece == PieceModel.King   => "K",
				   _ => ""
			   };
	}
}