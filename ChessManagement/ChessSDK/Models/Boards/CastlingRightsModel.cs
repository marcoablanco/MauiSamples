namespace ChessSDK.Models.Boards;

using ChessSDK.Models.ChessConcepts;

/// <summary>
/// Castling rights of both sides. Immutable: every mutation returns another cached instance.
/// </summary>
public sealed class CastlingRightsModel : IEquatable<CastlingRightsModel>
{
	private const int WhiteKingSideFlag = 1;
	private const int WhiteQueenSideFlag = 2;
	private const int BlackKingSideFlag = 4;
	private const int BlackQueenSideFlag = 8;

	private static readonly CastlingRightsModel[] all = BuildAll();

	private readonly int flags;

	private CastlingRightsModel(int flags)
	{
		this.flags = flags;
	}

	public static CastlingRightsModel None => all[0];

	public static CastlingRightsModel All => all[15];

	public bool WhiteKingSide => (flags & WhiteKingSideFlag) != 0;

	public bool WhiteQueenSide => (flags & WhiteQueenSideFlag) != 0;

	public bool BlackKingSide => (flags & BlackKingSideFlag) != 0;

	public bool BlackQueenSide => (flags & BlackQueenSideFlag) != 0;

	public bool IsEmpty => flags == 0;

	public static CastlingRightsModel Create(bool whiteKingSide, bool whiteQueenSide, bool blackKingSide, bool blackQueenSide)
	{
		var flags = (whiteKingSide ? WhiteKingSideFlag : 0)
					| (whiteQueenSide ? WhiteQueenSideFlag : 0)
					| (blackKingSide ? BlackKingSideFlag : 0)
					| (blackQueenSide ? BlackQueenSideFlag : 0);

		return all[flags];
	}

	/// <summary>Parses the third field of a FEN string, for example "KQkq" or "-".</summary>
	public static bool TryParse(string? text, out CastlingRightsModel rights)
	{
		rights = None;

		if (string.IsNullOrWhiteSpace(text))
			return false;

		var trimmed = text.Trim();

		if (trimmed == "-")
			return true;

		var flags = 0;

		foreach (var letter in trimmed)
		{
			var flag = letter switch
					   {
						   'K' => WhiteKingSideFlag,
						   'Q' => WhiteQueenSideFlag,
						   'k' => BlackKingSideFlag,
						   'q' => BlackQueenSideFlag,
						   _ => 0
					   };

			if (flag == 0 || (flags & flag) != 0)
				return false;

			flags |= flag;
		}

		rights = all[flags];

		return true;
	}

	public static bool operator ==(CastlingRightsModel? left, CastlingRightsModel? right)
	{
		if (ReferenceEquals(left, right))
			return true;

		return left is not null && right is not null && left.flags == right.flags;
	}

	public static bool operator !=(CastlingRightsModel? left, CastlingRightsModel? right) => !(left == right);

	private static CastlingRightsModel[] BuildAll()
	{
		var combinations = new CastlingRightsModel[16];

		for (var flags = 0; flags < combinations.Length; flags++)
			combinations[flags] = new CastlingRightsModel(flags);

		return combinations;
	}

	public bool HasKingSide(GameColorModel color) => color.IsWhite ? WhiteKingSide : BlackKingSide;

	public bool HasQueenSide(GameColorModel color) => color.IsWhite ? WhiteQueenSide : BlackQueenSide;

	/// <summary>Removes both castling rights of a color, for example after its king moves.</summary>
	public CastlingRightsModel Without(GameColorModel color)
		=> all[flags & ~(color.IsWhite ? WhiteKingSideFlag | WhiteQueenSideFlag : BlackKingSideFlag | BlackQueenSideFlag)];

	public CastlingRightsModel WithoutKingSide(GameColorModel color)
		=> all[flags & ~(color.IsWhite ? WhiteKingSideFlag : BlackKingSideFlag)];

	public CastlingRightsModel WithoutQueenSide(GameColorModel color)
		=> all[flags & ~(color.IsWhite ? WhiteQueenSideFlag : BlackQueenSideFlag)];

	/// <summary>Removes the rights lost when the rook standing on the given square moves or is captured.</summary>
	public CastlingRightsModel WithoutRookSquare(CoordinateModel square)
		=> square.Index switch
		   {
			   0 => all[flags & ~WhiteQueenSideFlag],
			   7 => all[flags & ~WhiteKingSideFlag],
			   56 => all[flags & ~BlackQueenSideFlag],
			   63 => all[flags & ~BlackKingSideFlag],
			   _ => this
		   };

	public bool Equals(CastlingRightsModel? other) => other is not null && flags == other.flags;

	public override bool Equals(object? obj) => obj is CastlingRightsModel other && Equals(other);

	public override int GetHashCode() => flags;

	/// <summary>Third field of a FEN string.</summary>
	public override string ToString()
	{
		if (flags == 0)
			return "-";

		var text = string.Empty;

		if (WhiteKingSide) text += "K";
		if (WhiteQueenSide) text += "Q";
		if (BlackKingSide) text += "k";
		if (BlackQueenSide) text += "q";

		return text;
	}
}
