namespace ChessSDK.UnitTests.Models.ChessConcepts.Formatters;

using AwesomeAssertions;
using ChessSDK.Models.Boards;
using ChessSDK.Models.ChessConcepts;
using ChessSDK.Models.ChessConcepts.Formatters;
using ChessSDK.Notation;
using ChessSDK.Rules;

/// <summary>
/// Tests of the algebraic notation written with the position in hand: disambiguation, castling
/// and the check and checkmate marks.
/// </summary>
[TestClass]
public class SanFormatterTests
{
	private static readonly FenSerializer fenSerializer = new();
	private static readonly LegalityValidator legalityValidator = new();

	private static MoveModel MoveOf(PositionModel position, string from, string to)
	{
		CoordinateModel origin = from;
		CoordinateModel target = to;

		return legalityValidator.GenerateLegal(position)
								.Single(candidate => candidate.From == origin && candidate.To == target);
	}

	[TestMethod]
	public void GivenTwoKnightsReachingTheSameSquare_WhenFormatted_ThenTheFileDisambiguates()
	{
		// Arrange
		var position = fenSerializer.Deserialize("4k3/8/8/8/8/5N2/8/1N2K3 w - - 0 1");
		var formatter = new EnglishSanFormatter();
		var move = MoveOf(position, "b1", "d2");

		// Act
		var san = formatter.Format(move, position);

		// Assert
		san.Should().Be("Nbd2");
	}

	[TestMethod]
	public void GivenTwoRooksOnTheSameFile_WhenFormatted_ThenTheRankDisambiguates()
	{
		// Arrange
		var position = fenSerializer.Deserialize("4k3/8/8/R7/8/8/R7/4K3 w - - 0 1");
		var formatter = new EnglishSanFormatter();
		var move = MoveOf(position, "a5", "a3");

		// Act
		var san = formatter.Format(move, position);

		// Assert
		san.Should().Be("R5a3");
	}

	[TestMethod]
	public void GivenOnlyOnePieceThatCanReachTheSquare_WhenFormatted_ThenThereIsNoDisambiguation()
	{
		// Arrange
		var position = PositionModel.StartingPosition;
		var formatter = new EnglishSanFormatter();
		var move = MoveOf(position, "g1", "f3");

		// Act
		var san = formatter.Format(move, position);

		// Assert
		san.Should().Be("Nf3");
	}

	[TestMethod]
	public void GivenACastlingMove_WhenFormatted_ThenTheStandardSymbolIsUsed()
	{
		// Arrange
		var position = fenSerializer.Deserialize("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
		var formatter = new EnglishSanFormatter();

		// Act
		var shortCastle = formatter.Format(MoveOf(position, "e1", "g1"), position);
		var longCastle = formatter.Format(MoveOf(position, "e1", "c1"), position);

		// Assert
		shortCastle.Should().Be("O-O");
		longCastle.Should().Be("O-O-O");
	}

	[TestMethod]
	public void GivenAMoveThatGivesCheck_WhenFormatted_ThenItEndsWithAPlusSign()
	{
		// Arrange
		var position = fenSerializer.Deserialize("4k3/8/8/8/8/8/8/R3K3 w Q - 0 1");
		var formatter = new EnglishSanFormatter();
		var move = MoveOf(position, "a1", "a8");

		// Act
		var san = formatter.Format(move, position);

		// Assert
		san.Should().Be("Ra8+");
	}

	[TestMethod]
	public void GivenAMatingMove_WhenFormatted_ThenItEndsWithAHash()
	{
		// Arrange
		var position = fenSerializer.Deserialize("6k1/5ppp/8/8/8/8/8/R5K1 w - - 0 1");
		var formatter = new EnglishSanFormatter();
		var move = MoveOf(position, "a1", "a8");

		// Act
		var san = formatter.Format(move, position);

		// Assert
		san.Should().Be("Ra8#");
	}

	[TestMethod]
	public void GivenAnEnPassantCapture_WhenFormatted_ThenTheOriginFileIsWritten()
	{
		// Arrange
		var position = fenSerializer.Deserialize("4k3/8/8/3pP3/8/8/8/4K3 w - d6 0 1");
		var formatter = new EnglishSanFormatter();
		var move = MoveOf(position, "e5", "d6");

		// Act
		var san = formatter.Format(move, position);

		// Assert
		san.Should().Be("exd6");
	}

	[TestMethod]
	public void GivenTheSpanishDialect_WhenAKnightMoves_ThenTheSpanishLetterIsUsed()
	{
		// Arrange
		var position = PositionModel.StartingPosition;
		var formatter = new SpanishSanFormatter();
		var move = MoveOf(position, "g1", "f3");

		// Act
		var san = formatter.Format(move, position);

		// Assert
		san.Should().Be("Cf3");
	}

	[TestMethod]
	public void GivenAGame_WhenTheHistoryIsFormattedFromTheStartingPosition_ThenItIsNumberedAndAnnotated()
	{
		// Arrange
		var session = new GameSessionModel("history", GameColorModel.White);

		foreach (var move in new[] { "e2e4", "e7e5", "g1f3", "b8c6", "f1b5" })
			session.TryApplyMove(move, out _).Should().BeTrue();

		var historyFormatter = new MoveHistoryFormatter(new EnglishSanFormatter());

		// Act
		var text = historyFormatter.Format(session.History, PositionModel.StartingPosition);

		// Assert
		text.Should().Be("1. e4 e5 2. Nf3 Nc6 3. Bb5");
	}
}
