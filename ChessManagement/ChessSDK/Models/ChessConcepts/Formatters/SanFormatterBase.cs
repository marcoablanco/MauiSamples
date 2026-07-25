namespace ChessSDK.Models.ChessConcepts.Formatters;

using ChessSDK.Enums;
using ChessSDK.Models.Boards;
using ChessSDK.Rules;

/// <summary>
/// Shared algebraic notation logic. The three SAN dialects only differ in the letter used for
/// each piece, so subclasses just supply that map.
/// </summary>
public abstract class SanFormatterBase : IMoveNotationFormatter
{
	public const string KingSideCastle = "O-O";
	public const string QueenSideCastle = "O-O-O";

	private static readonly LegalityValidator legalityValidator = new();

	private readonly IReadOnlyDictionary<PieceModel, string> pieceLetters;

	protected SanFormatterBase(IReadOnlyDictionary<PieceModel, string> pieceLetters)
	{
		ArgumentNullException.ThrowIfNull(pieceLetters);

		this.pieceLetters = pieceLetters;
	}

	public string Format(MoveModel move)
	{
		ArgumentNullException.ThrowIfNull(move);

		return FormatCore(move, null);
	}

	public string Format(MoveModel move, PositionModel position)
	{
		ArgumentNullException.ThrowIfNull(move);
		ArgumentNullException.ThrowIfNull(position);

		return FormatCore(move, position);
	}

	/// <summary>Letter of a piece in this dialect. The pawn has no letter.</summary>
	public string FormatPiece(PieceModel piece)
	{
		ArgumentNullException.ThrowIfNull(piece);

		return pieceLetters.TryGetValue(piece, out var letter) ? letter : string.Empty;
	}

	/// <summary>
	/// Smallest prefix that tells this move apart from any other legal move of the same kind of
	/// piece reaching the same square: nothing, the file, the rank or the whole square.
	/// </summary>
	private static string Disambiguate(MoveModel move, PositionModel? position)
	{
		if (position is null || ReferenceEquals(move.Piece, PieceModel.Pawn))
			return string.Empty;

		var rivals = legalityValidator.GenerateLegal(position)
									  .Where(candidate => ReferenceEquals(candidate.Piece, move.Piece)
														  && candidate.To == move.To
														  && candidate.From != move.From)
									  .ToArray();

		if (rivals.Length == 0)
			return string.Empty;

		if (rivals.All(candidate => candidate.From.File != move.From.File))
			return move.From.File.Name.ToString();

		if (rivals.All(candidate => candidate.From.Rank != move.From.Rank))
			return move.From.Rank.Name.ToString();

		return move.From.ToString();
	}

	private static string CheckMark(MoveModel move, PositionModel? position)
	{
		if (position is null)
			return string.Empty;

		var next = position.Apply(move);

		if (!legalityValidator.IsInCheck(next))
			return string.Empty;

		return legalityValidator.GenerateLegal(next).Count == 0 ? "#" : "+";
	}

	private string FormatCore(MoveModel move, PositionModel? position)
	{
		if (move.IsCastle)
			return (move.Kind == MoveKindEnum.CastleKingSide ? KingSideCastle : QueenSideCastle) + CheckMark(move, position);

		var isPawn = ReferenceEquals(move.Piece, PieceModel.Pawn);

		var origin = isPawn
						 ? move.IsCapture ? move.From.File.Name.ToString() : string.Empty
						 : Disambiguate(move, position);

		var capture = move.IsCapture ? "x" : string.Empty;
		var promotion = move.IsPromotion ? $"={FormatPiece(move.Promotion!)}" : string.Empty;

		return $"{FormatPiece(move.Piece)}{origin}{capture}{move.To}{promotion}{CheckMark(move, position)}";
	}
}
