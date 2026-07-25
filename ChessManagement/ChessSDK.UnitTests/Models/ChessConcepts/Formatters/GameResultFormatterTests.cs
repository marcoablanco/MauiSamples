namespace ChessSDK.UnitTests.Models.ChessConcepts.Formatters;

using AwesomeAssertions;
using ChessSDK.Enums;
using ChessSDK.Models.ChessConcepts;
using ChessSDK.Models.ChessConcepts.Formatters;

[TestClass]
public class GameResultFormatterTests
{
	[TestMethod]
	public void GivenAGameInProgress_WhenFormatted_ThenItSaysSo()
	{
		// Arrange
		var formatter = new GameResultFormatter();
		var session = new GameSessionModel("abc", GameColorModel.White);

		// Act
		var text = formatter.Format(session);

		// Assert
		text.Should().Be("en juego");
	}

	[TestMethod]
	public void GivenACheckmate_WhenFormatted_ThenItNamesTheWinner()
	{
		// Arrange
		var formatter = new GameResultFormatter();
		var session = new GameSessionModel("abc", GameColorModel.White);

		foreach (var move in new[] { "f2f3", "e7e5", "g2g4", "d8h4" })
			session.TryApplyMove(move, out _).Should().BeTrue();

		// Act
		var text = formatter.Format(session);

		// Assert
		text.Should().Be("jaque mate, ganan las negras");
	}

	[TestMethod]
	public void GivenAResignation_WhenFormatted_ThenItNamesTheWinner()
	{
		// Arrange
		var formatter = new GameResultFormatter();
		var session = new GameSessionModel("abc", GameColorModel.White);
		session.TryResign(GameColorModel.Black, out _).Should().BeTrue();

		// Act
		var text = formatter.Format(session);

		// Assert
		text.Should().Be("abandono, ganan las blancas");
	}

	[TestMethod]
	public void GivenEveryDraw_WhenFormatted_ThenTheReasonIsExplained()
	{
		// Arrange
		var formatter = new GameResultFormatter();

		// Act & Assert
		formatter.Format(GameResultEnum.Stalemate, null).Should().Be("tablas por rey ahogado");
		formatter.Format(GameResultEnum.InsufficientMaterial, null).Should().Be("tablas por material insuficiente");
		formatter.Format(GameResultEnum.ThreefoldRepetition, null).Should().Be("tablas por repeticion triple");
		formatter.Format(GameResultEnum.FiftyMoveRule, null).Should().Be("tablas por la regla de las 50 jugadas");
	}
}
