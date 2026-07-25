namespace ChessSDK.Services;

using ChessSDK.Models.ChessConcepts;

public interface IGameStoreService
{
	GameSessionModel Create(GameColorModel humanColor);
	GameSessionModel? Find(string gameId);
	IReadOnlyCollection<GameSessionModel> All();
	bool Remove(string gameId);
}
