namespace ChessSDK.UnitTests.Notation;

using AwesomeAssertions;
using ChessSDK.Models.Boards;
using ChessSDK.Models.ChessConcepts;
using ChessSDK.Models.ChessConcepts.Formatters;
using ChessSDK.Notation;
using ChessSDK.Rules;

[TestClass]
public class SanParserTests
{
	private static readonly FenSerializer fenSerializer = new();

	/// <summary>
	/// Morphy vs Duke of Brunswick and Count Isouard, Paris 1858, known as the Opera Game.
	/// It has captures, a long castle, checks and a mate, so it exercises the whole notation.
	/// </summary>
	internal static readonly string[] OperaGame =
	[
		"e4", "e5", "Nf3", "d6", "d4", "Bg4", "dxe5", "Bxf3", "Qxf3", "dxe5",
		"Bc4", "Nf6", "Qb3", "Qe7", "Nc3", "c6", "Bg5", "b5", "Nxb5", "cxb5",
		"Bxb5+", "Nbd7", "O-O-O", "Rd8", "Rxd7", "Rxd7", "Rd1", "Qe6", "Bxd7+", "Nxd7",
		"Qb8+", "Nxb8", "Rd8#"
	];

	[TestMethod]
	public void GivenAPieceMove_WhenParsed_ThenTheLegalMoveIsReturned()
	{
		// Arrange
		var parser = new SanParser();
		var position = PositionModel.StartingPosition;

		// Act
		var parsed = parser.TryParse(position, "Nf3", out var move, out var error);

		// Assert
		parsed.Should().BeTrue(error);
		move.ToLongAlgebraic().Should().Be("g1f3");
	}

	[TestMethod]
	public void GivenLongAlgebraicNotation_WhenParsed_ThenItIsStillAccepted()
	{
		// Arrange
		var parser = new SanParser();

		// Act
		var parsed = parser.TryParse(PositionModel.StartingPosition, "e2e4", out var move, out var error);

		// Assert
		parsed.Should().BeTrue(error);
		move.ToLongAlgebraic().Should().Be("e2e4");
	}

	[TestMethod]
	public void GivenACastlingWrittenWithZeros_WhenParsed_ThenItIsUnderstood()
	{
		// Arrange
		var parser = new SanParser();
		var position = fenSerializer.Deserialize("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");

		// Act
		var parsed = parser.TryParse(position, "0-0-0", out var move, out var error);

		// Assert
		parsed.Should().BeTrue(error);
		move.IsCastle.Should().BeTrue();
		move.ToLongAlgebraic().Should().Be("e1c1");
	}

	[TestMethod]
	public void GivenAPawnCapture_WhenParsed_ThenTheOriginFileIsEnough()
	{
		// Arrange
		var parser = new SanParser();
		var position = fenSerializer.Deserialize("rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 2");

		// Act
		var parsed = parser.TryParse(position, "exd5", out var move, out var error);

		// Assert
		parsed.Should().BeTrue(error);
		move.ToLongAlgebraic().Should().Be("e4d5");
		move.IsCapture.Should().BeTrue();
	}

	[TestMethod]
	public void GivenAPromotionWithCheck_WhenParsed_ThenThePromotionPieceIsRead()
	{
		// Arrange
		var parser = new SanParser();
		var position = fenSerializer.Deserialize("8/4P3/8/8/8/8/6k1/4K3 w - - 0 1");

		// Act
		var parsed = parser.TryParse(position, "e8=Q", out var move, out var error);

		// Assert
		parsed.Should().BeTrue(error);
		move.Promotion.Should().BeSameAs(PieceModel.Queen);
		move.ToLongAlgebraic().Should().Be("e7e8q");
	}

