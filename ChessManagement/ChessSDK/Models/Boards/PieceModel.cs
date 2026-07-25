namespace ChessSDK.Models.Boards;

/// <summary>
/// Kind of chess piece, with no color attached (see <see cref="PlacedPieceModel" />).
/// Sealed singleton: the constructor is private and the only instances are the static ones,
/// so equality is by reference and <c>==</c> is always safe.
/// </summary>
public sealed class PieceModel
{
	public static readonly PieceModel Pawn = new("Pawn", 'P', 1);
	public static readonly PieceModel Knight = new("Knight", 'N', 3);
	public static readonly PieceModel Bishop = new("Bishop", 'B', 3);
	public static readonly PieceModel Rook = new("Rook", 'R', 5);
	public static readonly PieceModel Queen = new("Queen", 'Q', 9);
	public static readonly PieceModel King = new("King", 'K', 0);

	private static readonly PieceModel[] all = { Pawn, Knight, Bishop, Rook, Queen, King };

	private readonly string name;

	private PieceModel(string name, char symbol, int value)
	{
		this.name = name;
		Symbol = symbol;
		Value = value;
	}

	/// <summary>Uppercase english letter of the piece, as used by FEN and SAN.</summary>
	public char Symbol { get; }

	/// <summary>Conventional material value. The king is worth 0 because it cannot be traded.</summary>
	public int Value { get; }

	/// <summary>Every kind of piece, ordered by material value.</summary>
	public static IReadOnlyList<PieceModel> All => all;

	/// <summary>Pieces a pawn may promote to.</summary>
	public static IReadOnlyList<PieceModel> PromotionPieces { get; } = new[] { Queen, Rook, Bishop, Knight };

	public static PieceModel FromSymbol(char symbol)
	{
		if (!TryFromSymbol(symbol, out var piece))
			throw new ArgumentOutOfRangeException(nameof(symbol), $"'{symbol}' is not a valid piece symbol.");

		return piece;
	}

	public static bool TryFromSymbol(char symbol, out PieceModel piece)
	{
		switch (char.ToUpperInvariant(symbol))
		{
			case 'P':
				piece = Pawn;

				return true;
			case 'N':
				piece = Knight;

				return true;
			case 'B':
				piece = Bishop;

				return true;
			case 'R':
				piece = Rook;

				return true;
			case 'Q':
				piece = Queen;

				return true;
			case 'K':
				piece = King;

				return true;
			default:
				piece = Pawn;

				return false;
		}
	}

	public override string ToString() => name;
}
