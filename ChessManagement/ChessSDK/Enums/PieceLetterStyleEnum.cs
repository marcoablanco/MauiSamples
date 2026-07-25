namespace ChessSDK.Enums;

/// <summary>Alphabet used to draw a piece on a board.</summary>
public enum PieceLetterStyleEnum
{
	/// <summary>Spanish letters: T, C, A, D, R, P.</summary>
	Spanish,

	/// <summary>English letters, the ones used by FEN: R, N, B, Q, K, P.</summary>
	English,

	/// <summary>Unicode chess symbols, which read the same in any language.</summary>
	Figurine
}
