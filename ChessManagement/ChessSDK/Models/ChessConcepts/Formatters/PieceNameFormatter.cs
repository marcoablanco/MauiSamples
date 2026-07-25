namespace ChessSDK.Models.ChessConcepts.Formatters;

using ChessSDK.Models.Boards;

/// <summary>
/// Full spanish name of a piece, for wording that reads better than a bare letter:
/// "Movimientos legales del caballo de g1".
/// </summary>
public sealed class PieceNameFormatter
{
	public string Format(PieceModel piece)
	{
		ArgumentNullException.ThrowIfNull(piece);

		return piece.Symbol switch
			   {
				   'P' => "peon",
				   'N' => "caballo",
				   'B' => "alfil",
				   'R' => "torre",
				   'Q' => "dama",
				   'K' => "rey",
				   _ => piece.ToString()
			   };
	}

	/// <summary>Same name in plural, to head a group of moves.</summary>
	public string FormatPlural(PieceModel piece)
	{
		ArgumentNullException.ThrowIfNull(piece);

		return piece.Symbol switch
			   {
				   'P' => "peones",
				   'N' => "caballos",
				   'B' => "alfiles",
				   'R' => "torres",
				   'Q' => "damas",
				   'K' => "rey",
				   _ => piece.ToString()
			   };
	}

	/// <summary>
	/// Name with its definite article, because "torre" and "dama" are feminine in spanish
	/// and "el torre" would read wrong.
	/// </summary>
	public string FormatWithArticle(PieceModel piece)
	{
		ArgumentNullException.ThrowIfNull(piece);

		var isFeminine = piece.Symbol is 'R' or 'Q';

		return $"{(isFeminine ? "La" : "El")} {Format(piece)}";
	}
}
