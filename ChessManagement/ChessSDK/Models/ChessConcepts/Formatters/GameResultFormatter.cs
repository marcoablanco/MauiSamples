namespace ChessSDK.Models.ChessConcepts.Formatters;

using ChessSDK.Enums;

/// <summary>
/// Turns the outcome of a game into a sentence a person (or a language model) can read.
/// Presentation of a domain concept, so it lives in the SDK and any front end can reuse it.
/// </summary>
public sealed class GameResultFormatter
{
	/// <summary>Name of a colour in spanish, as the rest of the wording expects it.</summary>
	public static string FormatColor(GameColorModel color)
	{
		ArgumentNullException.ThrowIfNull(color);

		return color.IsWhite ? "blancas" : "negras";
	}

	public string Format(GameSessionModel session)
	{
		ArgumentNullException.ThrowIfNull(session);

		return Format(session.Result, session.Winner);
	}

	public string Format(GameResultEnum result, GameColorModel? winner)
		=> result switch
		   {
			   GameResultEnum.InProgress => "en juego",
			   GameResultEnum.Checkmate => $"jaque mate, ganan las {FormatColor(winner!)}",
			   GameResultEnum.Resigned => $"abandono, ganan las {FormatColor(winner!)}",
			   GameResultEnum.Stalemate => "tablas por rey ahogado",
			   GameResultEnum.InsufficientMaterial => "tablas por material insuficiente",
			   GameResultEnum.ThreefoldRepetition => "tablas por repeticion triple",
			   GameResultEnum.FiftyMoveRule => "tablas por la regla de las 50 jugadas",
			   _ => result.ToString()
		   };
}
