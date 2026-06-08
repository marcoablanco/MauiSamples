namespace ChessSDK.Models.Boards;

public class BoardModel
{
	public static readonly FileModel[] AllFiles =
	{
		FileModel.A, FileModel.B, FileModel.C, FileModel.D,
		FileModel.E, FileModel.F, FileModel.G, FileModel.H
	};

	public static readonly RankModel[] AllRanks =
	{
		RankModel.R1, RankModel.R2, RankModel.R3, RankModel.R4,
		RankModel.R5, RankModel.R6, RankModel.R7, RankModel.R8
	};

	public BoardModel()
	{
		Ranks = AllRanks;
		Files = AllFiles;
	}

	public FileModel[] Files { get; }
	public RankModel[] Ranks { get; }
}