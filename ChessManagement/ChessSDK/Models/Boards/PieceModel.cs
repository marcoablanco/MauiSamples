namespace ChessSDK.Models.Boards;

public sealed class PieceModel
{
	public static readonly PieceModel Pawn = new("Pawn");
	public static readonly PieceModel Knight = new("Knight");
	public static readonly PieceModel Bishop = new("Bishop");
	public static readonly PieceModel Rook = new("Rook");
	public static readonly PieceModel Queen = new("Queen");
	public static readonly PieceModel King = new("King");

	private readonly string name;

	private PieceModel(string name)
	{
		this.name = name;
	}

	public override string ToString() => name;
}