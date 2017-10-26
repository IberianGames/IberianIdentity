namespace IberianSystem
{
    using System;

    public interface IPersistentGame
    {
        event Action<object> OnGameLoadSuccess;
        event Action<string> OnGameLoadFail;
        event Action OnEmptyGameLoad;

        event Action OnGameSaveSuccess;
        event Action<string> OnGameSaveFail;

        void Load(Type gameType);
        void Save<T>(T data);
        void ShowSavedGames();
    }
}
