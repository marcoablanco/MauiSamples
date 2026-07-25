namespace ChessSDK.Notation;

using System.Text;
using ChessSDK.Models.Boards;
using ChessSDK.Models.ChessConcepts;

/// <summary>
/// Converts a <see cref="PositionModel" /> to and from Forsyth-Edwards notation.
/// </summary>
public sealed class FenSerializer
{
	public const string StartingPositionFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

	public string Serialize(PositionModel position)
	{
		ArgumentNullException.ThrowIfNull(position);

		var builder = new StringBuilder(90);

		for (var rankIndex = 7; rankIndex >= 0; rankIndex--)
		{
			var empty = 0;

			for (var fileIndex = 0; fileIndex < 8; fileIndex++)
			{
				var placed = position.PieceAt(rankIndex * 8 + fileIndex);

				if (placed is null)
				{
					empty++;

					continue;
				}

				if (empty > 0)
				{
					builder.Append(empty);
					empty = 0;
				}

				builder.Append(placed.Symbol);
			}

			if (empty > 0)
				builder.Append(empty);

			if (rankIndex > 0)
				builder.Append('/');
		}

		builder.Append(' ').Append(position.SideToMove.Symbol);
		builder.Append(' ').Append(position.CastlingRights);
		builder.Append(' ').Append(position.EnPassantTarget?.ToString() ?? "-");
		builder.Append(' ').Append(position.HalfMoveClock);
		builder.Append(' ').Append(position.FullMoveNumber);

		return builder.ToString();
	}

	public PositionModel Deserialize(string fen)
	{
		if (!TryDeserialize(fen, out var position, out var error))
			throw new FormatException(error);

		return position;
	}

	/// <summary>
	/// Parses a FEN string. The half move clock and the full move number are optional, because
	/// many published test positions omit them.
	/// </summary>
	public bool TryDeserialize(string? fen, out PositionModel position, out string error)
	{
		position = PositionModel.StartingPosition;
		error = string.Empty;

		if (string.IsNullOrWhiteSpace(fen))
		{
			error = "El FEN no puede estar vacio.";

			return false;
		}

		var fields = fen.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

		if (fields.Length < 4)
		{
			error = $"'{fen}' no es un FEN valido: se esperaban al menos 4 campos.";

			return false;
		}

		if (!TryReadPlacement(fields[0], out var squares, out error))
			return false;

		if (!GameColorModel.TryParse(fields[1], out var sideToMove))
		{
			error = $"'{fields[1]}' no es un turno valido: usa 'w' o 'b'.";

			return false;
		}

		if (!CastlingRightsModel.TryParse(fields[2], out var castlingRights))
		{
			error = $"'{fields[2]}' no son derechos de enroque validos.";

			return false;
		}

		CoordinateModel? enPassantTarget = null;

		if (fields[3] != "-" && !CoordinateModel.TryParse(fields[3], out enPassantTarget))
		{
			error = $"'{fields[3]}' no es una casilla de captura al paso valida.";

			return false;
		}

		var halfMoveClock = 0;

		if (fields.Length > 4 && (!int.TryParse(fields[4], out halfMoveClock) || halfMoveClock < 0))
		{
			error = $"'{fields[4]}' no es un contador de medias jugadas valido.";

			return false;
		}

		var fullMoveNumber = 1;

		if (fields.Length > 5 && (!int.TryParse(fields[5], out fullMoveNumber) || fullMoveNumber < 1))
		{
			error = $"'{fields[5]}' no es un numero de jugada valido.";

			return false;
		}

		position = PositionModel.Create(squares, sideToMove, castlingRights, enPassantTarget, halfMoveClock, fullMoveNumber);

		return true;
	}

	private static bool TryReadPlacement(string placement, out PlacedPieceModel?[] squares, out string error)
	{
		squares = new PlacedPieceModel?[PositionModel.SquareCount];
		error = string.Empty;

		var rows = placement.Split('/');

		if (rows.Length != 8)
		{
			error = $"'{placement}' no describe 8 filas.";

			return false;
		}

		for (var row = 0; row < 8; row++)
		{
			var rankIndex = 7 - row;
			var fileIndex = 0;

			foreach (var symbol in rows[row])
			{
				if (char.IsDigit(symbol))
				{
					fileIndex += symbol - '0';

					continue;
				}

				if (!PlacedPieceModel.TryFromSymbol(symbol, out var placed))
				{
					error = $"'{symbol}' no es una pieza valida en un FEN.";

					return false;
				}

				if (fileIndex > 7)
				{
					error = $"La fila '{rows[row]}' tiene mas de 8 casillas.";

					return false;
				}

				squares[rankIndex * 8 + fileIndex] = placed;
				fileIndex++;
			}

			if (fileIndex != 8)
			{
				error = $"La fila '{rows[row]}' no suma 8 casillas.";

				return false;
			}
		}

		return true;
	}
}
