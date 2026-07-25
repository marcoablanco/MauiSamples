namespace ChessSDK.Enums;

/// <summary>
/// Nature of a move, needed to apply it to a position (the board effects of an en passant
/// capture or a castling move are not deducible from origin and destination alone).
/// </summary>
public enum MoveKindEnum
{
	Normal = 0,
	DoublePawnPush = 1,
	EnPassant = 2,
	CastleKingSide = 3,
	CastleQueenSide = 4
}
