namespace ChessSDK.Models.ChessConcepts.Formatters;

public interface IMoveNotationFormatter
{
	string Format(MoveModel move);
}