namespace ChessSDK.Models.ChessConcepts;

public sealed class GameColorModel
{
	public static readonly GameColorModel White = new("White");
	public static readonly GameColorModel Black = new("Black");

	private readonly string name;

	private GameColorModel(string name)
	{
		this.name = name;
	}

	// Optional: implicit conversion from string
	public static implicit operator GameColorModel(string s)
		=> s.ToLower() switch
		   {
			   "white" => White,
			   "black" => Black,
			   _ => throw new ArgumentException("Color must be 'White' or 'Black'.", nameof(s))
		   };

	public override string ToString() => name;
}