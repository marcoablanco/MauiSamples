namespace ChessSDK.UnitTests.Rules;

using AwesomeAssertions;
using ChessSDK.Enums;
using ChessSDK.Models.Boards;
using ChessSDK.Models.ChessConcepts;
using ChessSDK.Notation;
using ChessSDK.Rules;

[TestClass]
public class MoveGeneratorTests
{
	private static readonly LegalityValidator validator = new();

	private static PositionModel Parse(string fen) => new FenSerializer().Deserialize(fen);

	[TestMethod]
	public void GivenTheStartingPosition_WhenLegalMovesAreGenerated_ThenThereAreTwenty()
	{
		// Arrange & Act
		var moves = validator.GenerateLegal(PositionModel.StartingPosition);

		// Assert
		moves.Should().HaveCount(20);
	}

	[TestMethod]
	public void GivenAnEmptyBackRank_WhenLegalMovesAreGenerated_ThenBothCastlingMovesAppear()
	{
		// Arrange
		var position = Parse("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");

		// Act
		var castles = validator.GenerateLegal(position).Where(move => move.IsCastle).ToArray();

		// Assert
		castles.Should().HaveCount(2);
		castles.Select(move => move.Kind).Should().Contain(MoveKindEnum.CastleKingSide);
		castles.Select(move => move.Kind).Should().Contain(MoveKindEnum.CastleQueenSide);
	}

	[TestMethod]
	public void GivenARookAttackingTheSquareTheKingCrosses_WhenLegalMovesAreGenerated_ThenCastlingIsNotAllowed()
	{
		// Arrange
		var position = Parse("5rk1/8/8/8/8/8/8/4K2R w K - 0 1");

		// Act
		var castles = validator.GenerateLegal(position).Where(move => move.IsCastle);

		// Assert
		castles.Should().BeEmpty();
	}

	[TestMethod]
	public void GivenAKingInCheck_WhenLegalMovesAreGenerated_ThenCastlingIsNotAllowed()
	{
		// Arrange
		var position = Parse("4r1k1/8/8/8/8/8/8/4K2R w K - 0 1");

		// Act
		var castles = validator.GenerateLegal(position).Where(move => move.IsCastle);

		// Assert
		castles.Should().BeEmpty();
	}

	[TestMethod]
	public void GivenAPinnedBishop_WhenLegalMovesAreGenerated_ThenItCannotMove()
	{
		// Arrange
		var position = Parse("4k3/4r3/8/8/8/8/4B3/4K3 w - - 0 1");

		// Act
		var bishopMoves = validator.GenerateLegal(position).Where(move => ReferenceEquals(move.Piece, PieceModel.Bishop));

		// Assert
		bishopMoves.Should().BeEmpty();
	}

	[TestMethod]
	public void GivenAnEnPassantTarget_WhenLegalMovesAreGenerated_ThenTheCaptureIsAvailable()
	{
		// Arrange
		var position = Parse("8/8/8/2pP4/8/8/8/K6k w - c6 0 2");

		// Act
		var enPassant = validator.GenerateLegal(position).Where(move => move.IsEnPassant).ToArray();

		// Assert
		enPassant.Should().HaveCount(1);
		enPassant[0].ToLongAlgebraic().Should().Be("d5c6");
	}

	[TestMethod]
	public void GivenAPawnOnTheSeventhRank_WhenLegalMovesAreGenerated_ThenTheFourPromotionsAppear()
	{
		// Arrange
		var position = Parse("8/P6k/8/8/8/8/8/K7 w - - 0 1");

		// Act
		var promotions = validator.GenerateLegal(position).Where(move => move.IsPromotion).ToArray();

		// Assert
		promotions.Should().HaveCount(4);
		promotions.Select(move => move.Promotion!.Symbol).Should().BeEquivalentTo(new[] { 'Q', 'R', 'B', 'N' });
	}

	[TestMethod]
	public void GivenAKingNextToAnEnemyRook_WhenLegalMovesAreGenerated_ThenItCannotStepIntoCheck()
	{
		// Arrange
		var position = Parse("4k3/8/8/8/8/8/5r2/4K3 w - - 0 1");

		// Act
		var destinations = validator.GenerateLegal(position).Select(move => move.To.ToString()).ToArray();

		// Assert
		destinations.Should().NotContain("f1");
		destinations.Should().NotContain("e2");
		destinations.Should().NotContain("d2");
		destinations.Should().Contain("d1");
		destinations.Should().Contain("f2");
	}

	[TestMethod]
	public void GivenACheck_WhenTheSideToMoveIsAsked_ThenItIsReportedAsInCheck()
	{
		// Arrange
		var position = Parse("4k3/8/8/8/8/8/8/4K2r w - - 0 1");

		// Act
		var inCheck = validator.IsInCheck(position);

		// Assert
		inCheck.Should().BeTrue();
	}

	[TestMethod]
	public void GivenAPositionWithNoCheck_WhenTheSideToMoveIsAsked_ThenItIsNotInCheck()
	{
		// Arrange & Act
		var inCheck = validator.IsInCheck(PositionModel.StartingPosition);

		// Assert
		inCheck.Should().BeFalse();
	}
}
