namespace ChessSDK.Models.Boards;

using ChessSDK.Models.ChessConcepts;

/// <summary>
/// A piece placed on the board, together with the color of its owner.
/// </summary>
public sealed class PlacedPieceModel
{
	public PlacedPieceModel(PieceModel piece, GameColorModel color)
	{
		ArgumentNullException.ThrowIfNull(piece);
		ArgumentNullException.ThrowIfNull(color);

		Piece = piece;
		Color = color;
	}

	public PieceModel Piece { get; }
	public GameColorModel Color { get; }

	public override string ToString() => $"{Color} {Piece}";
}
