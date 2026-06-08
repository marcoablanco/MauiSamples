namespace ChessSDK.Models.Boards;

/// <summary>
/// Row of a chess board, represented by a rank (1-8).
/// </summary>
public class RankModel
{
	public static readonly RankModel R1 = new('1');
	public static readonly RankModel R2 = new('2');
	public static readonly RankModel R3 = new('3');
	public static readonly RankModel R4 = new('4');
	public static readonly RankModel R5 = new('5');
	public static readonly RankModel R6 = new('6');
	public static readonly RankModel R7 = new('7');
	public static readonly RankModel R8 = new('8');

	private readonly char name;

	private RankModel(char name)
	{
		if (name is < '1' or > '8')
			throw new ArgumentOutOfRangeException(nameof(name), "Rank must be between '1' and '8'.");
		this.name = name;
	}

	public static implicit operator RankModel(char c) => new(c);
	public static implicit operator RankModel(string s) => new RankModel(s.First());
	public static implicit operator char(RankModel rank) => rank.name;
	public static implicit operator string(RankModel rank) => rank.name.ToString();

	public int Index => name - '1';


	public override string ToString() => name.ToString();
}