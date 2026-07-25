namespace ChessSDK.Notation;

using ChessSDK.Models.Boards;
using ChessSDK.Models.ChessConcepts;
using ChessSDK.Models.ChessConcepts.Formatters;
using ChessSDK.Rules;

/// <summary>
/// Turns a move written by a human (or by a language model) into a <see cref="MoveModel" />.
/// It never tries to understand the notation on its own: it generates the legal moves of the
/// position, writes every one of them down, and looks for the one that matches. That way an
/// illegal or ambiguous move can never get through, and the parser can accept several dialects
/// at once without duplicating the notation rules.
/// </summary>
public sealed class SanParser
{
	private readonly LegalityValidator legalityValidator;
	private readonly IMoveNotationFormatter sanFormatter;
	private readonly LanFormatter lanFormatter = new();

	public SanParser()
		: this(new EnglishSanFormatter(), new LegalityValidator())
	{
	}

	public SanParser(IMoveNotationFormatter sanFormatter)
		: this(sanFormatter, new LegalityValidator())
	{
	}

	public SanParser(IMoveNotationFormatter sanFormatter, LegalityValidator legalityValidator)
	{
		ArgumentNullException.ThrowIfNull(sanFormatter);
		ArgumentNullException.ThrowIfNull(legalityValidator);

		this.sanFormatter = sanFormatter;
		this.legalityValidator = legalityValidator;
	}

	public MoveModel Parse(PositionModel position, string text)
	{
		if (!TryParse(position, text, out var move, out var error))
			throw new FormatException(error);

		return move;
	}

	public bool TryParse(PositionModel position, string? text, out MoveModel move, out string error)
	{
		ArgumentNullException.ThrowIfNull(position);

		move = null!;
		error = string.Empty;

		if (string.IsNullOrWhiteSpace(text))
		{
			error = "El movimiento no puede estar vacio.";

			return false;
		}

		var wanted = Normalize(text);
		var legalMoves = legalityValidator.GenerateLegal(position);
		var matches = new List<MoveModel>(2);

		foreach (var candidate in legalMoves)
			if (WritingsOf(candidate, position).Contains(wanted))
				matches.Add(candidate);

		if (matches.Count == 1)
		{
			move = matches[0];

			return true;
		}

		error = matches.Count == 0
					? $"'{text.Trim()}' no es un movimiento legal en esta posicion. Movimientos legales: {Describe(legalMoves)}."
					: $"'{text.Trim()}' es ambiguo. Puede ser: {Describe(matches)}.";

		return false;
	}

	/// <summary>
	/// Removes everything that carries no information: check and annotation marks, the en passant
	/// suffix, whitespace, and the zeros some people write castling with.
	/// </summary>
	private static string Normalize(string text)
	{
		var cleaned = text.Trim().Replace("e.p.", string.Empty, StringComparison.OrdinalIgnoreCase);

		cleaned = new string(cleaned.Where(letter => !char.IsWhiteSpace(letter) && letter is not ('+' or '#' or '!' or '?')).ToArray());

		if (cleaned.Length > 0 && cleaned.All(letter => letter is '0' or 'O' or 'o' or '-'))
			cleaned = cleaned.Replace('0', 'O').Replace('o', 'O');

		return cleaned;
	}

	private static string Describe(IReadOnlyList<MoveModel> moves)
		=> string.Join(", ", moves.Select(candidate => candidate.ToLongAlgebraic()));

	/// <summary>Every accepted way of writing a move down.</summary>
	private HashSet<string> WritingsOf(MoveModel move, PositionModel position)
	{
		var san = Normalize(sanFormatter.Format(move, position));
		var longAlgebraic = move.ToLongAlgebraic();

		var writings = new HashSet<string>(StringComparer.Ordinal)
		{
			san,
			san.Replace("=", string.Empty),
			longAlgebraic,
			Normalize(lanFormatter.Format(move))
		};

		// Written without the position, the formatter skips the disambiguation. Keeping that
		// looser form means "Nd2" is reported as ambiguous instead of as illegal.
		var loose = Normalize(sanFormatter.Format(move));

		writings.Add(loose);
		writings.Add(loose.Replace("=", string.Empty));

		if (move.IsPromotion)
			writings.Add($"{move.From}{move.To}{move.Promotion!.Symbol}");

		if (move.IsCastle)
		{
			// Castling is also a king move, so "e1g1" has to be accepted as well.
			writings.Add($"{move.From}{move.To}");
			writings.Add(move.Kind == Enums.MoveKindEnum.CastleKingSide ? "OO" : "OOO");
		}

		return writings;
	}
}
