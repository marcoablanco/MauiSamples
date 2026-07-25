namespace ChessSDK.Models.Boards;

/// <summary>
/// Row of a chess board, represented by a rank (1-8).
/// Value object: two instances with the same digit are equal, and the conversions always
/// return the canonical instance, so reference equality keeps working too.
/// </summary>
public sealed class RankModel : IEquatable<RankModel>
{
	public static readonly RankModel R1 = new('1');
	public static readonly RankModel R2 = new('2');
	public static readonly RankModel R3 = new('3');
	public static readonly RankModel R4 = new('4');
	public static readonly RankModel R5 = new('5');
	public static readonly RankModel R6 = new('6');
	public static readonly RankModel R7 = new('7');
	public static readonly RankModel R8 = new('8');

	private static readonly RankModel[] all = { R1, R2, R3, R4, R5, R6, R7, R8 };

	private readonly char name;

	private RankModel(char name)
	{
		if (name is < '1' or > '8')
			throw new ArgumentOutOfRangeException(nameof(name), "Rank must be between '1' and '8'.");

		this.name = name;
	}

	/// <summary>Digit of the rank, from '1' to '8'.</summary>
	public char Name => name;

	/// <summary>Zero based position of the rank, 0 for '1' and 7 for '8'.</summary>
	public int Index => name - '1';

	public static RankModel FromChar(char value)
	{
		if (value is < '1' or > '8')
			throw new ArgumentOutOfRangeException(nameof(value), "Rank must be between '1' and '8'.");

		return all[value - '1'];
	}

	public static RankModel FromIndex(int index)
	{
		if (index is < 0 or > 7)
			throw new ArgumentOutOfRangeException(nameof(index), "Rank index must be between 0 and 7.");

		return all[index];
	}

	public static bool TryFromIndex(int index, out RankModel rank)
	{
		if (index is < 0 or > 7)
		{
			rank = R1;

			return false;
		}

		rank = all[index];

		return true;
	}

	public static implicit operator RankModel(char c) => FromChar(c);

	public static implicit operator RankModel(string s)
	{
		ArgumentException.ThrowIfNullOrEmpty(s);

		return FromChar(s[0]);
	}

	public static implicit operator char(RankModel rank) => rank.name;

	public static bool operator ==(RankModel? left, RankModel? right)
	{
		if (ReferenceEquals(left, right))
			return true;

		return left is not null && right is not null && left.name == right.name;
	}

	public static bool operator !=(RankModel? left, RankModel? right) => !(left == right);

	public bool Equals(RankModel? other) => other is not null && name == other.name;

	public override bool Equals(object? obj) => obj is RankModel other && Equals(other);

	public override int GetHashCode() => name.GetHashCode();

	public override string ToString() => name.ToString();
}
