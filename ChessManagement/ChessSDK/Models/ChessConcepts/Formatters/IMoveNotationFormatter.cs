namespace ChessSDK.Models.ChessConcepts.Formatters;

using ChessSDK.Models.Boards;

public interface IMoveNotationFormatter
{
	/// <summary>
	/// Formats a move on its own. Without a position there is no way to know whether the move
	/// needs disambiguation or gives check, so those marks are omitted.
	/// </summary>
	string Format(MoveModel move);

	/// <summary>
	/// Formats a move knowing the position it is played from, which allows disambiguation
	/// ("Nbd2") and the check and checkmate marks.
	/// </summary>
	string Format(MoveModel move, PositionModel position);
}
