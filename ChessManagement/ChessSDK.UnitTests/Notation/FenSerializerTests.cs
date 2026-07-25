namespace ChessSDK.UnitTests.Notation;

using AwesomeAssertions;
using ChessSDK.Models.Boards;
using ChessSDK.Notation;

[TestClass]
public class FenSerializerTests
{
	private static readonly string[] knownFens =
	{
		"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
		"rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2",
		"r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1",
		"8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1",
		"r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1",
		"rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8",
		"r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10",
		"8/8/8/8/8/8/8/K6k w - - 0 1",
		"4k3/8/8/8/8/8/4P3/4K3 w - - 5 39",
		"8/8/8/2k5/2pP4/8/B7/4K3 b - d3 0 3",
		"r1bqkbnr/pppp1ppp/2n5/4p3/2B1P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 3 3",
		"8/5k2/8/8/8/8/2K5/6R1 b - - 12 45",
		"6k1/5ppp/8/8/8/8/5PPP/6K1 w - - 0 30",
		"rnbqkbnr/ppp1pppp/8/3p4/8/8/PPPPPPPP/RNBQKBNR w KQkq d6 0 2",
		"2kr3r/pp1b1ppp/2n1pn2/q7/3P4/2N1BN2/PPQ2PPP/2KR3R w - - 4 13",
		"8/8/8/8/1k6/8/1K6/8 w - - 0 1",
		"r1bq1rk1/pp2ppbp/2np1np1/8/2PNP3/2N1B3/PP2BPPP/R2Q1RK1 b - - 2 9",
		"3k4/3p4/8/K1P4r/8/8/8/8 b - - 0 1",
		"5k2/8/8/8/8/8/8/4K2R w K - 0 1",
		"4k2r/8/8/8/8/8/8/4K3 b k - 0 1"
	};

	[TestMethod]
	public void GivenTheStartingPosition_WhenSerialized_ThenItIsTheCanonicalInitialFen()
	{
		// Arrange
		var serializer = new FenSerializer();

		// Act
		var fen = serializer.Serialize(PositionModel.StartingPosition);

		// Assert
		fen.Should().Be("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
	}

	[TestMethod]
	public void GivenTwentyKnownFens_WhenRoundTripped_ThenTheTextIsPreserved()
	{
		// Arrange
		var serializer = new FenSerializer();

		// Act
		var roundTripped = knownFens.Select(fen => serializer.Serialize(serializer.Deserialize(fen)));

		// Assert
		roundTripped.Should().Equal(knownFens);
	}

	[TestMethod]
	public void GivenTwentyKnownFens_WhenRoundTripped_ThenThePositionsAreEqual()
	{
		// Arrange
		var serializer = new FenSerializer();

		// Act & Assert
		foreach (var fen in knownFens)
		{
			var position = serializer.Deserialize(fen);
			var again = serializer.Deserialize(serializer.Serialize(position));

			again.Should().Be(position);
		}
	}

	[TestMethod]
	public void GivenTheInitialFen_WhenDeserialized_ThenItMatchesTheStartingPosition()
	{
		// Arrange
		var serializer = new FenSerializer();

		// Act
		var position = serializer.Deserialize(FenSerializer.StartingPositionFen);

		// Assert
		position.Should().Be(PositionModel.StartingPosition);
	}

	[TestMethod]
	public void GivenAFenWithoutClocks_WhenDeserialized_ThenTheClocksTakeTheirDefaults()
	{
		// Arrange
		var serializer = new FenSerializer();

		// Act
		var position = serializer.Deserialize("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -");

		// Assert
		position.HalfMoveClock.Should().Be(0);
		position.FullMoveNumber.Should().Be(1);
	}

	[TestMethod]
	public void GivenAFenWithEnPassant_WhenDeserialized_ThenTheTargetSquareIsRead()
	{
		// Arrange
		var serializer = new FenSerializer();

		// Act
		var position = serializer.Deserialize("8/8/8/2k5/2pP4/8/B7/4K3 b - d3 0 3");

		// Assert
		position.EnPassantTarget.Should().Be(CoordinateModel.Create(FileModel.D, RankModel.R3));
	}

	[TestMethod]
	public void GivenAFenWithoutCastlingRights_WhenDeserialized_ThenNoRightsRemain()
	{
		// Arrange
		var serializer = new FenSerializer();

		// Act
		var position = serializer.Deserialize("8/8/8/8/1k6/8/1K6/8 w - - 0 1");

		// Assert
		position.CastlingRights.IsEmpty.Should().BeTrue();
	}

	[TestMethod]
	public void GivenAMalformedFen_WhenDeserialized_ThenItThrows()
	{
		// Arrange
		var serializer = new FenSerializer();

		// Act
		var act = () => serializer.Deserialize("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP w KQkq - 0 1");

		// Assert
		act.Should().Throw<FormatException>();
	}

	[TestMethod]
	public void GivenARowThatDoesNotAddUpToEight_WhenDeserialized_ThenItIsRejected()
	{
		// Arrange
		var serializer = new FenSerializer();

		// Act
		var parsed = serializer.TryDeserialize("rnbqkbnr/ppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", out _, out var error);

		// Assert
		parsed.Should().BeFalse();
		error.Should().NotBeEmpty();
	}
}
