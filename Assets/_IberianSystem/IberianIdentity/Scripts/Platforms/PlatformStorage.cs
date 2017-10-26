namespace IberianSystem
{
    using System;
    using System.Collections;
    using UnityEngine;

    public abstract class PlatformStorage : MonoBehaviour, IPersistentGame
    {
        [HideInInspector]
        public string fileName;

        public event Action<object> OnGameLoadSuccess = delegate { };
        public event Action<string> OnGameLoadFail = delegate { };
        public event Action OnEmptyGameLoad = delegate { };

        public event Action OnGameSaveSuccess = delegate { };
        public event Action<string> OnGameSaveFail = delegate { };

        public abstract void ShowSavedGames();

        protected abstract void LoadData();
        protected abstract void SaveData(byte[] data);

        public float timeout;

        Type gameType;
        object pendingGameToSave;
        bool isSaving = false;
        bool isLoading = false;
        bool thereIsPendingLoad = false;

        private int loadIdCount = 0;
        private int saveIdCount = 0;

        public void Load(Type gameType)
        {
            this.gameType = gameType;
            if (isLoading)
            {
                Debug.Log("Load. Already loading");
            }
            else if (isSaving)
            {
                Debug.Log("Load. saving in progress");
                thereIsPendingLoad = true;
            }
            else
            {
                Debug.Log("New load");

                StartLoadData();
            }
        }

        public void Save<T>(T game)
        {
            SaveWithType(game, typeof(T));
        }

        void StartLoadData()
        {
            isLoading = true;
            LoadData();
            loadIdCount++;
            StartCoroutine(CheckForLoadTimeout(loadIdCount));
        }

        IEnumerator CheckForLoadTimeout(int loadId)
        {
            yield return new WaitForSeconds(timeout);
            if (isLoading && loadId == loadIdCount)
            {
                NotifyDataLoadFail("Load timeout"); // TODO use variable for message
            }
        }

        void SaveWithType(object game, Type type)
        {
            if (isLoading)
            {
                Debug.Log("Save. Loading in progress");
                OnGameSaveFail("Loading already in progress"); // TODO use variable for message
            }
            else if (isSaving)
            {
                Debug.Log("Save. Already saving");
                pendingGameToSave = game;
            }
            else
            {
                Debug.Log("Starting new save");

                gameType = type;
                byte[] data = IberianUtils.SerializeObject(game, gameType);
                StartSaveData(data);
            }
        }

        void StartSaveData(byte[] data)
        {
            isSaving = true;
            SaveData(data);
            saveIdCount++;
            StartCoroutine(CheckForSaveTimeout(saveIdCount));
        }

        IEnumerator CheckForSaveTimeout(int saveId)
        {
            yield return new WaitForSeconds(timeout);
            if (isSaving && saveId == saveIdCount)
            {
                NotifyDataSaveFail("Save timeout"); // TODO use variable for message
            }
        }

        protected void NotifyDataLoaded(byte[] data)
        {
            if (isLoading)
            {
                isLoading = false;
                object game = null;
                if (data != null && data.Length > 0)
                {
                    game = IberianUtils.DeserializeObject(data, gameType);
                }

                OnGameLoadSuccess(game);
            }
        }

        protected void NotifyDataLoadFail(string failMessage)
        {
            if (isLoading)
            {
                Debug.Log("NotifyDataLoadFail");
                isLoading = false;
                OnGameLoadFail(failMessage);
            }
        }

        protected void NotifyDataLoadEmpty()
        {
            if (isLoading)
            {
                Debug.Log("NotifyDataLoadEmpty");
                isLoading = false;
                OnEmptyGameLoad();
            }
        }

        protected void NotifyDataSaveSuccess()
        {
            if (isSaving)
            {
                OnDataSaved(RequestResult.Success, "");
            }
        }

        protected void NotifyDataSaveFail(string failMessage)
        {
            OnDataSaved(RequestResult.Fail, failMessage);
        }

        void OnDataSaved(RequestResult result, string message)
        {
            isSaving = false;
            if (pendingGameToSave != null)
            {
                object game = pendingGameToSave;
                pendingGameToSave = null;
                SaveWithType(game, gameType);
            }
            else
            {
                if (result == RequestResult.Success)
                {
                    OnGameSaveSuccess();
                }
                else if (result == RequestResult.Fail)
                {
                    OnGameSaveFail(message);
                }

                if (thereIsPendingLoad)
                {
                    thereIsPendingLoad = false;
                    StartLoadData();
                }
            }
        }


    }

}
