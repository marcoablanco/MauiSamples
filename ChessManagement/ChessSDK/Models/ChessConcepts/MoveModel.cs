namespace ChessSDK.Models.ChessConcepts;

using ChessSDK.Models.Boards;

public sealed class MoveModel
{
	public readonly PieceModel Piece;
	public readonly CoordinateModel From;
	public readonly CoordinateModel To;
	public readonly PieceModel? Captured;
	public readonly PieceModel? Promotion;

	public MoveModel(PieceModel piece, CoordinateModel from, CoordinateModel to, PieceModel? captured = null, PieceModel? promotion = null)
	{
		ArgumentNullException.ThrowIfNull(piece);
		ArgumentNullException.ThrowIfNull(from);
		ArgumentNullException.ThrowIfNull(to);

		Piece = piece;
		From = from;
		To = to;
		Captured = captured;
		Promotion = promotion;
	}

	public bool IsCapture => Captured is not null;
	public bool IsPromotion => Promotion is not null;

	public override string ToString() => $"{Piece} {From}->{To}" + (IsCapture ? $" captures {Captured}" : "") + (IsPromotion ? $" promotes to {Promotion}" : "");
}