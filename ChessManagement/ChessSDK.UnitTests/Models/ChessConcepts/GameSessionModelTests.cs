namespace ChessSDK.UnitTests.Models.ChessConcepts;

using AwesomeAssertions;
using ChessSDK.Enums;
using ChessSDK.Models.ChessConcepts;
using ChessSDK.Notation;

[TestClass]
public class GameSessionModelTests
{
	[TestMethod]
	public void GivenAGame_WhenASideResigns_ThenTheGameIsOverAndTheOtherSideWins()
	{
		// Arrange
		var session = new GameSessionModel("abc", GameColorModel.White);
		session.TryApplyMove("e2e4", out _).Should().BeTrue();
		var fenBeforeResigning = session.ToFen();

		// Act
		var resigned = session.TryResign(GameColorModel.White, out var error);

		// Assert
		resigned.Should().BeTrue(error);
		session.Result.Should().Be(GameResultEnum.Resigned);
		session.IsOver.Should().BeTrue();
		session.ResignedBy.Should().BeSameAs(GameColorModel.White);
		session.Winner.Should().BeSameAs(GameColorModel.Black);
		session.History.Should().HaveCount(1);
		session.ToFen().Should().Be(fenBeforeResigning);
	}

	[TestMethod]
	public void GivenAResignedGame_WhenAMoveIsApplied_ThenItIsRejected()
	{
		// Arrange
		var session = new GameSessionModel("abc", GameColorModel.White);
		session.TryResign(GameColorModel.Black, out _).Should().BeTrue();

		// Act
		var applied = session.TryApplyMove("e2e4", out var error);

		// Assert
		applied.Should().BeFalse();
		error.Should().Contain("terminado");
	}

	[TestMethod]
	public void GivenAResignedGame_WhenResigningAgain_ThenItIsRejected()
	{
		// Arrange
		var session = new GameSessionModel("abc", GameColorModel.White);
		session.TryResign(GameColorModel.White, out _).Should().BeTrue();

		// Act
		var resigned = session.TryResign(GameColorModel.Black, out var error);

		// Assert
		resigned.Should().BeFalse();
		error.Should().Contain("terminado");
		session.ResignedBy.Should().BeSameAs(GameColorModel.White);
	}

	[TestMethod]
	public void GivenAResignedGame_WhenUndoing_ThenNothingIsTakenBack()
	{
		// Arrange
		var session = new GameSessionModel("abc", GameColorModel.White);
		session.TryApplyMove("e2e4", out _).Should().BeTrue();
		session.TryResign(GameColorModel.White, out _).Should().BeTrue();

		// Act
		var undone = session.Undo();

		// Assert
		undone.Should().BeFalse();
		session.History.Should().HaveCount(1);
	}

	[TestMethod]
	public void GivenAResignedGame_WhenReset_ThenTheResignationIsCleared()
	{
		// Arrange
		var session = new GameSessionModel("abc", GameColorModel.White);
		session.TryResign(GameColorModel.White, out _).Should().BeTrue();

		// Act
		session.Reset();

		// Assert
		session.ResignedBy.Should().BeNull();
		session.Result.Should().Be(GameResultEnum.InProgress);
		session.ToFen().Should().Be(FenSerializer.StartingPositionFen);
	}

	[TestMethod]
	public void GivenACheckmate_WhenTheWinnerIsAsked_ThenItIsTheSideThatDeliveredIt()
	{
		// Arrange
		var session = new GameSessionModel("abc", GameColorModel.White);

		foreach (var move in new[] { "f2f3", "e7e5", "g2g4", "d8h4" })
			session.TryApplyMove(move, out _).Should().BeTrue();

		// Act
		var winner = session.Winner;

		// Assert
		session.Result.Should().Be(GameResultEnum.Checkmate);
		winner.Should().BeSameAs(GameColorModel.Black);
	}

	[TestMethod]
	public void GivenSeveralMoves_WhenSeveralPliesAreUndone_ThenAllOfThemAreTakenBack()
	{
		// Arrange
		var session = new GameSessionModel("abc", GameColorModel.White);
		var afterFirstMove = string.Empty;

		foreach (var move in new[] { "e2e4", "e7e5", "g1f3" })
		{
			session.TryApplyMove(move, out _).Should().BeTrue();

			if (move == "e2e4")
				afterFirstMove = session.ToFen();
		}

		// Act
		var undone = session.Undo(2);

		// Assert
		undone.Should().Be(2);
		session.ToFen().Should().Be(afterFirstMove);
		session.History.Should().HaveCount(1);
	}

	[TestMethod]
	public void GivenMorePliesThanPlayed_WhenUndone_ThenOnlyThePlayedOnesAreTakenBack()
	{
		// Arrange
		var session = new GameSessionModel("abc", GameColorModel.White);
		session.TryApplyMove("e2e4", out _).Should().BeTrue();

		// Act
		var undone = session.Undo(99);

		// Assert
		undone.Should().Be(1);
		session.History.Should().BeEmpty();
		session.ToFen().Should().Be(FenSerializer.StartingPositionFen);
	}

	[TestMethod]
	public void GivenAResignedGame_WhenSeveralPliesAreUndone_ThenNothingIsTakenBack()
	{
		// Arrange
		var session = new GameSessionModel("abc", GameColorModel.White);
		session.TryApplyMove("e2e4", out _).Should().BeTrue();
		session.TryResign(GameColorModel.White, out _).Should().BeTrue();

		// Act
		var undone = session.Undo(2);

		// Assert
		undone.Should().Be(0);
		session.History.Should().HaveCount(1);
	}

