namespace ChessSDK.Models.ChessConcepts.Formatters;

using ChessSDK.Models.Boards;

public sealed class LanFormatter : IMoveNotationFormatter
{
	public string Format(MoveModel move)
	{
		var piece = FormatPiece(move.Piece);
		var from = move.From.ToString();
		var dest = move.To.ToString();
		var capture = move.IsCapture ? "x" : "";
		var promo = move.IsPromotion ? FormatPiece(move.Promotion!) : "";

		return $"{piece}{from}{capture}{dest}{promo}";
	}

	private static string FormatPiece(PieceModel piece)
	{
		return piece switch
			   {
				   _ when piece == PieceModel.Pawn   => "P",
				   _ when piece == PieceModel.Knight => "N",
				   _ when piece == PieceModel.Bishop => "B",
				   _ when piece == PieceModel.Rook   => "R",
				   _ when piece == PieceModel.Queen  => "Q",
				   _ when piece == PieceModel.King   => "K",
				   _ => ""
			   };
	}
}