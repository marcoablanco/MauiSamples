namespace ChessSDK.Enums;

/// <summary>
/// Outcome of a position once the legal moves of the side to move are known.
/// </summary>
public enum GameResultEnum
{
	InProgress = 0,
	Checkmate = 1,
	Stalemate = 2,
	InsufficientMaterial = 3,
	ThreefoldRepetition = 4,
	FiftyMoveRule = 5,
	Resigned = 6
}
