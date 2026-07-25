namespace ChessSDK.Rules;

using ChessSDK.Enums;
using ChessSDK.Models.Boards;
using ChessSDK.Models.ChessConcepts;

/// <summary>
/// Decides whether a position is still playable and, if it is not, why.
/// </summary>
public sealed class GameResultEvaluator
{
	private const int HalfMovesForFiftyMoveRule = 100;

	private readonly LegalityValidator legalityValidator;

	public GameResultEvaluator()
		: this(new LegalityValidator())
	{
	}

	public GameResultEvaluator(LegalityValidator legalityValidator)
	{
		ArgumentNullException.ThrowIfNull(legalityValidator);

		this.legalityValidator = legalityValidator;
	}

	/// <summary>
	/// Only the two kings, or a lone minor piece, or a bishop each on squares of the same color:
	/// no sequence of legal moves can produce a checkmate.
	/// </summary>
	public static bool HasInsufficientMaterial(PositionModel position)
	{
		ArgumentNullException.ThrowIfNull(position);

		var minorSquares = new List<int>(4);
		var minorPieces = new List<PieceModel>(4);

		for (var index = 0; index < PositionModel.SquareCount; index++)
		{
			var placed = position.PieceAt(index);

			if (placed is null || ReferenceEquals(placed.Piece, PieceModel.King))
				continue;

			if (ReferenceEquals(placed.Piece, PieceModel.Pawn)
				|| ReferenceEquals(placed.Piece, PieceModel.Rook)
				|| ReferenceEquals(placed.Piece, PieceModel.Queen))
				return false;

			minorPieces.Add(placed.Piece);
			minorSquares.Add(index);
		}

		if (minorPieces.Count <= 1)
			return true;

		if (minorPieces.Count > 2)
			return false;

		if (!ReferenceEquals(minorPieces[0], PieceModel.Bishop) || !ReferenceEquals(minorPieces[1], PieceModel.Bishop))
			return false;

		return SquareColorOf(minorSquares[0]) == SquareColorOf(minorSquares[1]);
	}

	/// <summary>
	/// Evaluates the position. <paramref name="playedPositions" /> is the list of every position
	/// that has occurred in the game, including the current one, and is only needed to detect
	/// threefold repetition.
	/// </summary>
	public GameResultEnum Evaluate(PositionModel position, IEnumerable<PositionModel>? playedPositions = null)
	{
		ArgumentNullException.ThrowIfNull(position);

		if (legalityValidator.GenerateLegal(position).Count == 0)
			return legalityValidator.IsInCheck(position) ? GameResultEnum.Checkmate : GameResultEnum.Stalemate;

		if (HasInsufficientMaterial(position))
			return GameResultEnum.InsufficientMaterial;

		if (IsThreefoldRepetition(position, playedPositions))
			return GameResultEnum.ThreefoldRepetition;

		return position.HalfMoveClock >= HalfMovesForFiftyMoveRule
				   ? GameResultEnum.FiftyMoveRule
				   : GameResultEnum.InProgress;
	}

	public bool IsCheckmate(PositionModel position) => Evaluate(position) == GameResultEnum.Checkmate;

	public bool IsStalemate(PositionModel position) => Evaluate(position) == GameResultEnum.Stalemate;

	private static bool IsThreefoldRepetition(PositionModel position, IEnumerable<PositionModel>? playedPositions)
	{
		if (playedPositions is null)
			return false;

		var key = position.ToRepetitionKey();
		var occurrences = 0;

		foreach (var played in playedPositions)
			if (played.ToRepetitionKey() == key && ++occurrences >= 3)
				return true;

		return false;
	}

	private static int SquareColorOf(int squareIndex) => (squareIndex / 8 + squareIndex % 8) % 2;
}
