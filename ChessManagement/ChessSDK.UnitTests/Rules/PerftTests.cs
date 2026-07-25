namespace ChessSDK.UnitTests.Rules;

using AwesomeAssertions;
using ChessSDK.Models.Boards;
using ChessSDK.Notation;
using ChessSDK.Rules;

/// <summary>
/// Perft is the reference test of a move generator: it counts the leaf nodes of the legal move
/// tree at a given depth. If these numbers do not match, the generator is wrong.
/// </summary>
[TestClass]
public class PerftTests
{
	private const string KiwipeteFen = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";

	private static readonly LegalityValidator validator = new();

	private static long Perft(PositionModel position, int depth)
	{
		var moves = validator.GenerateLegal(position);

		if (depth <= 1)
			return moves.Count;

		long nodes = 0;

		foreach (var move in moves)
			nodes += Perft(position.Apply(move), depth - 1);

		return nodes;
	}

	private static PositionModel Parse(string fen) => new FenSerializer().Deserialize(fen);

	[TestMethod]
	public void GivenTheStartingPosition_WhenPerftDepthOneIsCounted_ThenThereAreTwentyNodes()
	{
		// Arrange & Act
		var nodes = Perft(PositionModel.StartingPosition, 1);

		// Assert
		nodes.Should().Be(20);
	}

	[TestMethod]
	public void GivenTheStartingPosition_WhenPerftDepthTwoIsCounted_ThenThereAreFourHundredNodes()
	{
		// Arrange & Act
		var nodes = Perft(PositionModel.StartingPosition, 2);

		// Assert
		nodes.Should().Be(400);
	}

	[TestMethod]
	public void GivenTheStartingPosition_WhenPerftDepthThreeIsCounted_ThenThereAreEightThousandNineHundredAndTwoNodes()
	{
		// Arrange & Act
		var nodes = Perft(PositionModel.StartingPosition, 3);

		// Assert
		nodes.Should().Be(8_902);
	}

	[TestMethod]
	[TestCategory("Slow")]
	public void GivenTheStartingPosition_WhenPerftDepthFourIsCounted_ThenThereAreOneHundredNinetySevenThousandNodes()
	{
		// Arrange & Act
		var nodes = Perft(PositionModel.StartingPosition, 4);

		// Assert
		nodes.Should().Be(197_281);
	}

	[TestMethod]
	[TestCategory("Slow")]
	public void GivenTheStartingPosition_WhenPerftDepthFiveIsCounted_ThenThereAreFourMillionNodes()
	{
		// Arrange & Act
		var nodes = Perft(PositionModel.StartingPosition, 5);

		// Assert
		nodes.Should().Be(4_865_609);
	}

	[TestMethod]
	public void GivenKiwipete_WhenPerftDepthOneIsCounted_ThenThereAreFortyEightNodes()
	{
		// Arrange & Act
		var nodes = Perft(Parse(KiwipeteFen), 1);

		// Assert
		nodes.Should().Be(48);
	}

	[TestMethod]
	public void GivenKiwipete_WhenPerftDepthTwoIsCounted_ThenThereAreTwoThousandAndThirtyNineNodes()
	{
		// Arrange & Act
		var nodes = Perft(Parse(KiwipeteFen), 2);

		// Assert
		nodes.Should().Be(2_039);
	}

	[TestMethod]
	public void GivenKiwipete_WhenPerftDepthThreeIsCounted_ThenThereAreNinetySevenThousandNodes()
	{
		// Arrange & Act
		var nodes = Perft(Parse(KiwipeteFen), 3);

		// Assert
		nodes.Should().Be(97_862);
	}

	[TestMethod]
	public void GivenTheEnPassantPosition_WhenPerftIsCounted_ThenTheKnownNodesAreFound()
	{
		// Arrange
		var position = Parse("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1");

		// Act & Assert
		Perft(position, 1).Should().Be(14);
		Perft(position, 2).Should().Be(191);
		Perft(position, 3).Should().Be(2_812);
	}

	[TestMethod]
	public void GivenThePromotionPosition_WhenPerftIsCounted_ThenTheKnownNodesAreFound()
	{
		// Arrange
		var position = Parse("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1");

		// Act & Assert
		Perft(position, 1).Should().Be(6);
		Perft(position, 2).Should().Be(264);
		Perft(position, 3).Should().Be(9_467);
	}

	[TestMethod]
	public void GivenTheTalkchessPosition_WhenPerftIsCounted_ThenTheKnownNodesAreFound()
	{
		// Arrange
		var position = Parse("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8");

		// Act & Assert
		Perft(position, 1).Should().Be(44);
		Perft(position, 2).Should().Be(1_486);
		Perft(position, 3).Should().Be(62_379);
	}

	[TestMethod]
	public void GivenTheSteveMakerPosition_WhenPerftIsCounted_ThenTheKnownNodesAreFound()
	{
		// Arrange
		var position = Parse("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10");

		// Act & Assert
		Perft(position, 1).Should().Be(46);
		Perft(position, 2).Should().Be(2_079);
		Perft(position, 3).Should().Be(89_890);
	}
}
