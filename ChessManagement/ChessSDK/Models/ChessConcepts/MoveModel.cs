namespace ChessSDK.Models.ChessConcepts;

using ChessSDK.Enums;
using ChessSDK.Models.Boards;

public sealed class MoveModel : IEquatable<MoveModel>
{
	public readonly PieceModel Piece;
	public readonly CoordinateModel From;
	public readonly CoordinateModel To;
	public readonly PieceModel? Captured;
	public readonly PieceModel? Promotion;
	public readonly MoveKindEnum Kind;

	public MoveModel(
		PieceModel piece,
		CoordinateModel from,
		CoordinateModel to,
		PieceModel? captured = null,
		PieceModel? promotion = null,
		MoveKindEnum kind = MoveKindEnum.Normal)
	{
		ArgumentNullException.ThrowIfNull(piece);
		ArgumentNullException.ThrowIfNull(from);
		ArgumentNullException.ThrowIfNull(to);

		Piece = piece;
		From = from;
		To = to;
		Captured = captured;
		Promotion = promotion;
		Kind = kind;
	}

	public bool IsCapture => Captured is not null;

	public bool IsPromotion => Promotion is not null;

	public bool IsEnPassant => Kind == MoveKindEnum.EnPassant;

	public bool IsDoublePawnPush => Kind == MoveKindEnum.DoublePawnPush;

	public bool IsCastle => Kind is MoveKindEnum.CastleKingSide or MoveKindEnum.CastleQueenSide;

	/// <summary>Long algebraic notation without piece letter: "e2e4", "e7e8q".</summary>
	public string ToLongAlgebraic()
		=> IsPromotion
			   ? $"{From}{To}{char.ToLowerInvariant(Promotion!.Symbol)}"
			   : $"{From}{To}";

	public bool Equals(MoveModel? other)
		=> other is not null
		   && ReferenceEquals(Piece, other.Piece)
		   && From == other.From
		   && To == other.To
		   && ReferenceEquals(Promotion, other.Promotion)
		   && Kind == other.Kind;

	public override bool Equals(object? obj) => obj is MoveModel other && Equals(other);

	public override int GetHashCode() => HashCode.Combine(Piece.Symbol, From.Index, To.Index, Promotion?.Symbol, (int)Kind);

	public override string ToString() => $"{Piece} {From}->{To}" + (IsCapture ? $" captures {Captured}" : "") + (IsPromotion ? $" promotes to {Promotion}" : "");
}
