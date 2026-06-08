namespace ChessSDK.Models.ChessConcepts;

using ChessSDK.Models.Players;

public class MatchModel
{
	public MatchModel(PlayerModel player1, PlayerModel player2)
	{
		Player1 = player1;
		Player2 = player2;
	}

	public PlayerModel Player1 { get; set; }
	public PlayerModel Player2 { get; set; }
}