	[TestMethod]
	public void GivenANonPositiveCount_WhenUndoing_ThenItThrows()
	{
		// Arrange
		var session = new GameSessionModel("abc", GameColorModel.White);

		// Act
		var undoing = () => session.Undo(0);

		// Assert
		undoing.Should().Throw<ArgumentOutOfRangeException>();
	}

	[TestMethod]
	public void GivenANewSession_WhenCreated_ThenItStartsFromTheInitialFen()
	{
		// Arrange & Act
		var session = new GameSessionModel("abc", GameColorModel.White);

		// Assert
		session.ToFen().Should().Be(FenSerializer.StartingPositionFen);
		session.LegalMoves().Should().HaveCount(20);
	}

	[TestMethod]
	public void GivenALegalMove_WhenApplied_ThenTheTurnAndTheFenChange()
	{
		// Arrange
		var session = new GameSessionModel("abc", GameColorModel.White);

		// Act
		var applied = session.TryApplyMove("e2e4", out var error);

		// Assert
		applied.Should().BeTrue();
		error.Should().BeEmpty();
		session.SideToMove.Should().BeSameAs(GameColorModel.Black);
		session.ToFen().Should().Be("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1");
	}

	[TestMethod]
	public void GivenAnIllegalMove_WhenApplied_ThenItIsRejectedAndThePositionIsUnchanged()
	{
		// Arrange
		var session = new GameSessionModel("abc", GameColorModel.White);
		var before = session.ToFen();

		// Act
		var applied = session.TryApplyMove("e2e5", out var error);

		// Assert
		applied.Should().BeFalse();
		error.Should().Contain("no es legal");
		session.ToFen().Should().Be(before);
		session.History.Should().BeEmpty();
	}

	[TestMethod]
	public void GivenAnEmptyOriginSquare_WhenAMoveIsApplied_ThenTheErrorExplainsIt()
	{
		// Arrange
		var session = new GameSessionModel("abc", GameColorModel.White);

		// Act
		var applied = session.TryApplyMove("e3e4", out var error);

		// Assert
		applied.Should().BeFalse();
		error.Should().Contain("no hay ninguna pieza");
	}

	[TestMethod]
	public void GivenAPieceOfTheOtherSide_WhenAMoveIsApplied_ThenTheErrorExplainsTheTurn()
	{
		// Arrange
		var session = new GameSessionModel("abc", GameColorModel.White);

		// Act
		var applied = session.TryApplyMove("e7e5", out var error);

		// Assert
		applied.Should().BeFalse();
		error.Should().Contain("le toca mover");
	}

	[TestMethod]
	public void GivenAnIllegalKnightMove_WhenApplied_ThenTheErrorListsTheLegalOnes()
	{
		// Arrange
		var session = new GameSessionModel("abc", GameColorModel.White);

		// Act
		session.TryApplyMove("g1g3", out var error);

		// Assert
		error.Should().Contain("g1f3");
		error.Should().Contain("g1h3");
	}

	[TestMethod]
	public void GivenSomeMoves_WhenUndone_ThenThePreviousPositionIsRestored()
	{
		// Arrange
		var session = new GameSessionModel("abc", GameColorModel.White);
		session.TryApplyMove("e2e4", out _);
		var afterFirstMove = session.ToFen();
		session.TryApplyMove("e7e5", out _);

		// Act
		var undone = session.Undo();

		// Assert
		undone.Should().BeTrue();
		session.ToFen().Should().Be(afterFirstMove);
		session.History.Should().HaveCount(1);
	}

	[TestMethod]
	public void GivenANewSession_WhenUndoIsRequested_ThenNothingHappens()
	{
		// Arrange
		var session = new GameSessionModel("abc", GameColorModel.White);

		// Act
		var undone = session.Undo();

		// Assert
		undone.Should().BeFalse();
	}

	[TestMethod]
	public void GivenACastlingMove_WhenApplied_ThenTheKingAndTheRookMove()
	{
		// Arrange
		var session = new GameSessionModel("abc", GameColorModel.White);

		foreach (var move in new[] { "e2e4", "e7e5", "g1f3", "b8c6", "f1c4", "f8c5" })
			session.TryApplyMove(move, out _).Should().BeTrue();

		// Act
		var applied = session.TryApplyMove("e1g1", out var error);

		// Assert
		applied.Should().BeTrue(error);
		session.PieceAt("g1")!.Piece.Symbol.Should().Be('K');
		session.PieceAt("f1")!.Piece.Symbol.Should().Be('R');
	}

	[TestMethod]
	public void GivenAPromotionWithoutAPiece_WhenApplied_ThenThePieceIsRequested()
	{
		// Arrange
		var session = new GameSessionModel("abc", GameColorModel.White);

		foreach (var move in new[] { "a2a4", "b7b5", "a4b5", "g8f6", "b5b6", "f6g8", "b6b7", "g8f6" })
			session.TryApplyMove(move, out _).Should().BeTrue();

		// Act
		var applied = session.TryApplyMove("b7a8", out var error);

		// Assert
		applied.Should().BeFalse();
		error.Should().Contain("promociona");
	}

	[TestMethod]
	public void GivenAFinishedGame_WhenAnotherMoveIsApplied_ThenItIsRejected()
	{
		// Arrange
		var session = new GameSessionModel("abc", GameColorModel.White);

		foreach (var move in new[] { "f2f3", "e7e5", "g2g4", "d8h4" })
			session.TryApplyMove(move, out _).Should().BeTrue();

		// Act
		var applied = session.TryApplyMove("e1f2", out var error);

		// Assert
		applied.Should().BeFalse();
		error.Should().Contain("terminado");
	}
}
