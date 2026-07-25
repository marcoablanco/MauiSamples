namespace ChessSDK.Rules;

using ChessSDK.Enums;
using ChessSDK.Models.Boards;
using ChessSDK.Models.ChessConcepts;

/// <summary>
/// Generates the pseudo legal moves of a position: every move that respects how the pieces
/// move, without checking whether it leaves the own king in check.
/// Castling is fully validated here, because the "cannot castle through check" rule cannot be
/// deduced from the resulting position.
/// </summary>
public sealed class MoveGenerator
{
	private static readonly int[] pawnCaptureFileDeltas = { -1, 1 };

	private readonly AttackDetector attackDetector;

	public MoveGenerator()
		: this(new AttackDetector())
	{
	}

	public MoveGenerator(AttackDetector attackDetector)
	{
		ArgumentNullException.ThrowIfNull(attackDetector);

		this.attackDetector = attackDetector;
	}

	public IReadOnlyList<MoveModel> GeneratePseudoLegal(PositionModel position)
	{
		ArgumentNullException.ThrowIfNull(position);

		var color = position.SideToMove;
		var moves = new List<MoveModel>(48);

		for (var index = 0; index < PositionModel.SquareCount; index++)
		{
			var placed = position.PieceAt(index);

			if (placed is null || !ReferenceEquals(placed.Color, color))
				continue;

			var from = CoordinateModel.FromIndex(index);

			if (ReferenceEquals(placed.Piece, PieceModel.Pawn))
				AddPawnMoves(position, from, color, moves);
			else if (ReferenceEquals(placed.Piece, PieceModel.Knight))
				AddStepMoves(position, from, color, PieceModel.Knight, AttackDetector.KnightOffsets, moves);
			else if (ReferenceEquals(placed.Piece, PieceModel.King))
				AddStepMoves(position, from, color, PieceModel.King, AttackDetector.KingOffsets, moves);
			else if (ReferenceEquals(placed.Piece, PieceModel.Bishop))
				AddRayMoves(position, from, color, PieceModel.Bishop, AttackDetector.BishopDirections, moves);
			else if (ReferenceEquals(placed.Piece, PieceModel.Rook))
				AddRayMoves(position, from, color, PieceModel.Rook, AttackDetector.RookDirections, moves);
			else if (ReferenceEquals(placed.Piece, PieceModel.Queen))
			{
				AddRayMoves(position, from, color, PieceModel.Queen, AttackDetector.BishopDirections, moves);
				AddRayMoves(position, from, color, PieceModel.Queen, AttackDetector.RookDirections, moves);
			}
		}

		AddCastlingMoves(position, color, moves);

		return moves;
	}

	private static void AddPawnMoves(PositionModel position, CoordinateModel from, GameColorModel color, List<MoveModel> moves)
	{
		var direction = color.PawnDirection;
		var startRank = color.IsWhite ? 1 : 6;
		var promotionRank = color.IsWhite ? 7 : 0;
		var fileIndex = from.File.Index;
		var rankIndex = from.Rank.Index;
		var nextRank = rankIndex + direction;

		if (nextRank is < 0 or > 7)
			return;

		if (position.PieceAt(nextRank * 8 + fileIndex) is null)
		{
			AddPawnDestination(from, CoordinateModel.FromIndexes(fileIndex, nextRank), null, promotionRank, MoveKindEnum.Normal, moves);

			var doubleRank = rankIndex + direction * 2;

			if (rankIndex == startRank && position.PieceAt(doubleRank * 8 + fileIndex) is null)
				moves.Add(new MoveModel(
					PieceModel.Pawn,
					from,
					CoordinateModel.FromIndexes(fileIndex, doubleRank),
					null,
					null,
					MoveKindEnum.DoublePawnPush));
		}

		foreach (var fileDelta in pawnCaptureFileDeltas)
		{
			var targetFile = fileIndex + fileDelta;

			if (targetFile is < 0 or > 7)
				continue;

			var targetIndex = nextRank * 8 + targetFile;
			var target = position.PieceAt(targetIndex);
			var to = CoordinateModel.FromIndexes(targetFile, nextRank);

			if (target is not null)
			{
				if (!ReferenceEquals(target.Color, color))
					AddPawnDestination(from, to, target.Piece, promotionRank, MoveKindEnum.Normal, moves);

				continue;
			}

			if (position.EnPassantTarget is not null && position.EnPassantTarget.Index == targetIndex)
				moves.Add(new MoveModel(PieceModel.Pawn, from, to, PieceModel.Pawn, null, MoveKindEnum.EnPassant));
		}
	}

