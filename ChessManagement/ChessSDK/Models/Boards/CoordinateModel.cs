namespace ChessSDK.Models.Boards;

/// <summary>
/// A coordinate on a chess board, composed of a file (a-h) and a rank (1-8).
/// </summary>
public class CoordinateModel
{
	private CoordinateModel(FileModel file, RankModel rank)
	{
		File = file ?? throw new ArgumentNullException(nameof(file));
		Rank = rank ?? throw new ArgumentNullException(nameof(rank));
	}

	public FileModel File { get; }
	public RankModel Rank { get; }

	public static CoordinateModel Create(FileModel file, RankModel rank)
		=> new(file, rank);

	public static implicit operator CoordinateModel(string s)
	{
		if (string.IsNullOrWhiteSpace(s) || s.Length != 2)
			throw new ArgumentException("Coordinate must be in algebraic form (e.g., 'e4').", nameof(s));

		var file = (FileModel)s[0];
		var rank = (RankModel)s[1];

		return new CoordinateModel(file, rank);
	}

	public static implicit operator string(CoordinateModel sq) => $"{sq.File}{sq.Rank}";

	public override string ToString() => $"{File}{Rank}";
}