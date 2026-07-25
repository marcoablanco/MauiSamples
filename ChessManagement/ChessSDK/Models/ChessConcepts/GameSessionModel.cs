namespace ChessSDK.Models.ChessConcepts;

using System.Text;
using ChessSDK.Models.Boards;

/// <summary>
/// Mutable state of a single chess game.
/// NOTE: legality is only checked at a basic level (origin occupancy, turn and own-piece capture).
/// Full move generation / check detection is still pending (see PLAN.md).
/// </summary>
public sealed class GameSessionModel
{
	private readonly Dictionary<string, PlacedPieceModel> board = new(StringComparer.Ordinal);
	private readonly List<MoveModel> history = new();

	public GameSessionModel(string id, GameColorModel humanColor)
	{
		Id = id;
		HumanColor = humanColor;
		Reset();
	}

	public string Id { get; }
	public GameColorModel HumanColor { get; }
	public GameColorModel SideToMove { get; private set; } = GameColorModel.White;
	public int FullMoveNumber { get; private set; } = 1;
	public int HalfMoveClock { get; private set; }
	public IReadOnlyList<MoveModel> History => history;

	private static bool IsValidSquare(string square)
		=> square.Length == 2 && square[0] is >= 'a' and <= 'h' && square[1] is >= '1' and <= '8';

	private static PieceModel? ParsePromotion(char letter)
		=> letter switch
		   {
			   'q' or 'd' => PieceModel.Queen,
			   'r' or 't' => PieceModel.Rook,
			   'b' or 'a' => PieceModel.Bishop,
			   'n' or 'c' => PieceModel.Knight,
			   _ => null
		   };

	private static char ToFenLetter(PlacedPieceModel placed)
	{
		var letter = placed.Piece switch
					 {
						 _ when ReferenceEquals(placed.Piece, PieceModel.Pawn)   => 'p',
						 _ when ReferenceEquals(placed.Piece, PieceModel.Knight) => 'n',
						 _ when ReferenceEquals(placed.Piece, PieceModel.Bishop) => 'b',
						 _ when ReferenceEquals(placed.Piece, PieceModel.Rook)   => 'r',
						 _ when ReferenceEquals(placed.Piece, PieceModel.Queen)  => 'q',
						 _ => 'k'
					 };

		return ReferenceEquals(placed.Color, GameColorModel.White) ? char.ToUpperInvariant(letter) : letter;
	}

	public void Reset()
	{
		board.Clear();
		history.Clear();
		SideToMove = GameColorModel.White;
		FullMoveNumber = 1;
		HalfMoveClock = 0;

		PlaceBackRank('1', GameColorModel.White);
		PlacePawnRank('2', GameColorModel.White);
		PlacePawnRank('7', GameColorModel.Black);
		PlaceBackRank('8', GameColorModel.Black);
	}

	public PlacedPieceModel? PieceAt(string square)
		=> board.GetValueOrDefault(square);

