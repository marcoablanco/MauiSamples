namespace ChessSDK.Models.ChessConcepts;

using System.Text;
using ChessSDK.Enums;
using ChessSDK.Models.Boards;
using ChessSDK.Notation;
using ChessSDK.Rules;

/// <summary>
/// State of a single chess game: the current position, every position that has occurred and the
/// list of moves played. Every move is validated against the legal move generator, so an illegal
/// move can never be applied.
/// </summary>
public sealed class GameSessionModel
{
	private static readonly FenSerializer fenSerializer = new();
	private static readonly LegalityValidator legalityValidator = new();
	private static readonly GameResultEvaluator resultEvaluator = new();

	private readonly List<MoveModel> history = new();
	private readonly List<PositionModel> positions = new();

	public GameSessionModel(string id, GameColorModel humanColor)
	{
		Id = id;
		HumanColor = humanColor;
		Reset();
	}

	public string Id { get; }

	public GameColorModel HumanColor { get; }

	public PositionModel Position => positions[^1];

	public GameColorModel SideToMove => Position.SideToMove;

	public int FullMoveNumber => Position.FullMoveNumber;

	public int HalfMoveClock => Position.HalfMoveClock;

	public IReadOnlyList<MoveModel> History => history;

	public bool IsInCheck => legalityValidator.IsInCheck(Position);

	public GameResultEnum Result => resultEvaluator.Evaluate(Position, positions);

	public bool IsOver => Result != GameResultEnum.InProgress;

	private static PieceModel? ParsePromotion(char letter)
		=> letter switch
		   {
			   'q' or 'd' => PieceModel.Queen,
			   'r' or 't' => PieceModel.Rook,
			   'b' or 'a' => PieceModel.Bishop,
			   'n' or 'c' => PieceModel.Knight,
			   _ => null
		   };

	public void Reset()
	{
		history.Clear();
		positions.Clear();
		positions.Add(PositionModel.StartingPosition);
	}

	public IReadOnlyList<MoveModel> LegalMoves() => legalityValidator.GenerateLegal(Position);

	public IReadOnlyList<MoveModel> LegalMovesFrom(CoordinateModel from)
	{
		ArgumentNullException.ThrowIfNull(from);

		return LegalMoves().Where(move => move.From == from).ToArray();
	}

	public PlacedPieceModel? PieceAt(string square)
		=> CoordinateModel.TryParse(square, out var coordinate) ? Position.PieceAt(coordinate) : null;

	/// <summary>
	/// Applies a move expressed in long algebraic form: "e2e4", "e7e8q".
	/// The move is rejected unless it is one of the legal moves of the position.
	/// </summary>
	public bool TryApplyMove(string move, out string error)
	{
		error = string.Empty;

		if (string.IsNullOrWhiteSpace(move))
		{
			error = "El movimiento no puede estar vacio. Usa notacion larga, por ejemplo 'e2e4'.";

			return false;
		}

		if (IsOver)
		{
			error = $"La partida ya ha terminado ({Result}). No se pueden aplicar mas movimientos.";

			return false;
		}

		var normalized = move.Trim().Replace("-", "").Replace("x", "").ToLowerInvariant();

		if (normalized.Length is not (4 or 5))
		{
			error = $"'{move}' no tiene formato valido. Usa origen+destino, por ejemplo 'e2e4' o 'e7e8q'.";

			return false;
		}

		if (!CoordinateModel.TryParse(normalized[..2], out var from))
		{
			error = $"La casilla de origen '{normalized[..2]}' no existe.";

			return false;
		}

		if (!CoordinateModel.TryParse(normalized[2..4], out var to))
		{
			error = $"La casilla de destino '{normalized[2..4]}' no existe.";

			return false;
		}

		PieceModel? promotion = null;

		if (normalized.Length == 5)
		{
			promotion = ParsePromotion(normalized[4]);

			if (promotion is null)
			{
				error = $"'{normalized[4]}' no es una pieza de promocion valida. Usa q, r, b o n.";

				return false;
			}
		}

		var legalMoves = LegalMoves();

		var chosen = legalMoves.FirstOrDefault(
			candidate => candidate.From == from
						 && candidate.To == to
						 && (promotion is null || ReferenceEquals(candidate.Promotion, promotion)));

		if (chosen is null)
		{
			error = DescribeIllegalMove(normalized, from, legalMoves);

			return false;
		}

		if (chosen.IsPromotion && promotion is null)
		{
			error = $"El peon de '{from}' promociona en '{to}'. Indica la pieza, por ejemplo '{from}{to}q'.";

			return false;
		}

		positions.Add(Position.Apply(chosen));
		history.Add(chosen);

		return true;
	}

	/// <summary>Takes back the last move played.</summary>
	public bool Undo()
	{
		if (history.Count == 0)
			return false;

		history.RemoveAt(history.Count - 1);
		positions.RemoveAt(positions.Count - 1);

		return true;
	}

	public string ToFen() => fenSerializer.Serialize(Position);

	public string ToAscii()
	{
		var builder = new StringBuilder();

		for (var rankIndex = 7; rankIndex >= 0; rankIndex--)
		{
			builder.Append(RankModel.FromIndex(rankIndex).Name).Append(" |");

			for (var fileIndex = 0; fileIndex < 8; fileIndex++)
			{
				var placed = Position.PieceAt(rankIndex * 8 + fileIndex);
				builder.Append(' ').Append(placed?.Symbol ?? '.');
			}

			builder.AppendLine();
		}

		builder.AppendLine("   ----------------");
		builder.AppendLine("    a b c d e f g h");

		return builder.ToString();
	}

	private string DescribeIllegalMove(string normalized, CoordinateModel from, IReadOnlyList<MoveModel> legalMoves)
	{
		var placed = Position.PieceAt(from);

		if (placed is null)
			return $"'{normalized}' no es legal: no hay ninguna pieza en '{from}'. Consulta el tablero antes de mover.";

		if (!ReferenceEquals(placed.Color, SideToMove))
			return $"'{normalized}' no es legal: le toca mover a {SideToMove} y la pieza de '{from}' es de {placed.Color}.";

		var fromThisSquare = legalMoves.Where(candidate => candidate.From == from).Select(candidate => candidate.ToLongAlgebraic()).ToArray();

		if (fromThisSquare.Length == 0)
			return $"'{normalized}' no es legal. La pieza de '{from}' no tiene ningun movimiento legal en esta posicion.";

		return $"'{normalized}' no es legal. Movimientos legales de la pieza de '{from}': {string.Join(", ", fromThisSquare)}.";
	}
}
