namespace IberianSystem
{
    using UnityEngine;
    using System.Collections.Generic;

    public class NativeGameCenterStorage : PlatformStorage
    {
        // TODO find and resolve conflicts for games
        private byte[] dataAux;

        public override void ShowSavedGames()
        {
            Debug.LogWarning("ShowSavedGames called. GameCenter has no saved games GUI.");
        }



        protected override void LoadData()
        {
            Debug.Log("it subcribes to HandleActionSavesFetched");
            ISN_GameSaves.ActionSavesFetched += HandleActionSavesFetched;
            ISN_GameSaves.Instance.FetchSavedGames();
        }

        protected override void SaveData(byte[] data)
        {
            ISN_GameSaves.ActionGameSaved += HandleActionGameSaved;
            ISN_GameSaves.Instance.SaveGame(data, fileName);
        }



        private void HandleActionSavesFetched(GK_FetchResult result)
        {
            Debug.Log("it enters on HandleActionSavesFetched");
            if (result.SavedGames.Count > 0)
            {
                Debug.Log("the result is :" + result.SavedGames[0].Name);
            }
            else
            {
                Debug.Log("the result is empty.");
            }
            ISN_GameSaves.ActionSavesFetched -= HandleActionSavesFetched;

            if (result.IsSucceeded)
            {
                //Check conflicts before searching the savedgame desired
                Debug.Log("Calling SearchAndResolveConflicts");
                SearchAndResolveConflicts(result.SavedGames);
            }

            if (result.IsFailed)
            {
                Debug.Log("Failed: " + result.Error.Message);
                //OnDataLoaded(null, RequestResult.Fail);
            }
        }



        private List<GK_SavedGame> LoopSearchOfConflicts(List<GK_SavedGame> savedGames)
        {
            bool isRepeated;
            List<GK_SavedGame> conflictsSaves = new List<GK_SavedGame>();
            for (int i = 0; i < savedGames.Count - 1; i++)
            {
                isRepeated = false;
                for (int j = i + 1; j < savedGames.Count; j++)
                {
                    if (savedGames[i].Name == savedGames[j].Name)
                    {
                        isRepeated = true;
                        conflictsSaves.Add(savedGames[j]);
                    }
                }
                if (isRepeated)
                {
                    Debug.Log("adding to resolve conflict: " + savedGames[i].Name);
                    conflictsSaves.Add(savedGames[i]);
                    break;
                }
            }

            return conflictsSaves;
        }

        private void SearchAndResolveConflicts(List<GK_SavedGame> savedGames)
        {
            List<GK_SavedGame> conflictsSaves = new List<GK_SavedGame>();

            conflictsSaves = LoopSearchOfConflicts(savedGames);

            if (conflictsSaves.Count == 0)
            {
                GK_SavedGame savedGame = FindSavedGame(savedGames);

                if (savedGame == null)
                {
                    NotifyDataLoadEmpty();
                }
                else
                {
                    LoadAfterFetch(savedGame);
                }
            }
            else
            {
                byte[] dataLatestSavedGame = null;
                System.DateTime mostRecentDate = new System.DateTime();
                for (int i = 0; i < conflictsSaves.Count; i++)
                {
                    if (System.DateTime.Compare(mostRecentDate, conflictsSaves[i].ModificationDate) < 0)
                    {
                        dataLatestSavedGame = conflictsSaves[i].Data;
                        mostRecentDate = conflictsSaves[i].ModificationDate;
                    }
                }

                ISN_GameSaves.ActionSavesResolved += HandleActionsResolved;
                ISN_GameSaves.Instance.ResolveConflictingSavedGames(conflictsSaves, dataLatestSavedGame);
            }
        }

        //TODO eperimental, should be fixed to be more compact with what we have
        private void LoadAfterFetch(GK_SavedGame save)
        {
            save.ActionDataLoaded += HandleActionDataLoaded;
            save.LoadData();
        }

        void HandleActionsResolved(GK_SavesResolveResult result)
        {
            ISN_GameSaves.ActionSavesResolved -= HandleActionsResolved;
            if (result.IsSucceeded)
            {
                Debug.Log("ResolveLoaded");
                SearchAndResolveConflicts(result.SavedGames);

            }
            else
            {
                Debug.Log("Failed: " + result.Error.Message);
            }
        }

        private void HandleActionDataLoaded(GK_SaveDataLoaded result)
        {
            if (result.IsSucceeded)
            {
                Debug.Log("Data loaded. data Length: " + result.SavedGame.Data.Length);
                NotifyDataLoaded(result.SavedGame.Data);
            }
            else
            {
                Debug.Log("Load Failed: " + result.Error.Message);
                NotifyDataLoadFail(result.Error.Message);
            }

        }

        private void HandleActionGameSaved(GK_SaveResult result)
        {
            ISN_GameSaves.ActionGameSaved -= HandleActionGameSaved;
            if (result.IsSucceeded)
            {
                NotifyDataSaveSuccess();
            }
            else
            {
                NotifyDataSaveFail(result.Error.Message);
            }
        }

        private GK_SavedGame FindSavedGame(List<GK_SavedGame> savedGames)
        {
            GK_SavedGame foundSavedGame = null;

            foreach (GK_SavedGame savedGame in savedGames)
            {
                Debug.Log("savedGame " + savedGame.Name);

                if (savedGame.Name == fileName)
                {
                    foundSavedGame = savedGame;
                    Debug.Log("Found the savedGame!!!");
                }
            }

            return foundSavedGame;
        }
    }
}
