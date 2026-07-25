namespace ChessSDK.Models.Boards;

/// <summary>
/// Column of a chess board, represented by a file (a-h).
/// Value object: two instances with the same letter are equal, and the conversions always
/// return the canonical instance, so reference equality keeps working too.
/// </summary>
public sealed class FileModel : IEquatable<FileModel>
{
	public static readonly FileModel A = new('a');
	public static readonly FileModel B = new('b');
	public static readonly FileModel C = new('c');
	public static readonly FileModel D = new('d');
	public static readonly FileModel E = new('e');
	public static readonly FileModel F = new('f');
	public static readonly FileModel G = new('g');
	public static readonly FileModel H = new('h');

	private static readonly FileModel[] all = { A, B, C, D, E, F, G, H };

	private readonly char name;

	private FileModel(char name)
	{
		if (name is < 'a' or > 'h')
			throw new ArgumentOutOfRangeException(nameof(name), "File must be between 'a' and 'h'.");

		this.name = name;
	}

	/// <summary>Letter of the file, from 'a' to 'h'.</summary>
	public char Name => name;

	/// <summary>Zero based position of the file, 0 for 'a' and 7 for 'h'.</summary>
	public int Index => name - 'a';

	public static FileModel FromChar(char value)
	{
		var normalized = char.ToLowerInvariant(value);

		if (normalized is < 'a' or > 'h')
			throw new ArgumentOutOfRangeException(nameof(value), "File must be between 'a' and 'h'.");

		return all[normalized - 'a'];
	}

	public static FileModel FromIndex(int index)
	{
		if (index is < 0 or > 7)
			throw new ArgumentOutOfRangeException(nameof(index), "File index must be between 0 and 7.");

		return all[index];
	}

	public static bool TryFromIndex(int index, out FileModel file)
	{
		if (index is < 0 or > 7)
		{
			file = A;

			return false;
		}

		file = all[index];

		return true;
	}

	public static implicit operator FileModel(char c) => FromChar(c);

	public static implicit operator FileModel(string s)
	{
		ArgumentException.ThrowIfNullOrEmpty(s);

		return FromChar(s[0]);
	}

	public static implicit operator char(FileModel file) => file.name;

	public static bool operator ==(FileModel? left, FileModel? right)
	{
		if (ReferenceEquals(left, right))
			return true;

		return left is not null && right is not null && left.name == right.name;
	}

	public static bool operator !=(FileModel? left, FileModel? right) => !(left == right);

	public bool Equals(FileModel? other) => other is not null && name == other.name;

	public override bool Equals(object? obj) => obj is FileModel other && Equals(other);

	public override int GetHashCode() => name.GetHashCode();

	public override string ToString() => name.ToString();
}
