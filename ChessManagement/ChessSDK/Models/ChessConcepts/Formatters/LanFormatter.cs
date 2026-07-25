namespace ChessSDK.Models.ChessConcepts.Formatters;

using ChessSDK.Models.Boards;

/// <summary>
/// Long algebraic notation: piece letter, origin, destination. It never needs disambiguation,
/// and it carries no check marks on purpose, so it stays easy to parse by a machine.
/// </summary>
public sealed class LanFormatter : IMoveNotationFormatter
{
	public string Format(MoveModel move)
	{
		ArgumentNullException.ThrowIfNull(move);

		var piece = FormatPiece(move.Piece);
		var capture = move.IsCapture ? "x" : "";
		var promotion = move.IsPromotion ? FormatPiece(move.Promotion!) : "";

		return $"{piece}{move.From}{capture}{move.To}{promotion}";
	}

	public string Format(MoveModel move, PositionModel position) => Format(move);

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
