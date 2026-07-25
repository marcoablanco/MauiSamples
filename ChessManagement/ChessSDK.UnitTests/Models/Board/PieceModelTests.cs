namespace ChessSDK.UnitTests.Models.Board;

using AwesomeAssertions;
using ChessSDK.Models.Boards;
using ChessSDK.Models.ChessConcepts;

[TestClass]
public class PieceModelTests
{
	[TestMethod]
	public void GivenAPieceSymbol_WhenParsed_ThenTheSingletonInstanceIsReturned()
	{
		// Arrange & Act
		var piece = PieceModel.FromSymbol('n');

		// Assert
		piece.Should().BeSameAs(PieceModel.Knight);
	}

	[TestMethod]
	public void GivenAnUnknownSymbol_WhenParsed_ThenItThrows()
	{
		// Arrange & Act
		var act = () => PieceModel.FromSymbol('x');

		// Assert
		act.Should().Throw<ArgumentOutOfRangeException>();
	}

	[TestMethod]
	public void GivenThePieces_WhenTheirSymbolsAreRead_ThenTheyMatchEnglishNotation()
	{
		// Arrange & Act
		var symbols = PieceModel.All.Select(piece => piece.Symbol);

		// Assert
		symbols.Should().Equal('P', 'N', 'B', 'R', 'Q', 'K');
	}

	[TestMethod]
	public void GivenThePieces_WhenTheirMaterialValueIsRead_ThenItFollowsTheUsualScale()
	{
		// Arrange & Act & Assert
		PieceModel.Pawn.Value.Should().Be(1);
		PieceModel.Knight.Value.Should().Be(3);
		PieceModel.Bishop.Value.Should().Be(3);
		PieceModel.Rook.Value.Should().Be(5);
		PieceModel.Queen.Value.Should().Be(9);
		PieceModel.King.Value.Should().Be(0);
	}

	[TestMethod]
	public void GivenTheAllCollection_WhenAddedToAHashSet_ThenEveryPieceIsDistinct()
	{
		// Arrange & Act
		var set = new HashSet<PieceModel>(PieceModel.All);

		// Assert
		set.Should().HaveCount(6);
	}

	[TestMethod]
	public void GivenAColor_WhenItsOppositeIsRead_ThenTheOtherColorIsReturned()
	{
		// Arrange & Act & Assert
		GameColorModel.White.Opposite.Should().BeSameAs(GameColorModel.Black);
		GameColorModel.Black.Opposite.Should().BeSameAs(GameColorModel.White);
	}

	[TestMethod]
	public void GivenAColorName_WhenParsed_ThenTheSingletonInstanceIsReturned()
	{
		// Arrange & Act
		GameColorModel color = "black";

		// Assert
		color.Should().BeSameAs(GameColorModel.Black);
	}

	[TestMethod]
	public void GivenAPieceAndAColor_WhenPlaced_ThenTheCachedInstanceIsReused()
	{
		// Arrange & Act
		var first = PlacedPieceModel.Get(PieceModel.Rook, GameColorModel.Black);
		var second = PlacedPieceModel.Get(PieceModel.Rook, GameColorModel.Black);

		// Assert
		first.Should().BeSameAs(second);
		first.Symbol.Should().Be('r');
	}

	[TestMethod]
	public void GivenTwoEquivalentPlacedPieces_WhenCompared_ThenTheyAreEqual()
	{
		// Arrange
		var left = new PlacedPieceModel(PieceModel.Queen, GameColorModel.White);
		var right = PlacedPieceModel.Get(PieceModel.Queen, GameColorModel.White);

		// Act & Assert
		left.Should().Be(right);
		(left == right).Should().BeTrue();
		left.Symbol.Should().Be('Q');
	}
}
