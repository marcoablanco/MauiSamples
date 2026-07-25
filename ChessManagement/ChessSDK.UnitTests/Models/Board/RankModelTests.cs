namespace ChessSDK.UnitTests.Models.Board;

using AwesomeAssertions;
using ChessSDK.Models.Boards;

[TestClass]
public class RankModelTests
{
	[TestMethod]
	public void GivenTwoRanksWithTheSameDigit_WhenCompared_ThenTheyAreEqual()
	{
		// Arrange
		var left = RankModel.FromChar('4');
		var right = (RankModel)"4";

		// Act & Assert
		left.Should().Be(right);
		(left == right).Should().BeTrue();
		(left != right).Should().BeFalse();
	}

	[TestMethod]
	public void GivenTwoDifferentRanks_WhenCompared_ThenTheyAreNotEqual()
	{
		// Arrange & Act & Assert
		RankModel.R1.Should().NotBe(RankModel.R8);
		(RankModel.R1 != RankModel.R8).Should().BeTrue();
	}

	[TestMethod]
	public void GivenARank_WhenConvertedFromChar_ThenTheCanonicalInstanceIsReturned()
	{
		// Arrange & Act
		RankModel rank = '3';

		// Assert
		rank.Should().BeSameAs(RankModel.R3);
	}

	[TestMethod]
	public void GivenTheEightRanks_WhenAddedToAHashSet_ThenTheSetHasEightEntries()
	{
		// Arrange & Act
		var set = new HashSet<RankModel>(BoardModel.AllRanks);

		// Assert
		set.Should().HaveCount(8);
	}

	[TestMethod]
	public void GivenARank_WhenIndexIsRead_ThenItIsZeroBased()
	{
		// Arrange & Act & Assert
		RankModel.R1.Index.Should().Be(0);
		RankModel.R8.Index.Should().Be(7);
	}

	[TestMethod]
	public void GivenAnIndex_WhenFromIndexIsCalled_ThenTheMatchingRankIsReturned()
	{
		// Arrange & Act
		var rank = RankModel.FromIndex(6);

		// Assert
		rank.Should().BeSameAs(RankModel.R7);
	}

	[TestMethod]
	public void GivenAnOutOfRangeDigit_WhenARankIsCreated_ThenItThrows()
	{
		// Arrange & Act
		var act = () => RankModel.FromChar('9');

		// Assert
		act.Should().Throw<ArgumentOutOfRangeException>();
	}

	[TestMethod]
	public void GivenARank_WhenFormattedAsText_ThenItIsTheDigit()
	{
		// Arrange & Act & Assert
		RankModel.R5.ToString().Should().Be("5");
		RankModel.R5.Name.Should().Be('5');
	}
}
