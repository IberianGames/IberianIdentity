using System.Collections;
using UnityEngine;
using IberianSystem;
using UnityEngine.Networking;

public class IberianGamesServerStore : PlatformStorage
{
    public string loadUrl;
    public string saveUrl;

    const string UserIdFieldName = "UserId";
    const string GameBundleFieldName = "GameBundle";
    const string DataFieldName = "Data";

    public override void ShowSavedGames()
    {
        // Do nothing
    }

    protected override void LoadData()
    {
        StartCoroutine(
            StartDownloadingGameData());
    }

    protected override void SaveData(byte[] data)
    {
        StartCoroutine(
            StartUploadingGameData(data));
    }



    IEnumerator StartDownloadingGameData()
    {
        WWWForm form = ConfigureDownloadRequestForm();

        var webRequest = UnityWebRequest.Post(loadUrl, form);
        yield return webRequest.Send();

        HandleDowloadResponse(webRequest);
    }

    IEnumerator StartUploadingGameData(byte[] data)
    {
        WWWForm form = ConfigureUploadRequestForm(data);

        UnityWebRequest webRequest = UnityWebRequest.Post(saveUrl, form);
        yield return webRequest.Send();

        HandleUploadResponse(webRequest);
    }



    WWWForm ConfigureDownloadRequestForm()
    {
        return ConfigureBasicRequestForm();
    }

    WWWForm ConfigureUploadRequestForm(byte[] data)
    {
        WWWForm form = ConfigureBasicRequestForm();

        string textGameData = System.Text.Encoding.UTF8.GetString(data);
        form.AddField(DataFieldName, textGameData);

        return form;
    }

    WWWForm ConfigureBasicRequestForm()
    {
        string userId = GetComponent<PlatformLogin>().UserId;
        string gameBundleId = Application.identifier;

        WWWForm form = new WWWForm();

        form.AddField(UserIdFieldName, userId);
        form.AddField(GameBundleFieldName, gameBundleId);

        return form;
    }



    void HandleDowloadResponse(UnityWebRequest webRequest)
    {
        if (!webRequest.isError)
        {
            string textGameData = webRequest.downloadHandler.text;
            if (textGameData != "")
            {
                byte[] gameData = System.Text.Encoding.UTF8.GetBytes(textGameData);
                NotifyDataLoaded(gameData);
            }
            else
            {
                NotifyDataLoadEmpty();
            }
        }
        else
        {
            NotifyDataLoadFail(webRequest.error);
        }
    }

    void HandleUploadResponse(UnityWebRequest webRequest)
    {
        if (!webRequest.isError)
        {
            NotifyDataSaveSuccess();
        }
        else
        {
            NotifyDataSaveFail(webRequest.error);
        }
    }
}
