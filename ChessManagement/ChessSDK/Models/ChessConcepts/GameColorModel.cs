namespace ChessSDK.Models.ChessConcepts;

/// <summary>
/// Side of a chess game.
/// Sealed singleton: the constructor is private and the only instances are the static ones,
/// so equality is by reference and <c>==</c> is always safe.
/// </summary>
public sealed class GameColorModel
{
	public static readonly GameColorModel White = new("White", 'w');
	public static readonly GameColorModel Black = new("Black", 'b');

	private readonly string name;

	private GameColorModel(string name, char symbol)
	{
		this.name = name;
		Symbol = symbol;
	}

	/// <summary>Lowercase letter used by FEN: 'w' or 'b'.</summary>
	public char Symbol { get; }

	public bool IsWhite => ReferenceEquals(this, White);

	public GameColorModel Opposite => ReferenceEquals(this, White) ? Black : White;

	/// <summary>Direction a pawn of this color advances in: +1 for white, -1 for black.</summary>
	public int PawnDirection => ReferenceEquals(this, White) ? 1 : -1;

	public static bool TryParse(string? text, out GameColorModel color)
	{
		switch (text?.Trim().ToLowerInvariant())
		{
			case "w":
			case "white":
			case "blancas":
				color = White;

				return true;
			case "b":
			case "black":
			case "negras":
				color = Black;

				return true;
			default:
				color = White;

				return false;
		}
	}

	public static implicit operator GameColorModel(string s)
	{
		if (!TryParse(s, out var color))
			throw new ArgumentException("Color must be 'White' or 'Black'.", nameof(s));

		return color;
	}

	public override string ToString() => name;
}
