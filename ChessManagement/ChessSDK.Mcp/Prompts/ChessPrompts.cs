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

			El servidor es la unica fuente de verdad del estado de la partida. Tu memoria del
			tablero no vale: consultalo siempre antes de decidir.

			Reglas de trabajo, sin excepciones:
			1. Si no hay partida activa, llama a new_game.
			2. Antes de cada jugada tuya, llama a get_position para ver el FEN y el tablero reales.
			3. Despues llama SIEMPRE a get_legal_moves y elige tu jugada de esa lista. No inventes
			   movimientos ni los deduzcas de memoria: si no esta en la lista, no es legal.
			4. Mueve con make_move. Acepta notacion algebraica ('Nf3', 'exd5', 'O-O', 'e8=Q') y
			   larga ('g1f3', 'e7e8q').
			5. Si make_move devuelve ERROR, la jugada NO se ha aplicado. Vuelve a get_legal_moves
			   y elige otra de la lista. Nunca supongas que se aplico.
			6. Comprueba el campo Estado de la respuesta: si indica mate, tablas o abandono, la
			   partida ha terminado y no debes seguir moviendo.
			7. Tras mover, resume tu plan en una frase.

			Empieza saludando y preguntandome con que color quiero jugar.
			""";

	[McpServerPrompt(Name = "analyze_position")]
	[Description("Pide un analisis de la posicion actual de una partida.")]
	public static string AnalyzePosition(
		[Description("Identificador de la partida a analizar.")] string gameId)
		=> $"""
			Analiza la partida {gameId}.
			Usa get_position para obtener el FEN y el estado, get_history para ver la secuencia de
			jugadas y get_legal_moves para saber que se puede jugar de verdad.
			Comenta: material, seguridad de los reyes, estructura de peones y los tres mejores planes para el bando que mueve.
			""";
}

