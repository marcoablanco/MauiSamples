namespace ChessSDK.Models.Players;

using ChessSDK.Models.ChessConcepts;

public class PlayerModel
{
	public PlayerModel(GameColorModel gameColorModel)
	{
		GameColorModel = gameColorModel;
	}

	public string? Id { get; set; }
	public string? Name { get; set; }
	public GameColorModel GameColorModel { get; }
}