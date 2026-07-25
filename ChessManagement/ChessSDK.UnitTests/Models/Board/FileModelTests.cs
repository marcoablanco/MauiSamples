namespace ChessSDK.UnitTests.Models.Board;

using AwesomeAssertions;
using ChessSDK.Models.Boards;

[TestClass]
public class FileModelTests
{
	[TestMethod]
	public void GivenTwoFilesWithTheSameLetter_WhenCompared_ThenTheyAreEqual()
	{
		// Arrange
		var left = FileModel.FromChar('e');
		var right = (FileModel)"e";

		// Act & Assert
		left.Should().Be(right);
		(left == right).Should().BeTrue();
		(left != right).Should().BeFalse();
	}

	[TestMethod]
	public void GivenTwoDifferentFiles_WhenCompared_ThenTheyAreNotEqual()
	{
		// Arrange & Act & Assert
		FileModel.A.Should().NotBe(FileModel.B);
		(FileModel.A != FileModel.B).Should().BeTrue();
	}

	[TestMethod]
	public void GivenAFile_WhenConvertedFromChar_ThenTheCanonicalInstanceIsReturned()
	{
		// Arrange & Act
		FileModel file = 'c';

		// Assert
		file.Should().BeSameAs(FileModel.C);
	}

	[TestMethod]
	public void GivenTheEightFiles_WhenAddedToAHashSet_ThenTheSetHasEightEntries()
	{
		// Arrange & Act
		var set = new HashSet<FileModel>(BoardModel.AllFiles);

		// Assert
		set.Should().HaveCount(8);
	}

	[TestMethod]
	public void GivenTheSameFileTwice_WhenAddedToAHashSet_ThenOnlyOneEntryRemains()
	{
		// Arrange & Act
		var set = new HashSet<FileModel> { FileModel.FromChar('h'), (FileModel)"h" };

		// Assert
		set.Should().HaveCount(1);
	}

	[TestMethod]
	public void GivenAFile_WhenIndexIsRead_ThenItIsZeroBased()
	{
		// Arrange & Act & Assert
		FileModel.A.Index.Should().Be(0);
		FileModel.H.Index.Should().Be(7);
	}

	[TestMethod]
	public void GivenAnIndex_WhenFromIndexIsCalled_ThenTheMatchingFileIsReturned()
	{
		// Arrange & Act
		var file = FileModel.FromIndex(4);

		// Assert
		file.Should().BeSameAs(FileModel.E);
	}

	[TestMethod]
	public void GivenAnOutOfRangeLetter_WhenAFileIsCreated_ThenItThrows()
	{
		// Arrange & Act
		var act = () => FileModel.FromChar('i');

		// Assert
		act.Should().Throw<ArgumentOutOfRangeException>();
	}

	[TestMethod]
	public void GivenAFile_WhenFormattedAsText_ThenItIsTheLetter()
	{
		// Arrange & Act & Assert
		FileModel.D.ToString().Should().Be("d");
		FileModel.D.Name.Should().Be('d');
	}
}