	[TestMethod]
	public void GivenAnAmbiguousMove_WhenParsed_ThenItIsRejectedAndTheCandidatesAreListed()
	{
		// Arrange
		var parser = new SanParser();
		var position = fenSerializer.Deserialize("4k3/8/8/8/8/5N2/8/1N2K3 w - - 0 1");

		// Act
		var parsed = parser.TryParse(position, "Nd2", out _, out var error);

		// Assert
		parsed.Should().BeFalse();
		error.Should().Contain("ambiguo");
		error.Should().Contain("b1d2");
		error.Should().Contain("f3d2");
	}

	[TestMethod]
	public void GivenAnAmbiguousMoveWithItsFile_WhenParsed_ThenItIsResolved()
	{
		// Arrange
		var parser = new SanParser();
		var position = fenSerializer.Deserialize("4k3/8/8/8/8/5N2/8/1N2K3 w - - 0 1");

		// Act
		var parsed = parser.TryParse(position, "Nbd2", out var move, out var error);

		// Assert
		parsed.Should().BeTrue(error);
		move.ToLongAlgebraic().Should().Be("b1d2");
	}

	[TestMethod]
	public void GivenAnIllegalMove_WhenParsed_ThenTheErrorListsTheLegalOnes()
	{
		// Arrange
		var parser = new SanParser();

		// Act
		var parsed = parser.TryParse(PositionModel.StartingPosition, "Nf6", out _, out var error);

		// Assert
		parsed.Should().BeFalse();
		error.Should().Contain("no es un movimiento legal");
		error.Should().Contain("g1f3");
	}

	[TestMethod]
	public void GivenGibberish_WhenParsed_ThenItIsRejected()
	{
		// Arrange
		var parser = new SanParser();

		// Act
		var parsed = parser.TryParse(PositionModel.StartingPosition, "hola", out _, out var error);

		// Assert
		parsed.Should().BeFalse();
		error.Should().NotBeEmpty();
	}

	[TestMethod]
	public void GivenTheOperaGame_WhenEveryMoveIsParsedAndWrittenBack_ThenTheNotationSurvivesTheRoundTrip()
	{
		// Arrange
		var parser = new SanParser();
		var formatter = new EnglishSanFormatter();
		var position = PositionModel.StartingPosition;

		// Act
		var written = new List<string>(OperaGame.Length);

		foreach (var san in OperaGame)
		{
			parser.TryParse(position, san, out var move, out var error).Should().BeTrue($"'{san}' should be legal: {error}");

			written.Add(formatter.Format(move, position));
			position = position.Apply(move);
		}

		// Assert
		written.Should().Equal(OperaGame);
		new GameResultEvaluator().Evaluate(position, [position]).Should().Be(ChessSDK.Enums.GameResultEnum.Checkmate);
	}

	[TestMethod]
	[TestCategory("Slow")]
	public void GivenHundredsOfRandomPositions_WhenEveryLegalMoveIsWrittenAndParsedBack_ThenTheSameMoveComesOut()
	{
		// Arrange
		var parser = new SanParser();
		var formatter = new EnglishSanFormatter();
		var validator = new LegalityValidator();
		var random = new Random(20260226);
		var evaluator = new GameResultEvaluator();
		var checkedMoves = 0;

		// Act
		for (var game = 0; game < 20; game++)
		{
			var position = PositionModel.StartingPosition;
			var seen = new List<PositionModel> { position };

			for (var ply = 0; ply < 60; ply++)
			{
				var legalMoves = validator.GenerateLegal(position);

				if (legalMoves.Count == 0 || evaluator.Evaluate(position, seen) != ChessSDK.Enums.GameResultEnum.InProgress)
					break;

				// Assert
				foreach (var move in legalMoves)
				{
					var san = formatter.Format(move, position);

					parser.TryParse(position, san, out var parsed, out var error).Should().BeTrue($"'{san}' should round trip: {error}");
					parsed.Should().Be(move);
					checkedMoves++;
				}

				position = position.Apply(legalMoves[random.Next(legalMoves.Count)]);
				seen.Add(position);
			}
		}

		checkedMoves.Should().BeGreaterThan(10000);
	}
}
