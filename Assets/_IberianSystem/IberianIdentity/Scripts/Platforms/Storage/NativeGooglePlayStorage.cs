namespace IberianSystem
{
    using UnityEngine;

    public class NativeGooglePlayStorage : PlatformStorage
    {
        private bool wasPaused = false;
        private byte[] pendingDataToSave;
        private bool thereIsPendingDataToSave = false;

        private void OnEnable()
        {
            IdentityManager.Instance.OnConnectionChange -= OnConnectionChanged;
            IdentityManager.Instance.OnConnectionChange += OnConnectionChanged;
        }

        private void OnConnectionChanged(LoginStatus loginStatus)
        {
            if (wasPaused && loginStatus == LoginStatus.Connected && thereIsPendingDataToSave)
            {
                wasPaused = false;
                thereIsPendingDataToSave = false;
                SaveData(pendingDataToSave);
            }
            wasPaused = false;
        }

        public override void ShowSavedGames()
        {
            GooglePlaySavedGamesManager.Instance.ShowSavedGamesUI("Saves", 5);
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                wasPaused = pauseStatus;
            }
        }

        private void ActionGameSaveConflicted(GP_SnapshotConflict conflict)
        {
            if (conflict.ConflictingSnapshot.meta.LastModifiedTimestamp >= conflict.Snapshot.meta.LastModifiedTimestamp)
            {
                conflict.Resolve(conflict.ConflictingSnapshot);
            }
            else
            {
                conflict.Resolve(conflict.Snapshot);
            }
        }

        protected override void LoadData()
        {
            GooglePlaySavedGamesManager.ActionGameSaveLoaded -= ActionGameSaveLoaded;
            GooglePlaySavedGamesManager.ActionConflict -= ActionGameSaveConflicted;

            GooglePlaySavedGamesManager.ActionGameSaveLoaded += ActionGameSaveLoaded;
            GooglePlaySavedGamesManager.ActionConflict += ActionGameSaveConflicted;
            GooglePlaySavedGamesManager.Instance.LoadSpanshotByName(fileName);
        }

        protected override void SaveData(byte[] data)
        {
            if (wasPaused)
            {
                pendingDataToSave = data;
                thereIsPendingDataToSave = true;
            }
            else
            {
                GooglePlaySavedGamesManager.ActionGameSaveResult -= ActionGameSaveResult;
                GooglePlaySavedGamesManager.ActionConflict -= ActionGameSaveConflicted;

                GooglePlaySavedGamesManager.ActionGameSaveResult += ActionGameSaveResult;
                GooglePlaySavedGamesManager.ActionConflict += ActionGameSaveConflicted;

                Texture2D screenshot = new Texture2D(1, 1);
                GooglePlaySavedGamesManager.Instance.CreateNewSnapshot(fileName, fileName, screenshot, data, 0);
            }
        }



        void ActionGameSaveLoaded(GP_SpanshotLoadResult result)
        {
            GooglePlaySavedGamesManager.ActionGameSaveLoaded -= ActionGameSaveLoaded;

            if (result.IsSucceeded)
            {
                if (result.Snapshot.bytes.Length > 0)
                {
                    Debug.Log("Lenght > 0");
                    NotifyDataLoaded(result.Snapshot.bytes);
                }
                else
                {
                    Debug.Log("empty");
                    NotifyDataLoadEmpty();
                }
            }
            else if (result.IsFailed)
            {
                NotifyDataLoadFail(result.Message);
            }
        }

        void ActionGameSaveResult(GP_SpanshotLoadResult result)
        {
            GooglePlaySavedGamesManager.ActionGameSaveResult -= ActionGameSaveResult;

            if (result.IsSucceeded)
            {
                NotifyDataSaveSuccess();
            }
            else if (result.IsFailed)
            {
                NotifyDataSaveFail(result.Message);
            }
        }

        private Texture2D MakeScreenShot()
        {
            int width = 5;
            int height = 5;

            Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenshot.Apply();

            return screenshot;
        }
    }
}
