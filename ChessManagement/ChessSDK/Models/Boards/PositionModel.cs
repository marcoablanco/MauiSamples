namespace ChessSDK.Models.Boards;

using System.Text;
using ChessSDK.Enums;
using ChessSDK.Models.ChessConcepts;

/// <summary>
/// Immutable snapshot of a chess position: the 64 squares plus the state that FEN records.
/// Applying a move never mutates the receiver, it returns a new position.
/// </summary>
public sealed class PositionModel : IEquatable<PositionModel>
{
	public const int SquareCount = 64;

	private static readonly PositionModel startingPosition = BuildStartingPosition();

	private readonly PlacedPieceModel?[] squares;

	private PositionModel(
		PlacedPieceModel?[] squares,
		GameColorModel sideToMove,
		CastlingRightsModel castlingRights,
		CoordinateModel? enPassantTarget,
		int halfMoveClock,
		int fullMoveNumber)
	{
		this.squares = squares;
		SideToMove = sideToMove;
		CastlingRights = castlingRights;
		EnPassantTarget = enPassantTarget;
		HalfMoveClock = halfMoveClock;
		FullMoveNumber = fullMoveNumber;
	}

	public static PositionModel StartingPosition => startingPosition;

	public GameColorModel SideToMove { get; }

	public CastlingRightsModel CastlingRights { get; }

	/// <summary>Square a pawn could be captured on by en passant, or null if there is none.</summary>
	public CoordinateModel? EnPassantTarget { get; }

	public int HalfMoveClock { get; }

	public int FullMoveNumber { get; }

	public static PositionModel Create(
		IReadOnlyList<PlacedPieceModel?> squares,
		GameColorModel sideToMove,
		CastlingRightsModel castlingRights,
		CoordinateModel? enPassantTarget,
		int halfMoveClock,
		int fullMoveNumber)
	{
		ArgumentNullException.ThrowIfNull(squares);
		ArgumentNullException.ThrowIfNull(sideToMove);
		ArgumentNullException.ThrowIfNull(castlingRights);

		if (squares.Count != SquareCount)
			throw new ArgumentException($"A position needs exactly {SquareCount} squares.", nameof(squares));

		if (halfMoveClock < 0)
			throw new ArgumentOutOfRangeException(nameof(halfMoveClock));

		if (fullMoveNumber < 1)
			throw new ArgumentOutOfRangeException(nameof(fullMoveNumber));

		var copy = new PlacedPieceModel?[SquareCount];

		for (var index = 0; index < SquareCount; index++)
			copy[index] = squares[index];

		return new PositionModel(copy, sideToMove, castlingRights, enPassantTarget, halfMoveClock, fullMoveNumber);
	}

	private static PositionModel BuildStartingPosition()
	{
		var squares = new PlacedPieceModel?[SquareCount];

		PieceModel[] backRank =
		{
			PieceModel.Rook, PieceModel.Knight, PieceModel.Bishop, PieceModel.Queen,
			PieceModel.King, PieceModel.Bishop, PieceModel.Knight, PieceModel.Rook
		};

		for (var fileIndex = 0; fileIndex < 8; fileIndex++)
		{
			squares[fileIndex] = PlacedPieceModel.Get(backRank[fileIndex], GameColorModel.White);
			squares[8 + fileIndex] = PlacedPieceModel.Get(PieceModel.Pawn, GameColorModel.White);
			squares[48 + fileIndex] = PlacedPieceModel.Get(PieceModel.Pawn, GameColorModel.Black);
			squares[56 + fileIndex] = PlacedPieceModel.Get(backRank[fileIndex], GameColorModel.Black);
		}

		return new PositionModel(squares, GameColorModel.White, CastlingRightsModel.All, null, 0, 1);
	}

	public PlacedPieceModel? PieceAt(CoordinateModel square)
	{
		ArgumentNullException.ThrowIfNull(square);

		return squares[square.Index];
	}

	public PlacedPieceModel? PieceAt(int squareIndex) => squares[squareIndex];

	public bool IsEmpty(CoordinateModel square) => squares[square.Index] is null;

	public CoordinateModel? FindKing(GameColorModel color)
	{
		for (var index = 0; index < SquareCount; index++)
		{
			var placed = squares[index];

			if (placed is not null && ReferenceEquals(placed.Piece, PieceModel.King) && ReferenceEquals(placed.Color, color))
				return CoordinateModel.FromIndex(index);
		}

		return null;
	}

