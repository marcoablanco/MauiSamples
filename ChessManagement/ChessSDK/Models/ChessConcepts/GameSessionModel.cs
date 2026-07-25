namespace ChessSDK.Models.ChessConcepts;

using System.Text;
using ChessSDK.Enums;
using ChessSDK.Models.Boards;
using ChessSDK.Models.ChessConcepts.Formatters;
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
	private static readonly SanParser sanParser = new();
	private static readonly GameResultFormatter gameResultFormatter = new();

	private readonly List<MoveModel> history = new();
	private readonly List<PositionModel> positions = new();

	private GameColorModel? resignedBy;

	public GameSessionModel(string id, GameColorModel humanColor)
	{
		Id = id;
		HumanColor = humanColor;
		Reset();
	}

	public string Id { get; }

	public GameColorModel HumanColor { get; }

	public PositionModel Position => positions[^1];

	/// <summary>Position the game started from. Needed to replay the history into notation.</summary>
	public PositionModel StartingPosition => positions[0];

	public GameColorModel SideToMove => Position.SideToMove;

	public int FullMoveNumber => Position.FullMoveNumber;

	public int HalfMoveClock => Position.HalfMoveClock;

	public IReadOnlyList<MoveModel> History => history;

	public bool IsInCheck => legalityValidator.IsInCheck(Position);

	/// <summary>Side that gave the game up, or null if nobody did.</summary>
	public GameColorModel? ResignedBy => resignedBy;

	/// <summary>Side that won, or null if the game is unfinished or drawn.</summary>
	public GameColorModel? Winner
		=> Result switch
		   {
			   GameResultEnum.Checkmate => SideToMove.Opposite,
			   GameResultEnum.Resigned => resignedBy!.Opposite,
			   _ => null
		   };

	public GameResultEnum Result
		=> resignedBy is not null
			   ? GameResultEnum.Resigned
			   : resultEvaluator.Evaluate(Position, positions);

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
		resignedBy = null;
	}

	/// <summary>
	/// Gives the game up for one side. The game is over from that moment on, and the position is
	/// kept exactly as it was so the history and the export still make sense.
	/// </summary>
	public bool TryResign(GameColorModel color, out string error)
	{
		ArgumentNullException.ThrowIfNull(color);

		error = string.Empty;

		if (IsOver)
		{
			error = $"La partida ya ha terminado ({gameResultFormatter.Format(this)}). Nadie puede abandonarla.";

			return false;
		}

		resignedBy = color;

		return true;
	}

	public IReadOnlyList<MoveModel> LegalMoves() => legalityValidator.GenerateLegal(Position);

	public IReadOnlyList<MoveModel> LegalMovesFrom(CoordinateModel from)
	{
		ArgumentNullException.ThrowIfNull(from);

		return LegalMoves().Where(move => move.From == from).ToArray();
	}

	public PlacedPieceModel? PieceAt(string square)
		=> CoordinateModel.TryParse(square, out var coordinate) ? Position.PieceAt(coordinate) : null;

	public PlacedPieceModel? PieceAt(CoordinateModel square)
	{
		ArgumentNullException.ThrowIfNull(square);

		return Position.PieceAt(square);
	}

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
			error = $"La partida ya ha terminado ({gameResultFormatter.Format(this)}). No se pueden aplicar mas movimientos.";

			return false;
		}

		var normalized = move.Trim().Replace("-", "").Replace("x", "").ToLowerInvariant();

		if (!LooksLongAlgebraic(normalized))
			return TryApplyNotatedMove(move, out error);

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

	/// <summary>
	/// Takes back the last <paramref name="plies" /> moves played and returns how many were
	/// actually undone, which is fewer than asked when the game is shorter than that.
	/// </summary>
	public int Undo(int plies)
	{
		if (plies < 1)
			throw new ArgumentOutOfRangeException(nameof(plies), plies, "At least one move has to be undone.");

		var undone = 0;

		while (undone < plies && Undo())
			undone++;

		return undone;
	}

	/// <summary>
	/// Takes back the last move played. Giving up is deliberate and final, so a resigned game
	/// cannot be rewound: start a new one instead.
	/// </summary>
	public bool Undo()
	{
		if (history.Count == 0 || resignedBy is not null)
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

	/// <summary>
	/// A move looks like long algebraic when it is just two squares (plus an optional promotion
	/// letter). Anything else — "Nf3", "O-O", "exd5" — is handed over to the SAN parser.
	/// </summary>
	private static bool LooksLongAlgebraic(string normalized)
		=> normalized.Length is 4 or 5
		   && char.IsLetter(normalized[0])
		   && char.IsDigit(normalized[1])
		   && char.IsLetter(normalized[2])
		   && char.IsDigit(normalized[3]);

	/// <summary>Applies a move written in algebraic notation: "Nf3", "exd5", "O-O", "e8=Q+".</summary>
	private bool TryApplyNotatedMove(string move, out string error)
	{
		if (!sanParser.TryParse(Position, move, out var chosen, out error))
			return false;

		positions.Add(Position.Apply(chosen));
		history.Add(chosen);

		return true;
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
