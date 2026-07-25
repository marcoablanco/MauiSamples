namespace ChessSDK.Mcp.Prompts;

using System.ComponentModel;
using ModelContextProtocol.Server;

[McpServerPromptType]
public sealed class ChessPrompts
{
	[McpServerPrompt(Name = "play_chess")]
	[Description("Configura al asistente para jugar una partida de ajedrez con un estilo concreto.")]
	public static string PlayChess(
		[Description("Estilo de juego: agresivo, posicional o defensivo.")] string style = "posicional")
		=> $"""
			Vas a jugar al ajedrez contra mi con un estilo {style}.
			Reglas de trabajo:
			1. Si no hay partida activa, llama a new_game.
			2. Antes de cada jugada, consulta get_position para conocer el FEN real.
			3. Mueve siempre con make_move usando notacion larga (por ejemplo 'g1f3').
			4. Si make_move devuelve ERROR, corrige la jugada; nunca supongas que se aplico.
			5. Tras mover, resume brevemente tu plan en una frase.
			Empieza saludando y preguntandome con que color quiero jugar.
			""";

	[McpServerPrompt(Name = "analyze_position")]
	[Description("Pide un analisis de la posicion actual de una partida.")]
	public static string AnalyzePosition(
		[Description("Identificador de la partida a analizar.")] string gameId)
		=> $"""
			Analiza la partida {gameId}.
			Usa get_position para obtener el FEN y get_history para ver la secuencia de jugadas.
			Comenta: material, seguridad de los reyes, estructura de peones y los tres mejores planes para el bando que mueve.
			""";
}

