namespace ChessSDK.Models.Boards;

/// <summary>
/// A coordinate on a chess board, composed of a file (a-h) and a rank (1-8).
/// Value object: equality is by file and rank. The 64 squares are cached, so
/// <see cref="Create" /> always returns the same instance for the same square.
/// </summary>
public sealed class CoordinateModel : IEquatable<CoordinateModel>
{
	private static readonly CoordinateModel[] all = BuildAll();

	private readonly int index;

	private CoordinateModel(FileModel file, RankModel rank)
	{
		File = file ?? throw new ArgumentNullException(nameof(file));
		Rank = rank ?? throw new ArgumentNullException(nameof(rank));
		index = rank.Index * 8 + file.Index;
	}

	public FileModel File { get; }

	public RankModel Rank { get; }

	/// <summary>Zero based square number, 0 for a1 and 63 for h8.</summary>
	public int Index => index;

	/// <summary>Every square of the board, ordered from a1 to h8.</summary>
	public static IReadOnlyList<CoordinateModel> All => all;

	public static CoordinateModel Create(FileModel file, RankModel rank)
	{
		ArgumentNullException.ThrowIfNull(file);
		ArgumentNullException.ThrowIfNull(rank);

		return all[rank.Index * 8 + file.Index];
	}

	public static CoordinateModel FromIndex(int index)
	{
		if (index is < 0 or > 63)
			throw new ArgumentOutOfRangeException(nameof(index), "Square index must be between 0 and 63.");

		return all[index];
	}

	public static CoordinateModel FromIndexes(int fileIndex, int rankIndex)
		=> Create(FileModel.FromIndex(fileIndex), RankModel.FromIndex(rankIndex));

	public static bool TryFromIndexes(int fileIndex, int rankIndex, out CoordinateModel coordinate)
	{
		if (fileIndex is < 0 or > 7 || rankIndex is < 0 or > 7)
		{
			coordinate = all[0];

			return false;
		}

		coordinate = all[rankIndex * 8 + fileIndex];

		return true;
	}

	public static bool TryParse(string? text, out CoordinateModel coordinate)
	{
		coordinate = all[0];

		if (text is null)
			return false;

		var trimmed = text.Trim();

		if (trimmed.Length != 2)
			return false;

		var file = char.ToLowerInvariant(trimmed[0]);
		var rank = trimmed[1];

		if (file is < 'a' or > 'h' || rank is < '1' or > '8')
			return false;

		coordinate = all[(rank - '1') * 8 + (file - 'a')];

		return true;
	}

	public static implicit operator CoordinateModel(string s)
	{
		if (!TryParse(s, out var coordinate))
			throw new ArgumentException("Coordinate must be in algebraic form (e.g., 'e4').", nameof(s));

		return coordinate;
	}

	public static implicit operator string(CoordinateModel sq) => sq.ToString();

	public static bool operator ==(CoordinateModel? left, CoordinateModel? right)
	{
		if (ReferenceEquals(left, right))
			return true;

		return left is not null && right is not null && left.index == right.index;
	}

	public static bool operator !=(CoordinateModel? left, CoordinateModel? right) => !(left == right);

	private static CoordinateModel[] BuildAll()
	{
		var squares = new CoordinateModel[64];

		for (var rankIndex = 0; rankIndex < 8; rankIndex++)
		for (var fileIndex = 0; fileIndex < 8; fileIndex++)
			squares[rankIndex * 8 + fileIndex] =
				new CoordinateModel(FileModel.FromIndex(fileIndex), RankModel.FromIndex(rankIndex));

		return squares;
	}

	/// <summary>Square reached by moving the given amount of files and ranks, if it is on the board.</summary>
	public bool TryOffset(int fileDelta, int rankDelta, out CoordinateModel coordinate)
		=> TryFromIndexes(File.Index + fileDelta, Rank.Index + rankDelta, out coordinate);

	public bool Equals(CoordinateModel? other) => other is not null && index == other.index;

	public override bool Equals(object? obj)
		=> obj switch
		   {
			   CoordinateModel other => Equals(other),
			   string text => TryParse(text, out var parsed) && index == parsed.index,
			   _ => false
		   };

	public override int GetHashCode() => index;

	public override string ToString() => $"{File.Name}{Rank.Name}";
}
