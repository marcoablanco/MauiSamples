namespace ChessSDK.UnitTests.Rules;

using AwesomeAssertions;
using ChessSDK.Enums;
using ChessSDK.Models.Boards;
using ChessSDK.Models.ChessConcepts;
using ChessSDK.Notation;
using ChessSDK.Rules;

[TestClass]
public class GameResultEvaluatorTests
{
	private static readonly GameResultEvaluator evaluator = new();

	private static PositionModel Parse(string fen) => new FenSerializer().Deserialize(fen);

	[TestMethod]
	public void GivenTheStartingPosition_WhenEvaluated_ThenTheGameIsInProgress()
	{
		// Arrange & Act
		var result = evaluator.Evaluate(PositionModel.StartingPosition);

		// Assert
		result.Should().Be(GameResultEnum.InProgress);
	}

	[TestMethod]
	public void GivenTheFoolsMate_WhenEvaluated_ThenItIsCheckmate()
	{
		// Arrange
		var session = new GameSessionModel("test", GameColorModel.White);

		// Act
		foreach (var move in new[] { "f2f3", "e7e5", "g2g4", "d8h4" })
			session.TryApplyMove(move, out _).Should().BeTrue();

		// Assert
		session.Result.Should().Be(GameResultEnum.Checkmate);
		session.IsInCheck.Should().BeTrue();
	}

	[TestMethod]
	public void GivenTheScholarsMate_WhenEvaluated_ThenItIsCheckmate()
	{
		// Arrange
		var session = new GameSessionModel("test", GameColorModel.White);

		// Act
		foreach (var move in new[] { "e2e4", "e7e5", "f1c4", "b8c6", "d1h5", "g8f6", "h5f7" })
			session.TryApplyMove(move, out _).Should().BeTrue();

		// Assert
		session.Result.Should().Be(GameResultEnum.Checkmate);
	}

	[TestMethod]
	public void GivenACorneredKingWithNoLegalMoves_WhenEvaluated_ThenItIsStalemate()
	{
		// Arrange
		var position = Parse("7k/5Q2/6K1/8/8/8/8/8 b - - 0 1");

		// Act
		var result = evaluator.Evaluate(position);

		// Assert
		result.Should().Be(GameResultEnum.Stalemate);
	}

	[TestMethod]
	public void GivenOnlyTheTwoKings_WhenEvaluated_ThenTheMaterialIsInsufficient()
	{
		// Arrange
		var position = Parse("8/8/8/4k3/8/8/8/4K3 w - - 0 1");

		// Act
		var result = evaluator.Evaluate(position);

		// Assert
		result.Should().Be(GameResultEnum.InsufficientMaterial);
	}

	[TestMethod]
	public void GivenAKingAndBishopAgainstAKing_WhenEvaluated_ThenTheMaterialIsInsufficient()
	{
		// Arrange
		var position = Parse("8/8/8/4k3/8/8/4B3/4K3 w - - 0 1");

		// Act
		var result = evaluator.Evaluate(position);

		// Assert
		result.Should().Be(GameResultEnum.InsufficientMaterial);
	}

	[TestMethod]
	public void GivenAKingAndRookAgainstAKing_WhenEvaluated_ThenTheMaterialIsSufficient()
	{
		// Arrange
		var position = Parse("8/8/8/4k3/8/8/4R3/4K3 w - - 0 1");

		// Act
		var result = evaluator.Evaluate(position);

		// Assert
		result.Should().Be(GameResultEnum.InProgress);
	}

	[TestMethod]
	public void GivenAHundredHalfMovesWithoutProgress_WhenEvaluated_ThenTheFiftyMoveRuleApplies()
	{
		// Arrange
		var position = Parse("4k3/8/8/8/8/8/4P3/4K3 w - - 100 60");

		// Act
		var result = evaluator.Evaluate(position);

		// Assert
		result.Should().Be(GameResultEnum.FiftyMoveRule);
	}

	[TestMethod]
	public void GivenThreeIdenticalPositions_WhenEvaluated_ThenTheRepetitionIsDetected()
	{
		// Arrange
		var session = new GameSessionModel("test", GameColorModel.White);

		// Act
		foreach (var move in new[] { "g1f3", "g8f6", "f3g1", "f6g8", "g1f3", "g8f6", "f3g1", "f6g8" })
			session.TryApplyMove(move, out _).Should().BeTrue();

		// Assert
		session.Result.Should().Be(GameResultEnum.ThreefoldRepetition);
	}
}
