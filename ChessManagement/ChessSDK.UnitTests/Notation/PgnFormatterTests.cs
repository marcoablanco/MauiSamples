namespace ChessSDK.UnitTests.Notation;

using AwesomeAssertions;
using ChessSDK.Enums;
using ChessSDK.Models.Boards;
using ChessSDK.Models.ChessConcepts;
using ChessSDK.Notation;

[TestClass]
public class PgnFormatterTests
{
	private static GameSessionModel PlayOperaGame()
	{
		var session = new GameSessionModel("opera", GameColorModel.White);

		foreach (var san in SanParserTests.OperaGame)
			session.TryApplyMove(san, out var error).Should().BeTrue($"'{san}': {error}");

		return session;
	}

	[TestMethod]
	public void GivenAFinishedGame_WhenExported_ThenTheSevenTagsAndTheResultAreWritten()
	{
		// Arrange
		var session = PlayOperaGame();

		var headers = new PgnHeadersModel
		{
			Event = "Paris",
			Site = "Paris FRA",
			Date = "1858.11.02",
			Round = "?",
			White = "Morphy, Paul",
			Black = "Duke of Brunswick and Count Isouard"
		};

		var formatter = new PgnFormatter();

		// Act
		var pgn = formatter.Format(session, headers);

		// Assert
		pgn.Should().Contain("[Event \"Paris\"]");
		pgn.Should().Contain("[Date \"1858.11.02\"]");
		pgn.Should().Contain("[White \"Morphy, Paul\"]");
		pgn.Should().Contain("[Result \"1-0\"]");
		pgn.Should().NotContain("[SetUp");
	}

	[TestMethod]
	public void GivenAFinishedGame_WhenExported_ThenTheMoveTextIsNumberedSanAndEndsWithTheResult()
	{
		// Arrange
		var session = PlayOperaGame();
		var formatter = new PgnFormatter();

		// Act
		var moveText = formatter.Format(session).Split("\r\n\r\n", StringSplitOptions.None)[1].Replace("\r\n", " ");

		// Assert
		moveText.Should().StartWith("1. e4 e5 2. Nf3 d6 3. d4 Bg4");
		moveText.Should().Contain("12. O-O-O Rd8");
		moveText.Should().EndWith("17. Rd8# 1-0");
	}

	[TestMethod]
	public void GivenALongGame_WhenExported_ThenNoLineGoesOverEightyCharacters()
	{
		// Arrange
		var session = PlayOperaGame();
		var formatter = new PgnFormatter();

		// Act
		var lines = formatter.Format(session).Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

		// Assert
		lines.Should().OnlyContain(line => line.Length <= 80);
	}

	[TestMethod]
	public void GivenAGameStartedFromAFen_WhenExported_ThenTheSetUpTagsAreWritten()
	{
		// Arrange
		var start = new FenSerializer().Deserialize("4k3/8/8/8/8/8/4P3/4K3 w - - 0 40");
		var formatter = new PgnFormatter();

		// Act
		var pgn = formatter.Format([], start, GameResultEnum.InProgress);

		// Assert
		pgn.Should().Contain("[SetUp \"1\"]");
		pgn.Should().Contain("[FEN \"4k3/8/8/8/8/8/4P3/4K3 w - - 0 40\"]");
		pgn.Should().Contain("[Result \"*\"]");
		pgn.TrimEnd().Should().EndWith("*");
	}

	[TestMethod]
	public void GivenAGameWhereBlackMovesFirst_WhenExported_ThenTheFirstMoveNumberUsesEllipsis()
	{
		// Arrange
		var start = new FenSerializer().Deserialize("4k3/4p3/8/8/8/8/4P3/4K3 b - - 0 40");
		var parser = new SanParser();
		var formatter = new PgnFormatter();

		parser.TryParse(start, "e5", out var blackMove, out var error).Should().BeTrue(error);

		// Act
		var pgn = formatter.Format([blackMove], start, GameResultEnum.InProgress);

		// Assert
		pgn.Should().Contain("40... e5 *");
	}

	[TestMethod]
	public void GivenACheckmate_WhenTheResultIsFormatted_ThenTheMatedSideLoses()
	{
		// Arrange, Act, Assert
		PgnFormatter.FormatResult(GameResultEnum.Checkmate, GameColorModel.Black).Should().Be("1-0");
		PgnFormatter.FormatResult(GameResultEnum.Checkmate, GameColorModel.White).Should().Be("0-1");
		PgnFormatter.FormatResult(GameResultEnum.Stalemate, GameColorModel.White).Should().Be("1/2-1/2");
		PgnFormatter.FormatResult(GameResultEnum.ThreefoldRepetition, GameColorModel.Black).Should().Be("1/2-1/2");
		PgnFormatter.FormatResult(GameResultEnum.InProgress, GameColorModel.White).Should().Be("*");
	}
}
