namespace ChessSDK.Models.Boards;

using ChessSDK.Models.ChessConcepts;

/// <summary>
/// A piece placed on the board, together with the color of its owner.
/// There are only twelve possible combinations, so <see cref="Get" /> returns cached instances.
/// </summary>
public sealed class PlacedPieceModel : IEquatable<PlacedPieceModel>
{
	private static readonly PlacedPieceModel[] all = BuildAll();

	public PlacedPieceModel(PieceModel piece, GameColorModel color)
	{
		ArgumentNullException.ThrowIfNull(piece);
		ArgumentNullException.ThrowIfNull(color);

		Piece = piece;
		Color = color;
	}

	public PieceModel Piece { get; }

	public GameColorModel Color { get; }

	/// <summary>FEN letter: uppercase for white, lowercase for black.</summary>
	public char Symbol => Color.IsWhite ? Piece.Symbol : char.ToLowerInvariant(Piece.Symbol);

	public static PlacedPieceModel Get(PieceModel piece, GameColorModel color)
	{
		ArgumentNullException.ThrowIfNull(piece);
		ArgumentNullException.ThrowIfNull(color);

		return all[IndexOf(piece, color)];
	}

	public static bool TryFromSymbol(char symbol, out PlacedPieceModel placed)
	{
		if (!PieceModel.TryFromSymbol(symbol, out var piece))
		{
			placed = all[0];

			return false;
		}

		placed = Get(piece, char.IsUpper(symbol) ? GameColorModel.White : GameColorModel.Black);

		return true;
	}

	public static bool operator ==(PlacedPieceModel? left, PlacedPieceModel? right)
	{
		if (ReferenceEquals(left, right))
			return true;

		return left is not null && right is not null
							   && ReferenceEquals(left.Piece, right.Piece)
							   && ReferenceEquals(left.Color, right.Color);
	}

	public static bool operator !=(PlacedPieceModel? left, PlacedPieceModel? right) => !(left == right);

	private static PlacedPieceModel[] BuildAll()
	{
		var pieces = new PlacedPieceModel[12];

		foreach (var piece in PieceModel.All)
		{
			pieces[IndexOf(piece, GameColorModel.White)] = new PlacedPieceModel(piece, GameColorModel.White);
			pieces[IndexOf(piece, GameColorModel.Black)] = new PlacedPieceModel(piece, GameColorModel.Black);
		}

		return pieces;
	}

	private static int IndexOf(PieceModel piece, GameColorModel color)
	{
		var pieceIndex = "PNBRQK".IndexOf(piece.Symbol);

		return pieceIndex * 2 + (color.IsWhite ? 0 : 1);
	}

	public bool Equals(PlacedPieceModel? other)
		=> other is not null && ReferenceEquals(Piece, other.Piece) && ReferenceEquals(Color, other.Color);

	public override bool Equals(object? obj) => obj is PlacedPieceModel other && Equals(other);

	public override int GetHashCode() => HashCode.Combine(Piece.Symbol, Color.Symbol);

	public override string ToString() => $"{Color} {Piece}";
}
