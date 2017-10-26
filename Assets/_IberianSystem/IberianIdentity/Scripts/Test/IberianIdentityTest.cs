using UnityEngine;
using System.Collections;
using IberianSystem;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IberianIdentityTest : MonoBehaviour
{
    public InputField textToSave;
    public Text textToLoad;

    public Image logInLed;
    public Image logOutLed;
    public Image saveLed;
    public Image loadLed;

    public string backSceneName;

    private void OnLogInSucceeded()
    {
        Debug.Log("IberianIndetityTest.OnLoggedIn success. User id: " + IdentityManager.Instance.UserId);
        logInLed.enabled = false;


        if (logInAndLoadInprogress)
        {
            LoadAfterLogin();
        }
    }

    private void OnLogInFailed(string failMessage)
    {
        Debug.Log("IberianIndetityTest.OnLoggedIn fail: " + failMessage);
        logInLed.enabled = false;
        logInAndLoadInprogress = false;
    }

    private void LoadAfterLogin()
    {
        logInAndLoadInprogress = false;
        OnLoadClicked();
    }

    public void OnLogInClicked()
    {
        logInLed.enabled = true;
        IdentityManager.Instance.OnLogInSuccess -= OnLogInSucceeded;
        IdentityManager.Instance.OnLogInFail -= OnLogInFailed;
        IdentityManager.Instance.OnLogInSuccess += OnLogInSucceeded;
        IdentityManager.Instance.OnLogInFail += OnLogInFailed;

        IdentityManager.Instance.LogIn();
    }

    private bool logInAndLoadInprogress;
    public void OnLogInAndLoadClicked()
    {
        logInAndLoadInprogress = true;
        OnLogInClicked();
    }

    public void OnLogOutClicked()
    {
        IdentityManager.Instance.LogOut();
    }

    public void OnSaveClicked()
    {
        saveLed.enabled = true;
        FooPlayerData fooPlayerData = new FooPlayerData();
        fooPlayerData.fieldA = 2.3f;
        fooPlayerData.fieldB = 4.5f;
        fooPlayerData.fieldC = textToSave.text;

        IdentityManager.Instance.OnGameSaveSuccess -= OnGameSaveSucceeded;
        IdentityManager.Instance.OnGameSaveFail -= OnGameSaveFailed;

        IdentityManager.Instance.OnGameSaveSuccess += OnGameSaveSucceeded;
        IdentityManager.Instance.OnGameSaveFail += OnGameSaveFailed;
        IdentityManager.Instance.Save(fooPlayerData);
    }

    public void OnLoadClicked()
    {
        loadLed.enabled = true;

        IdentityManager.Instance.OnGameLoadSuccess -= OnGameLoadSucceeded;
        IdentityManager.Instance.OnGameLoadFail -= OnGameLoadFailed;
        IdentityManager.Instance.OnEmptyGameLoad -= OnEmptyGameLoaded;

        IdentityManager.Instance.OnGameLoadSuccess += OnGameLoadSucceeded;
        IdentityManager.Instance.OnGameLoadFail += OnGameLoadFailed;
        IdentityManager.Instance.OnEmptyGameLoad += OnEmptyGameLoaded;
        IdentityManager.Instance.Load(typeof(FooPlayerData));
    }

    public void OnSavedGamesClicked()
    {
        IdentityManager.Instance.ShowSavedGames();
    }

    public void OnAchievements()
    {
        IdentityManager.Instance.ShowAchievements();
    }

    public void OnUnlockAchievementClicked()
    {
        IdentityManager.Instance.UnlockAchievement(AchievementIds.achievement_test_achievement_1);
    }

    public void OnIncrementAchievementClicked()
    {
        IdentityManager.Instance.UpdateAchievementProgress(AchievementIds.achievement_test_achievement_6, 31, AchievementIds.achievement_test_achievement_6_total_steps);
        IdentityManager.Instance.UpdateAchievementProgress(AchievementIds.achievement_test_achievement_6, 32, AchievementIds.achievement_test_achievement_6_total_steps);
        IdentityManager.Instance.UpdateAchievementProgress(AchievementIds.achievement_test_achievement_6, 33, AchievementIds.achievement_test_achievement_6_total_steps);
    }

    public void OnBackClicked()
    {
        SceneManager.LoadScene(backSceneName);
    }



    public void OnSwitchToAuthClicked()
    {
        IdentityManager.Instance.OnSwitchComplete -= OnSwitchCompleted;
        IdentityManager.Instance.OnSwitchFail -= OnSwitchFailed;

        IdentityManager.Instance.OnSwitchComplete += OnSwitchCompleted;
        IdentityManager.Instance.OnSwitchFail += OnSwitchFailed;

        IdentityManager.Instance.SwitchToAuthenticated(typeof(FooPlayerData));
        IdentityManager.Instance.calculateGameValue = CalculateFooPlayerValue;
    }

    private void OnSwitchCompleted(object playerData)
    {
        Debug.Log("Switch to authenticated completed.");
        IdentityManager.Instance.OnSwitchComplete -= OnSwitchCompleted;
        IdentityManager.Instance.OnSwitchFail -= OnSwitchFailed;

        if (playerData != null)
        {
            Debug.Log("Selected game:" + ((FooPlayerData)playerData).fieldC);
        }
        else
        {
            Debug.Log("Selected game null!");
        }
    }

    private void OnSwitchFailed(string failMessage)
    {
        IdentityManager.Instance.OnSwitchComplete -= OnSwitchCompleted;
        IdentityManager.Instance.OnSwitchFail -= OnSwitchFailed;

        Debug.Log("Switch to authenticated fail: " + failMessage);
    }

    public float CalculateFooPlayerValue(object playerData)
    {
        return ((FooPlayerData)playerData).fieldC.Length;
    }

    void OnGameLoadSucceeded(object savedGame)
    {
        loadLed.enabled = false;
        Debug.Log("Load game success");
        FooPlayerData fooPlayerData = (FooPlayerData)savedGame;
        textToLoad.text = fooPlayerData.fieldC;
        Debug.Log(fooPlayerData.fieldC);
    }

    void OnGameLoadFailed(string failMessage)
    {
        loadLed.enabled = false;
        Debug.Log("Load game fail: " + failMessage);
        textToLoad.text = "Load fail: " + failMessage;
    }

    void OnEmptyGameLoaded()
    {
        loadLed.enabled = false;
        Debug.Log("Load game empty");
        textToLoad.text = "Load empty";
    }

    void OnGameSaveSucceeded()
    {
        Debug.Log("OnSavedGameSaved success");
        saveLed.enabled = false;
    }

    void OnGameSaveFailed(string failMessage)
    {
        Debug.Log("OnSavedGameSaved fail: " + failMessage);
        saveLed.enabled = false;
    }

    private void OnDestroy()
    {
        if (IdentityManager.Instance != null)
        {
            IdentityManager.Instance.OnGameLoadSuccess -= OnGameLoadSucceeded;
            IdentityManager.Instance.OnGameLoadFail -= OnGameLoadFailed;
            IdentityManager.Instance.OnEmptyGameLoad -= OnEmptyGameLoaded;

            IdentityManager.Instance.OnGameSaveSuccess -= OnGameSaveSucceeded;
            IdentityManager.Instance.OnGameSaveFail -= OnGameSaveFailed;

            IdentityManager.Instance.OnLogInSuccess -= OnLogInSucceeded;
            IdentityManager.Instance.OnLogInFail -= OnLogInFailed;
        }
    }

    [Serializable]
    public struct FooPlayerData
    {
        public float fieldA;
        public float fieldB;
        public string fieldC;
    }
}