	private static void AddPawnDestination(
		CoordinateModel from,
		CoordinateModel to,
		PieceModel? captured,
		int promotionRank,
		MoveKindEnum kind,
		List<MoveModel> moves)
	{
		if (to.Rank.Index != promotionRank)
		{
			moves.Add(new MoveModel(PieceModel.Pawn, from, to, captured, null, kind));

			return;
		}

		foreach (var promotion in PieceModel.PromotionPieces)
			moves.Add(new MoveModel(PieceModel.Pawn, from, to, captured, promotion, kind));
	}

	private static void AddStepMoves(
		PositionModel position,
		CoordinateModel from,
		GameColorModel color,
		PieceModel piece,
		int[][] offsets,
		List<MoveModel> moves)
	{
		foreach (var offset in offsets)
		{
			var file = from.File.Index + offset[0];
			var rank = from.Rank.Index + offset[1];

			if (file is < 0 or > 7 || rank is < 0 or > 7)
				continue;

			var target = position.PieceAt(rank * 8 + file);

			if (target is not null && ReferenceEquals(target.Color, color))
				continue;

			moves.Add(new MoveModel(piece, from, CoordinateModel.FromIndexes(file, rank), target?.Piece));
		}
	}

	private static void AddRayMoves(
		PositionModel position,
		CoordinateModel from,
		GameColorModel color,
		PieceModel piece,
		int[][] directions,
		List<MoveModel> moves)
	{
		foreach (var direction in directions)
		{
			var file = from.File.Index + direction[0];
			var rank = from.Rank.Index + direction[1];

			while (file is >= 0 and <= 7 && rank is >= 0 and <= 7)
			{
				var target = position.PieceAt(rank * 8 + file);

				if (target is not null && ReferenceEquals(target.Color, color))
					break;

				moves.Add(new MoveModel(piece, from, CoordinateModel.FromIndexes(file, rank), target?.Piece));

				if (target is not null)
					break;

				file += direction[0];
				rank += direction[1];
			}
		}
	}

	private void AddCastlingMoves(PositionModel position, GameColorModel color, List<MoveModel> moves)
	{
		if (!position.CastlingRights.HasKingSide(color) && !position.CastlingRights.HasQueenSide(color))
			return;

		var rank = color.IsWhite ? 0 : 7;
		var kingSquare = CoordinateModel.FromIndexes(4, rank);
		var king = position.PieceAt(kingSquare);

		if (king is null || !ReferenceEquals(king.Piece, PieceModel.King) || !ReferenceEquals(king.Color, color))
			return;

		var opponent = color.Opposite;

		if (attackDetector.IsSquareAttacked(position, 4, rank, opponent))
			return;

		if (position.CastlingRights.HasKingSide(color)
			&& HasRook(position, 7, rank, color)
			&& position.PieceAt(rank * 8 + 5) is null
			&& position.PieceAt(rank * 8 + 6) is null
			&& !attackDetector.IsSquareAttacked(position, 5, rank, opponent)
			&& !attackDetector.IsSquareAttacked(position, 6, rank, opponent))
			moves.Add(new MoveModel(
				PieceModel.King,
				kingSquare,
				CoordinateModel.FromIndexes(6, rank),
				null,
				null,
				MoveKindEnum.CastleKingSide));

		if (position.CastlingRights.HasQueenSide(color)
			&& HasRook(position, 0, rank, color)
			&& position.PieceAt(rank * 8 + 1) is null
			&& position.PieceAt(rank * 8 + 2) is null
			&& position.PieceAt(rank * 8 + 3) is null
			&& !attackDetector.IsSquareAttacked(position, 3, rank, opponent)
			&& !attackDetector.IsSquareAttacked(position, 2, rank, opponent))
			moves.Add(new MoveModel(
				PieceModel.King,
				kingSquare,
				CoordinateModel.FromIndexes(2, rank),
				null,
				null,
				MoveKindEnum.CastleQueenSide));
	}

	private static bool HasRook(PositionModel position, int fileIndex, int rankIndex, GameColorModel color)
	{
		var placed = position.PieceAt(rankIndex * 8 + fileIndex);

		return placed is not null && ReferenceEquals(placed.Piece, PieceModel.Rook) && ReferenceEquals(placed.Color, color);
	}
}
