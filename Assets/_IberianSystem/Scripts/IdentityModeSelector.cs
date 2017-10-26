using UnityEngine;
using IberianSystem;
using UnityEngine.SceneManagement;

public class IdentityModeSelector : MonoBehaviour {

    public string nextSceneName;

    public void OnAuthenticatedClick()
    {
        IdentityManager.Instance.InitializeAsAuthenticated();
        LoadNextScene();
    }

    public void OnGuestClick()
    {
        IdentityManager.Instance.InitializeAsGuest();
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
