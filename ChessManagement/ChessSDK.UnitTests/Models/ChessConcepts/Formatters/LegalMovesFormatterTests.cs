namespace ChessSDK.UnitTests.Models.ChessConcepts.Formatters;

using AwesomeAssertions;
using ChessSDK.Models.ChessConcepts;
using ChessSDK.Models.ChessConcepts.Formatters;

[TestClass]
public class LegalMovesFormatterTests
{
	[TestMethod]
	public void GivenTheStartingPosition_WhenFormatted_ThenTheTwentyMovesAreGroupedByPiece()
	{
		// Arrange
		var formatter = new LegalMovesFormatter();
		var session = new GameSessionModel("abc", GameColorModel.White);

		// Act
		var text = formatter.Format(session);

		// Assert
		text.Should().StartWith("Movimientos legales de White (20):");
		text.Should().Contain("caballos: ");
		text.Should().Contain("peones: ");
		text.Should().Contain("Nf3 (g1f3)");
		text.Should().Contain("e4 (e2e4)");
	}

	[TestMethod]
	public void GivenSeveralKindsOfPiece_WhenFormatted_ThenTheMostValuableGoesFirst()
	{
		// Arrange
		var formatter = new LegalMovesFormatter();
		var session = new GameSessionModel("abc", GameColorModel.White);

		foreach (var move in new[] { "e2e4", "e7e5", "g1f3", "b8c6" })
			session.TryApplyMove(move, out _).Should().BeTrue();

		// Act
		var lines = formatter.Format(session).Split(Environment.NewLine);

		// Assert
		lines[1].Should().StartWith("rey:");
		lines.Should().Contain(line => line.StartsWith("damas:"));
		lines[^1].Should().StartWith("peones:");
	}

	[TestMethod]
	public void GivenAnOriginSquare_WhenFormatted_ThenOnlyThatPieceIsListed()
	{
		// Arrange
		var formatter = new LegalMovesFormatter();
		var session = new GameSessionModel("abc", GameColorModel.White);

		// Act
		var text = formatter.Format(session, "g1");

		// Assert
		text.Should().StartWith("Movimientos legales de White (2):");
		text.Should().Contain("Nf3 (g1f3)");
		text.Should().Contain("Nh3 (g1h3)");
		text.Should().NotContain("peones");
	}

	[TestMethod]
	public void GivenAnEmptySquare_WhenFormatted_ThenItSaysThereIsNoPiece()
	{
		// Arrange
		var formatter = new LegalMovesFormatter();
		var session = new GameSessionModel("abc", GameColorModel.White);

		// Act
		var text = formatter.Format(session, "e5");

		// Assert
		text.Should().Be("No hay ninguna pieza en 'e5'.");
	}

	[TestMethod]
	public void GivenASquareOfTheOtherSide_WhenFormatted_ThenTheTurnIsExplained()
	{
		// Arrange
		var formatter = new LegalMovesFormatter();
		var session = new GameSessionModel("abc", GameColorModel.White);

		// Act
		var text = formatter.Format(session, "e7");

		// Assert
		text.Should().Be("La pieza de 'e7' es de Black y mueven White.");
	}

	[TestMethod]
	public void GivenABlockedPiece_WhenFormatted_ThenItSaysItHasNoLegalMoves()
	{
		// Arrange
		var formatter = new LegalMovesFormatter();
		var session = new GameSessionModel("abc", GameColorModel.White);

		// Act
		var text = formatter.Format(session, "a1");

		// Assert
		text.Should().Be("La torre de 'a1' no tiene ningun movimiento legal.");
	}

	[TestMethod]
	public void GivenASquareThatDoesNotExist_WhenFormatted_ThenItIsRejectedWithHelp()
	{
		// Arrange
		var formatter = new LegalMovesFormatter();
		var session = new GameSessionModel("abc", GameColorModel.White);

		// Act
		var text = formatter.Format(session, "z9");

		// Assert
		text.Should().Contain("'z9' no existe");
		text.Should().Contain("g1");
	}

	[TestMethod]
	public void GivenNoOriginSquare_WhenFormatted_ThenEveryMoveIsListed()
	{
		// Arrange
		var formatter = new LegalMovesFormatter();
		var session = new GameSessionModel("abc", GameColorModel.White);

		// Act
		var text = formatter.Format(session, null);

		// Assert
		text.Should().StartWith("Movimientos legales de White (20):");
	}

	[TestMethod]
	public void GivenAFinishedGame_WhenFormatted_ThenTheOutcomeIsReportedInstead()
	{
		// Arrange
		var formatter = new LegalMovesFormatter();
		var session = new GameSessionModel("abc", GameColorModel.White);

		foreach (var move in new[] { "f2f3", "e7e5", "g2g4", "d8h4" })
			session.TryApplyMove(move, out _).Should().BeTrue();

		// Act
		var text = formatter.Format(session);

		// Assert
		text.Should().Be("La partida ha terminado: jaque mate, ganan las negras. No hay movimientos legales.");
	}

	[TestMethod]
	public void GivenAFinishedGame_WhenAnOriginSquareIsGiven_ThenTheOutcomeIsStillReported()
	{
		// Arrange
		var formatter = new LegalMovesFormatter();
		var session = new GameSessionModel("abc", GameColorModel.White);
		session.TryResign(GameColorModel.White, out _).Should().BeTrue();

		// Act
		var text = formatter.Format(session, "g1");

		// Assert
		text.Should().Contain("La partida ha terminado");
		text.Should().Contain("abandono");
	}

	[TestMethod]
	public void GivenTheSpanishDialect_WhenFormatted_ThenTheMovesUseSpanishLetters()
	{
		// Arrange
		var formatter = new LegalMovesFormatter(new SpanishSanFormatter());
		var session = new GameSessionModel("abc", GameColorModel.White);

		// Act
		var text = formatter.Format(session, "g1");

		// Assert
		text.Should().Contain("Cf3 (g1f3)");
	}
}
