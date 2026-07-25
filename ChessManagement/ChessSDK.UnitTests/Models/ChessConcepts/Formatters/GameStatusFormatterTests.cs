namespace ChessSDK.UnitTests.Models.ChessConcepts.Formatters;

using AwesomeAssertions;
using ChessSDK.Models.ChessConcepts;
using ChessSDK.Models.ChessConcepts.Formatters;

[TestClass]
public class GameStatusFormatterTests
{
	[TestMethod]
	public void GivenTheStartingPosition_WhenFormatted_ThenCheckAndMoveCountAreShown()
	{
		// Arrange
		var formatter = new GameStatusFormatter();
		var session = new GameSessionModel("abc", GameColorModel.White);

		// Act
		var text = formatter.Format(session);

		// Assert
		text.Should().Be("Estado: en juego | Jaque: no | Movimientos legales: 20");
	}

	[TestMethod]
	public void GivenAPositionInCheck_WhenFormatted_ThenTheCheckIsReported()
	{
		// Arrange
		var formatter = new GameStatusFormatter();
		var session = new GameSessionModel("abc", GameColorModel.White);

		foreach (var move in new[] { "e2e4", "e7e5", "f1c4", "b8c6", "c4f7" })
			session.TryApplyMove(move, out _).Should().BeTrue();

		// Act
		var text = formatter.Format(session);

		// Assert
		text.Should().StartWith("Estado: en juego | Jaque: si | Movimientos legales: ");
	}

	[TestMethod]
	public void GivenAFinishedGame_WhenFormatted_ThenOnlyTheOutcomeIsShown()
	{
		// Arrange
		var formatter = new GameStatusFormatter();
		var session = new GameSessionModel("abc", GameColorModel.White);

		foreach (var move in new[] { "f2f3", "e7e5", "g2g4", "d8h4" })
			session.TryApplyMove(move, out _).Should().BeTrue();

		// Act
		var text = formatter.Format(session);

		// Assert
		text.Should().Be("Estado: jaque mate, ganan las negras");
		text.Should().NotContain("Jaque:");
	}
}
