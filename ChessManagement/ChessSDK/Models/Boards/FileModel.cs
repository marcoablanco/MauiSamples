namespace ChessSDK.Models.Boards;

/// <summary>
/// Column of a chess board, represented by a file (a-h).
/// </summary>
public class FileModel
{
	public static readonly FileModel A = new('a');
	public static readonly FileModel B = new('b');
	public static readonly FileModel C = new('c');
	public static readonly FileModel D = new('d');
	public static readonly FileModel E = new('e');
	public static readonly FileModel F = new('f');
	public static readonly FileModel G = new('g');
	public static readonly FileModel H = new('h');

	private readonly char name;

	private FileModel(char name)
	{
		if (name is < 'a' or > 'h')
			throw new ArgumentOutOfRangeException(nameof(name), "File must be between 'a' and 'h'.");
		this.name = name;
	}

	public static implicit operator FileModel(char c) => new(c);
	public static implicit operator FileModel(string s) => new FileModel(s.First());
	public static implicit operator char(FileModel file) => file.name;
	public static implicit operator string(FileModel file) => file.name.ToString();

	public override string ToString()
	{
		return name.ToString();
	}
}