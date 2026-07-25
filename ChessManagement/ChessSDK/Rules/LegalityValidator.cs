namespace ChessSDK.Rules;

using ChessSDK.Models.Boards;
using ChessSDK.Models.ChessConcepts;

/// <summary>
/// Turns pseudo legal moves into legal ones by discarding every move that leaves the own king
/// in check. This is the class the MCP server relies on to make illegal moves impossible.
/// </summary>
public sealed class LegalityValidator
{
	private readonly MoveGenerator moveGenerator;
	private readonly AttackDetector attackDetector;

	public LegalityValidator()
		: this(new MoveGenerator(), new AttackDetector())
	{
	}

	public LegalityValidator(MoveGenerator moveGenerator, AttackDetector attackDetector)
	{
		ArgumentNullException.ThrowIfNull(moveGenerator);
		ArgumentNullException.ThrowIfNull(attackDetector);

		this.moveGenerator = moveGenerator;
		this.attackDetector = attackDetector;
	}

	public IReadOnlyList<MoveModel> GenerateLegal(PositionModel position)
	{
		ArgumentNullException.ThrowIfNull(position);

		var color = position.SideToMove;
		var legal = new List<MoveModel>(48);

		foreach (var move in moveGenerator.GeneratePseudoLegal(position))
			if (!LeavesOwnKingInCheck(position.Apply(move), color))
				legal.Add(move);

		return legal;
	}

	public bool IsInCheck(PositionModel position)
	{
		ArgumentNullException.ThrowIfNull(position);

		return IsInCheck(position, position.SideToMove);
	}

	public bool IsInCheck(PositionModel position, GameColorModel color)
	{
		ArgumentNullException.ThrowIfNull(position);
		ArgumentNullException.ThrowIfNull(color);

		var king = position.FindKing(color);

		return king is not null && attackDetector.IsSquareAttacked(position, king, color.Opposite);
	}

	public bool IsLegal(PositionModel position, MoveModel move)
	{
		ArgumentNullException.ThrowIfNull(position);
		ArgumentNullException.ThrowIfNull(move);

		return GenerateLegal(position).Contains(move);
	}

	private bool LeavesOwnKingInCheck(PositionModel position, GameColorModel color)
	{
		var king = position.FindKing(color);

		return king is not null && attackDetector.IsSquareAttacked(position, king, color.Opposite);
	}
}
