namespace ChessSDK.UnitTests;

using AwesomeAssertions;
using ChessSDK.Enums;
using ChessSDK.Models.Boards;
using ChessSDK.Models.ChessConcepts;
using ChessSDK.Models.ChessConcepts.Formatters;
using ChessSDK.Notation;

[TestClass]
public sealed class BoardAsciiFormatterTests
{
	private static readonly FenSerializer fenSerializer = new();

	private readonly BoardAsciiFormatter formatter = new();

	[TestMethod]
	public void GivenStartingPosition_WhenFormatting_ThenItMatchesTheExpectedDrawing()
	{
		// Arrange
		var position = PositionModel.StartingPosition;

		var expected = string.Join(Environment.NewLine,
								   "    a   b   c   d   e   f   g   h",
								   "  ┌───┬───┬───┬───┬───┬───┬───┬───┐",
								   "8 │ t │ c │ a │ d │ r │ a │ c │ t │ 8",
								   "  ├───┼───┼───┼───┼───┼───┼───┼───┤",
								   "7 │ p │ p │ p │ p │ p │ p │ p │ p │ 7",
								   "  ├───┼───┼───┼───┼───┼───┼───┼───┤",
								   "6 │   │   │   │   │   │   │   │   │ 6",
								   "  ├───┼───┼───┼───┼───┼───┼───┼───┤",
								   "5 │   │   │   │   │   │   │   │   │ 5",
								   "  ├───┼───┼───┼───┼───┼───┼───┼───┤",
								   "4 │   │   │   │   │   │   │   │   │ 4",
								   "  ├───┼───┼───┼───┼───┼───┼───┼───┤",
								   "3 │   │   │   │   │   │   │   │   │ 3",
								   "  ├───┼───┼───┼───┼───┼───┼───┼───┤",
								   "2 │ P │ P │ P │ P │ P │ P │ P │ P │ 2",
								   "  ├───┼───┼───┼───┼───┼───┼───┼───┤",
								   "1 │ T │ C │ A │ D │ R │ A │ C │ T │ 1",
								   "  └───┴───┴───┴───┴───┴───┴───┴───┘",
								   "    a   b   c   d   e   f   g   h");

		// Act
		var drawing = formatter.Format(position);

		// Assert
		drawing.Should().Be(expected);
	}

	[TestMethod]
	public void GivenStartingPosition_WhenFormattingInEnglish_ThenTheBackRankUsesFenLetters()
	{
		// Arrange
		var position = PositionModel.StartingPosition;

		// Act
		var lines = Lines(formatter.Format(position, PieceLetterStyleEnum.English, GameColorModel.White));

		// Assert
		lines[2].Should().Be("8 │ r │ n │ b │ q │ k │ b │ n │ r │ 8");
		lines[16].Should().Be("1 │ R │ N │ B │ Q │ K │ B │ N │ R │ 1");
	}

	[TestMethod]
	public void GivenStartingPosition_WhenFormattingAsFigurine_ThenBlackAndWhiteUseTheirOwnSymbols()
	{
		// Arrange
		var position = PositionModel.StartingPosition;

		// Act
		var lines = Lines(formatter.Format(position, PieceLetterStyleEnum.Figurine, GameColorModel.White));

		// Assert
		lines[2].Should().Be("8 │ ♜ │ ♞ │ ♝ │ ♛ │ ♚ │ ♝ │ ♞ │ ♜ │ 8");
		lines[16].Should().Be("1 │ ♖ │ ♘ │ ♗ │ ♕ │ ♔ │ ♗ │ ♘ │ ♖ │ 1");
	}

	[TestMethod]
	public void GivenBlackPerspective_WhenFormatting_ThenRankOneIsOnTopAndFileHOnTheLeft()
	{
		// Arrange
		var position = PositionModel.StartingPosition;

		// Act
		var lines = Lines(formatter.Format(position, PieceLetterStyleEnum.Spanish, GameColorModel.Black));

		// Assert
		lines[0].Should().Be("    h   g   f   e   d   c   b   a");
		lines[2].Should().Be("1 │ T │ C │ A │ R │ D │ A │ C │ T │ 1");
		lines[16].Should().Be("8 │ t │ c │ a │ r │ d │ a │ c │ t │ 8");
	}

	[TestMethod]
	public void GivenBlackPerspective_WhenFormatting_ThenEverySquareKeepsItsPiece()
	{
		// Arrange: only the white king on h1, the corner that moves the most when flipping.
		var position = fenSerializer.Deserialize("4k3/8/8/8/8/8/8/7K w - - 0 1");

		// Act
		var lines = Lines(formatter.Format(position, PieceLetterStyleEnum.Spanish, GameColorModel.Black));

		// Assert: from black's side h1 is the top left square.
		lines[2].Should().Be("1 │ R │   │   │   │   │   │   │   │ 1");
	}

	[TestMethod]
	public void GivenAnEmptyBoard_WhenFormatting_ThenNoLineHasTrailingSpaces()
	{
		// Arrange
		var position = fenSerializer.Deserialize("4k3/8/8/8/8/8/8/4K3 w - - 0 1");

		// Act
		var lines = Lines(formatter.Format(position, PieceLetterStyleEnum.Spanish, GameColorModel.White));

		// Assert
		lines.Should().AllSatisfy(line => line.Should().Be(line.TrimEnd()));
	}

	[TestMethod]
	public void GivenAnyPosition_WhenFormatting_ThenEveryRankRowIsTheSameWidth()
	{
		// Arrange
		var position = PositionModel.StartingPosition;

		// Act
		var lines = Lines(formatter.Format(position, PieceLetterStyleEnum.Spanish, GameColorModel.White));

		// Assert: rows carry the rank number on both sides, borders do not.
		lines.Where((_, index) => index % 2 == 0 && index > 0 && index < 18)
			 .Should().AllSatisfy(line => line.Length.Should().Be(37));
	}

	private static string[] Lines(string drawing)
		=> drawing.Split(Environment.NewLine);
}

