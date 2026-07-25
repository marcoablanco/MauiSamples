namespace ChessSDK.UnitTests.Models.Board;

using AwesomeAssertions;
using ChessSDK.Models.Boards;

[TestClass]
public class CoordinateModelTests
{
	[TestMethod]
	public void GivenACoordinate_WhenComparedWithItsAlgebraicText_ThenTheyAreEqual()
	{
		// Arrange
		var coordinate = CoordinateModel.Create(FileModel.E, RankModel.R4);

		// Act
		var result = coordinate.Equals("e4");

		// Assert
		result.Should().BeTrue();
	}

	[TestMethod]
	public void GivenTwoCoordinatesOfTheSameSquare_WhenCompared_ThenTheyAreEqual()
	{
		// Arrange
		var left = CoordinateModel.Create(FileModel.E, RankModel.R4);
		CoordinateModel right = "e4";

		// Act & Assert
		left.Should().Be(right);
		(left == right).Should().BeTrue();
		left.Should().BeSameAs(right);
	}

	[TestMethod]
	public void GivenTwoDifferentSquares_WhenCompared_ThenTheyAreNotEqual()
	{
		// Arrange
		CoordinateModel left = "e4";
		CoordinateModel right = "e5";

		// Act & Assert
		left.Should().NotBe(right);
		(left != right).Should().BeTrue();
	}

	[TestMethod]
	public void GivenTheSixtyFourSquares_WhenAddedToAHashSet_ThenTheSetHasSixtyFourEntries()
	{
		// Arrange
		var squares = new HashSet<CoordinateModel>();

		// Act
		foreach (var rank in BoardModel.AllRanks)
		foreach (var file in BoardModel.AllFiles)
			squares.Add(CoordinateModel.Create(file, rank));

		// Assert
		squares.Should().HaveCount(64);
	}

	[TestMethod]
	public void GivenTheCornerSquares_WhenIndexIsRead_ThenA1IsZeroAndH8IsSixtyThree()
	{
		// Arrange & Act & Assert
		CoordinateModel.Create(FileModel.A, RankModel.R1).Index.Should().Be(0);
		CoordinateModel.Create(FileModel.H, RankModel.R8).Index.Should().Be(63);
	}

	[TestMethod]
	public void GivenAnIndex_WhenFromIndexIsCalled_ThenTheMatchingSquareIsReturned()
	{
		// Arrange & Act
		var square = CoordinateModel.FromIndex(28);

		// Assert
		square.ToString().Should().Be("e4");
	}

	[TestMethod]
	public void GivenAllSquares_WhenTheirIndexesAreRead_ThenEveryIndexIsUnique()
	{
		// Arrange & Act
		var indexes = CoordinateModel.All.Select(square => square.Index).ToArray();

		// Assert
		indexes.Should().OnlyHaveUniqueItems();
		indexes.Should().HaveCount(64);
	}

	[TestMethod]
	public void GivenASquare_WhenOffsetStaysOnTheBoard_ThenTheTargetSquareIsReturned()
	{
		// Arrange
		CoordinateModel origin = "e4";

		// Act
		var moved = origin.TryOffset(1, 2, out var target);

		// Assert
		moved.Should().BeTrue();
		target.ToString().Should().Be("f6");
	}

	[TestMethod]
	public void GivenASquare_WhenOffsetLeavesTheBoard_ThenNoSquareIsReturned()
	{
		// Arrange
		CoordinateModel origin = "a1";

		// Act
		var moved = origin.TryOffset(-1, 0, out _);

		// Assert
		moved.Should().BeFalse();
	}

	[TestMethod]
	public void GivenAnInvalidText_WhenConvertedToACoordinate_ThenItThrows()
	{
		// Arrange & Act
		var act = () => (CoordinateModel)"z9";

		// Assert
		act.Should().Throw<ArgumentException>();
	}
}