	/// <summary>
	/// Applies a move expressed in long algebraic form: "e2e4", "e7e8q".
	/// </summary>
	public bool TryApplyMove(string move, out string error)
	{
		error = string.Empty;

		if (string.IsNullOrWhiteSpace(move))
		{
			error = "El movimiento no puede estar vacio. Usa notacion larga, por ejemplo 'e2e4'.";
			return false;
		}

		var normalized = move.Trim().Replace("-", "").Replace("x", "").ToLowerInvariant();

		if (normalized.Length is not (4 or 5))
		{
			error = $"'{move}' no tiene formato valido. Usa origen+destino, por ejemplo 'e2e4' o 'e7e8q'.";
			return false;
		}

		var from = normalized[..2];
		var to = normalized[2..4];

		if (!IsValidSquare(from))
		{
			error = $"La casilla de origen '{from}' no existe.";
			return false;
		}

		if (!IsValidSquare(to))
		{
			error = $"La casilla de destino '{to}' no existe.";
			return false;
		}

		if (from == to)
		{
			error = "El origen y el destino no pueden ser la misma casilla.";
			return false;
		}

		if (!board.TryGetValue(from, out var moving))
		{
			error = $"No hay ninguna pieza en '{from}'. Consulta el tablero antes de mover.";
			return false;
		}

		if (!ReferenceEquals(moving.Color, SideToMove))
		{
			error = $"Le toca mover a {SideToMove}, pero la pieza de '{from}' es de {moving.Color}.";
			return false;
		}

		var target = board.GetValueOrDefault(to);

		if (target is not null && ReferenceEquals(target.Color, moving.Color))
		{
			error = $"No puedes capturar tu propia pieza en '{to}'.";
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

		board.Remove(from);
		board[to] = new PlacedPieceModel(promotion ?? moving.Piece, moving.Color);
		history.Add(new MoveModel(moving.Piece, from, to, target?.Piece, promotion));

		HalfMoveClock = target is not null || ReferenceEquals(moving.Piece, PieceModel.Pawn) ? 0 : HalfMoveClock + 1;

		if (ReferenceEquals(SideToMove, GameColorModel.Black))
			FullMoveNumber++;

		SideToMove = ReferenceEquals(SideToMove, GameColorModel.White) ? GameColorModel.Black : GameColorModel.White;

		return true;
	}

	public string ToFen()
	{
		var builder = new StringBuilder();

		for (var rankIndex = BoardModel.AllRanks.Length - 1; rankIndex >= 0; rankIndex--)
		{
			var rank = BoardModel.AllRanks[rankIndex];
			var empty = 0;

			foreach (var file in BoardModel.AllFiles)
			{
				var piece = board.GetValueOrDefault($"{file}{rank}");

				if (piece is null)
				{
					empty++;
					continue;
				}

				if (empty > 0)
				{
					builder.Append(empty);
					empty = 0;
				}

				builder.Append(ToFenLetter(piece));
			}

			if (empty > 0)
				builder.Append(empty);

			if (rankIndex > 0)
				builder.Append('/');
		}

		var side = ReferenceEquals(SideToMove, GameColorModel.White) ? 'w' : 'b';

		return $"{builder} {side} {CastlingRights()} - {HalfMoveClock} {FullMoveNumber}";
	}

	public string ToAscii()
	{
		var builder = new StringBuilder();

		for (var rankIndex = BoardModel.AllRanks.Length - 1; rankIndex >= 0; rankIndex--)
		{
			var rank = BoardModel.AllRanks[rankIndex];
			builder.Append(rank.ToString()).Append(" |");

			foreach (var file in BoardModel.AllFiles)
			{
				var piece = board.GetValueOrDefault($"{file}{rank}");
				builder.Append(' ').Append(piece is null ? '.' : ToFenLetter(piece));
			}

			builder.AppendLine();
		}

		builder.AppendLine("   ----------------");
		builder.AppendLine("    a b c d e f g h");

		return builder.ToString();
	}


	private void PlaceBackRank(char rank, GameColorModel color)
	{
		PieceModel[] order =
		{
			PieceModel.Rook, PieceModel.Knight, PieceModel.Bishop, PieceModel.Queen,
			PieceModel.King, PieceModel.Bishop, PieceModel.Knight, PieceModel.Rook
		};

		for (var i = 0; i < order.Length; i++)
			board[$"{BoardModel.AllFiles[i]}{rank}"] = new PlacedPieceModel(order[i], color);
	}

	private void PlacePawnRank(char rank, GameColorModel color)
	{
		foreach (var file in BoardModel.AllFiles)
			board[$"{file}{rank}"] = new PlacedPieceModel(PieceModel.Pawn, color);
	}

	/// <summary>
	/// Heuristic castling rights: king and rook still on their home squares.
	/// Will be replaced once ChessSDK tracks real castling state.
	/// </summary>
	private string CastlingRights()
	{
		var rights = new StringBuilder();

		if (IsHome("e1", PieceModel.King, GameColorModel.White))
		{
			if (IsHome("h1", PieceModel.Rook, GameColorModel.White)) rights.Append('K');
			if (IsHome("a1", PieceModel.Rook, GameColorModel.White)) rights.Append('Q');
		}

		if (IsHome("e8", PieceModel.King, GameColorModel.Black))
		{
			if (IsHome("h8", PieceModel.Rook, GameColorModel.Black)) rights.Append('k');
			if (IsHome("a8", PieceModel.Rook, GameColorModel.Black)) rights.Append('q');
		}

		return rights.Length == 0 ? "-" : rights.ToString();
	}

	private bool IsHome(string square, PieceModel piece, GameColorModel color)
	{
		var placed = board.GetValueOrDefault(square);

		return placed is not null && ReferenceEquals(placed.Piece, piece) && ReferenceEquals(placed.Color, color);
	}
}

