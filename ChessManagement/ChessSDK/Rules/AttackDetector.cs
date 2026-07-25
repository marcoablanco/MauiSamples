namespace ChessSDK.Rules;

using ChessSDK.Models.Boards;
using ChessSDK.Models.ChessConcepts;

/// <summary>
/// Answers whether a square is attacked by a given side. It is the primitive the whole
/// legality of a chess position rests on: check, checkmate and castling all depend on it.
/// </summary>
public sealed class AttackDetector
{
	internal static readonly int[][] KnightOffsets =
	{
		new[] { 1, 2 }, new[] { 2, 1 }, new[] { 2, -1 }, new[] { 1, -2 },
		new[] { -1, -2 }, new[] { -2, -1 }, new[] { -2, 1 }, new[] { -1, 2 }
	};

	internal static readonly int[][] KingOffsets =
	{
		new[] { 0, 1 }, new[] { 1, 1 }, new[] { 1, 0 }, new[] { 1, -1 },
		new[] { 0, -1 }, new[] { -1, -1 }, new[] { -1, 0 }, new[] { -1, 1 }
	};

	internal static readonly int[][] BishopDirections =
	{
		new[] { 1, 1 }, new[] { 1, -1 }, new[] { -1, -1 }, new[] { -1, 1 }
	};

	internal static readonly int[][] RookDirections =
	{
		new[] { 0, 1 }, new[] { 1, 0 }, new[] { 0, -1 }, new[] { -1, 0 }
	};

	public bool IsSquareAttacked(PositionModel position, CoordinateModel square, GameColorModel byColor)
	{
		ArgumentNullException.ThrowIfNull(position);
		ArgumentNullException.ThrowIfNull(square);
		ArgumentNullException.ThrowIfNull(byColor);

		return IsSquareAttacked(position, square.File.Index, square.Rank.Index, byColor);
	}

	public bool IsSquareAttacked(PositionModel position, int fileIndex, int rankIndex, GameColorModel byColor)
	{
		if (IsAttackedByPawn(position, fileIndex, rankIndex, byColor))
			return true;

		if (IsAttackedByOffset(position, fileIndex, rankIndex, byColor, KnightOffsets, PieceModel.Knight))
			return true;

		if (IsAttackedByOffset(position, fileIndex, rankIndex, byColor, KingOffsets, PieceModel.King))
			return true;

		if (IsAttackedByRay(position, fileIndex, rankIndex, byColor, BishopDirections, PieceModel.Bishop))
			return true;

		return IsAttackedByRay(position, fileIndex, rankIndex, byColor, RookDirections, PieceModel.Rook);
	}

	private static bool IsAttackedByPawn(PositionModel position, int fileIndex, int rankIndex, GameColorModel byColor)
	{
		// A pawn attacking this square sits one rank "behind" it, from the attacker's point of view.
		var pawnRank = rankIndex - byColor.PawnDirection;

		if (pawnRank is < 0 or > 7)
			return false;

		return IsPieceAt(position, fileIndex - 1, pawnRank, byColor, PieceModel.Pawn)
			   || IsPieceAt(position, fileIndex + 1, pawnRank, byColor, PieceModel.Pawn);
	}

	private static bool IsAttackedByOffset(
		PositionModel position,
		int fileIndex,
		int rankIndex,
		GameColorModel byColor,
		int[][] offsets,
		PieceModel piece)
	{
		foreach (var offset in offsets)
			if (IsPieceAt(position, fileIndex + offset[0], rankIndex + offset[1], byColor, piece))
				return true;

		return false;
	}

	private static bool IsAttackedByRay(
		PositionModel position,
		int fileIndex,
		int rankIndex,
		GameColorModel byColor,
		int[][] directions,
		PieceModel slider)
	{
		foreach (var direction in directions)
		{
			var file = fileIndex + direction[0];
			var rank = rankIndex + direction[1];

			while (file is >= 0 and <= 7 && rank is >= 0 and <= 7)
			{
				var placed = position.PieceAt(rank * 8 + file);

				if (placed is not null)
				{
					if (ReferenceEquals(placed.Color, byColor)
						&& (ReferenceEquals(placed.Piece, slider) || ReferenceEquals(placed.Piece, PieceModel.Queen)))
						return true;

					break;
				}

				file += direction[0];
				rank += direction[1];
			}
		}

		return false;
	}

	private static bool IsPieceAt(
		PositionModel position,
		int fileIndex,
		int rankIndex,
		GameColorModel color,
		PieceModel piece)
	{
		if (fileIndex is < 0 or > 7 || rankIndex is < 0 or > 7)
			return false;

		var placed = position.PieceAt(rankIndex * 8 + fileIndex);

		return placed is not null && ReferenceEquals(placed.Piece, piece) && ReferenceEquals(placed.Color, color);
	}
}
