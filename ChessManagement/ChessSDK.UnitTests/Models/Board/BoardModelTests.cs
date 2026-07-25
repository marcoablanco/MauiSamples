namespace ChessSDK.UnitTests.Models.Board;

using AwesomeAssertions;
using ChessSDK.Models.Boards;

[TestClass]
public class BoardModelTests
{
	[TestMethod]
	public void GivenANewBoard_WhenCreated_ThenHasTheEightFiles()
	{
		// Arrange & Act
		var board = new BoardModel();

		// Assert
		board.Files.Should().Equal(
			FileModel.A, FileModel.B, FileModel.C, FileModel.D,
			FileModel.E, FileModel.F, FileModel.G, FileModel.H);
	}

	[TestMethod]
	public void GivenANewBoard_WhenCreated_ThenHasTheEightRanks()
	{
		// Arrange & Act
		var board = new BoardModel();

		// Assert
		board.Ranks.Should().Equal(
			RankModel.R1, RankModel.R2, RankModel.R3, RankModel.R4,
			RankModel.R5, RankModel.R6, RankModel.R7, RankModel.R8);
	}

	[TestMethod]
	public void GivenANewBoard_WhenCreated_ThenFilesUseTheSharedAllFilesArray()
	{
		// Arrange & Act
		var board = new BoardModel();

		// Assert
		board.Files.Should().BeSameAs(BoardModel.AllFiles);
	}

	[TestMethod]
	public void GivenANewBoard_WhenCreated_ThenRanksUseTheSharedAllRanksArray()
	{
		// Arrange & Act
		var board = new BoardModel();

		// Assert
		board.Ranks.Should().BeSameAs(BoardModel.AllRanks);
	}
}