	public IEnumerable<KeyValuePair<CoordinateModel, PlacedPieceModel>> PiecesOf(GameColorModel color)
	{
		for (var index = 0; index < SquareCount; index++)
		{
			var placed = squares[index];

			if (placed is not null && ReferenceEquals(placed.Color, color))
				yield return new KeyValuePair<CoordinateModel, PlacedPieceModel>(CoordinateModel.FromIndex(index), placed);
		}
	}

	public PositionModel Clone()
		=> new((PlacedPieceModel?[])squares.Clone(), SideToMove, CastlingRights, EnPassantTarget, HalfMoveClock, FullMoveNumber);

	/// <summary>Returns the position that results from playing the move. The move is assumed to be legal.</summary>
	public PositionModel Apply(MoveModel move)
	{
		ArgumentNullException.ThrowIfNull(move);

		var mover = squares[move.From.Index]
					?? throw new InvalidOperationException($"There is no piece on '{move.From}'.");

		var color = mover.Color;
		var next = (PlacedPieceModel?[])squares.Clone();

		next[move.From.Index] = null;

		if (move.IsEnPassant)
			next[CoordinateModel.FromIndexes(move.To.File.Index, move.From.Rank.Index).Index] = null;

		next[move.To.Index] = move.IsPromotion
								  ? PlacedPieceModel.Get(move.Promotion!, color)
								  : mover;

		if (move.IsCastle)
		{
			var rankIndex = move.From.Rank.Index;
			var rookFrom = CoordinateModel.FromIndexes(move.Kind == MoveKindEnum.CastleKingSide ? 7 : 0, rankIndex);
			var rookTo = CoordinateModel.FromIndexes(move.Kind == MoveKindEnum.CastleKingSide ? 5 : 3, rankIndex);

			next[rookTo.Index] = next[rookFrom.Index];
			next[rookFrom.Index] = null;
		}

		var rights = CastlingRights;

		if (ReferenceEquals(mover.Piece, PieceModel.King))
			rights = rights.Without(color);

		rights = rights.WithoutRookSquare(move.From).WithoutRookSquare(move.To);

		CoordinateModel? enPassantTarget = null;

		if (move.IsDoublePawnPush)
			enPassantTarget = CoordinateModel.FromIndexes(
				move.From.File.Index,
				(move.From.Rank.Index + move.To.Rank.Index) / 2);

		var isPawnMove = ReferenceEquals(mover.Piece, PieceModel.Pawn);
		var halfMoveClock = isPawnMove || move.IsCapture ? 0 : HalfMoveClock + 1;
		var fullMoveNumber = color.IsWhite ? FullMoveNumber : FullMoveNumber + 1;

		return new PositionModel(next, color.Opposite, rights, enPassantTarget, halfMoveClock, fullMoveNumber);
	}

	/// <summary>
	/// Key used to detect threefold repetition: placement, side to move, castling rights and
	/// en passant target, but not the clocks.
	/// </summary>
	public string ToRepetitionKey()
	{
		var builder = new StringBuilder(80);

		for (var index = 0; index < SquareCount; index++)
			builder.Append(squares[index]?.Symbol ?? '.');

		builder.Append(SideToMove.Symbol).Append(CastlingRights).Append(EnPassantTarget?.ToString() ?? "-");

		return builder.ToString();
	}

	public bool Equals(PositionModel? other)
	{
		if (other is null)
			return false;

		if (ReferenceEquals(this, other))
			return true;

		if (!ReferenceEquals(SideToMove, other.SideToMove)
			|| CastlingRights != other.CastlingRights
			|| EnPassantTarget != other.EnPassantTarget
			|| HalfMoveClock != other.HalfMoveClock
			|| FullMoveNumber != other.FullMoveNumber)
			return false;

		for (var index = 0; index < SquareCount; index++)
			if (squares[index] != other.squares[index])
				return false;

		return true;
	}

	public override bool Equals(object? obj) => obj is PositionModel other && Equals(other);

	public override int GetHashCode() => HashCode.Combine(ToRepetitionKey(), HalfMoveClock, FullMoveNumber);

	public override string ToString() => ToRepetitionKey();
}
