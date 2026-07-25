namespace ChessSDK.UnitTests.Models.Board;

using AwesomeAssertions;
using ChessSDK.Enums;
using ChessSDK.Models.Boards;
using ChessSDK.Models.ChessConcepts;
using ChessSDK.Notation;

[TestClass]
public class PositionModelTests
{
	private static PositionModel Parse(string fen) => new FenSerializer().Deserialize(fen);

	[TestMethod]
	public void GivenTheStartingPosition_WhenItIsInspected_ThenTheBackRanksAreInPlace()
	{
		// Arrange
		var position = PositionModel.StartingPosition;

		// Act
		var whiteKing = position.PieceAt((CoordinateModel)"e1");
		var blackQueen = position.PieceAt((CoordinateModel)"d8");

		// Assert
		whiteKing.Should().Be(PlacedPieceModel.Get(PieceModel.King, GameColorModel.White));
		blackQueen.Should().Be(PlacedPieceModel.Get(PieceModel.Queen, GameColorModel.Black));
		position.SideToMove.Should().BeSameAs(GameColorModel.White);
		position.CastlingRights.Should().Be(CastlingRightsModel.All);
	}

	[TestMethod]
	public void GivenAKingSideCastle_WhenApplied_ThenTheRookMovesToo()
	{
		// Arrange
		var position = Parse("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
		var castle = new MoveModel(PieceModel.King, "e1", "g1", null, null, MoveKindEnum.CastleKingSide);

		// Act
		var next = position.Apply(castle);

		// Assert
		next.PieceAt((CoordinateModel)"g1").Should().Be(PlacedPieceModel.Get(PieceModel.King, GameColorModel.White));
		next.PieceAt((CoordinateModel)"f1").Should().Be(PlacedPieceModel.Get(PieceModel.Rook, GameColorModel.White));
		next.IsEmpty("e1").Should().BeTrue();
		next.IsEmpty("h1").Should().BeTrue();
	}

	[TestMethod]
	public void GivenAQueenSideCastle_WhenApplied_ThenTheRookMovesToo()
	{
		// Arrange
		var position = Parse("r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1");
		var castle = new MoveModel(PieceModel.King, "e8", "c8", null, null, MoveKindEnum.CastleQueenSide);

		// Act
		var next = position.Apply(castle);

		// Assert
		next.PieceAt((CoordinateModel)"c8").Should().Be(PlacedPieceModel.Get(PieceModel.King, GameColorModel.Black));
		next.PieceAt((CoordinateModel)"d8").Should().Be(PlacedPieceModel.Get(PieceModel.Rook, GameColorModel.Black));
		next.CastlingRights.BlackKingSide.Should().BeFalse();
		next.CastlingRights.BlackQueenSide.Should().BeFalse();
	}

	[TestMethod]
	public void GivenAnEnPassantCapture_WhenApplied_ThenTheCapturedPawnLeavesItsOwnSquare()
	{
		// Arrange
		var position = Parse("8/8/8/2pP4/8/8/8/K6k w - c6 0 2");
		var capture = new MoveModel(PieceModel.Pawn, "d5", "c6", PieceModel.Pawn, null, MoveKindEnum.EnPassant);

		// Act
		var next = position.Apply(capture);

		// Assert
		next.PieceAt((CoordinateModel)"c6").Should().Be(PlacedPieceModel.Get(PieceModel.Pawn, GameColorModel.White));
		next.IsEmpty("c5").Should().BeTrue();
		next.IsEmpty("d5").Should().BeTrue();
	}

	[TestMethod]
	public void GivenADoublePawnPush_WhenApplied_ThenTheEnPassantTargetIsSet()
	{
		// Arrange
		var move = new MoveModel(PieceModel.Pawn, "e2", "e4", null, null, MoveKindEnum.DoublePawnPush);

		// Act
		var next = PositionModel.StartingPosition.Apply(move);

		// Assert
		next.EnPassantTarget.Should().Be((CoordinateModel)"e3");
	}

	[TestMethod]
	public void GivenAQuietMove_WhenApplied_ThenTheEnPassantTargetIsCleared()
	{
		// Arrange
		var position = Parse("8/8/8/2pP4/8/8/8/K6k w - c6 0 2");
		var move = new MoveModel(PieceModel.King, "a1", "a2");

		// Act
		var next = position.Apply(move);

		// Assert
		next.EnPassantTarget.Should().BeNull();
	}

	[TestMethod]
	public void GivenAPromotion_WhenApplied_ThenThePromotedPieceIsPlaced()
	{
		// Arrange
		var position = Parse("8/P6k/8/8/8/8/8/K7 w - - 0 1");
		var move = new MoveModel(PieceModel.Pawn, "a7", "a8", null, PieceModel.Queen);

		// Act
		var next = position.Apply(move);

		// Assert
		next.PieceAt((CoordinateModel)"a8").Should().Be(PlacedPieceModel.Get(PieceModel.Queen, GameColorModel.White));
	}

	[TestMethod]
	public void GivenAKingMove_WhenApplied_ThenItsSideLosesBothCastlingRights()
	{
		// Arrange
		var position = Parse("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
		var move = new MoveModel(PieceModel.King, "e1", "f1");

		// Act
		var next = position.Apply(move);

		// Assert
		next.CastlingRights.WhiteKingSide.Should().BeFalse();
		next.CastlingRights.WhiteQueenSide.Should().BeFalse();
		next.CastlingRights.BlackKingSide.Should().BeTrue();
		next.CastlingRights.BlackQueenSide.Should().BeTrue();
	}

	[TestMethod]
	public void GivenARookCapturedOnItsHomeSquare_WhenApplied_ThenThatCastlingRightIsLost()
	{
		// Arrange
		var position = Parse("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
		var move = new MoveModel(PieceModel.Rook, "h1", "h8", PieceModel.Rook);

		// Act
		var next = position.Apply(move);

		// Assert
		next.CastlingRights.WhiteKingSide.Should().BeFalse();
		next.CastlingRights.BlackKingSide.Should().BeFalse();
		next.CastlingRights.WhiteQueenSide.Should().BeTrue();
		next.CastlingRights.BlackQueenSide.Should().BeTrue();
	}

	[TestMethod]
	public void GivenAMove_WhenApplied_ThenTheOriginalPositionIsUntouched()
	{
		// Arrange
		var position = PositionModel.StartingPosition;
		var move = new MoveModel(PieceModel.Pawn, "e2", "e4", null, null, MoveKindEnum.DoublePawnPush);

		// Act
		position.Apply(move);

		// Assert
		position.PieceAt((CoordinateModel)"e2").Should().Be(PlacedPieceModel.Get(PieceModel.Pawn, GameColorModel.White));
		position.IsEmpty("e4").Should().BeTrue();
	}

	[TestMethod]
	public void GivenABlackMove_WhenApplied_ThenTheFullMoveNumberIncreases()
	{
		// Arrange
		var position = Parse("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1");
		var move = new MoveModel(PieceModel.Pawn, "e7", "e5", null, null, MoveKindEnum.DoublePawnPush);

		// Act
		var next = position.Apply(move);

		// Assert
		next.FullMoveNumber.Should().Be(2);
		next.SideToMove.Should().BeSameAs(GameColorModel.White);
	}

	[TestMethod]
	public void GivenAQuietMove_WhenApplied_ThenTheHalfMoveClockIncreases()
	{
		// Arrange
		var position = Parse("4k3/8/8/8/8/8/8/4K2R w K - 7 40");
		var move = new MoveModel(PieceModel.Rook, "h1", "h4");

		// Act
		var next = position.Apply(move);

		// Assert
		next.HalfMoveClock.Should().Be(8);
	}
}
