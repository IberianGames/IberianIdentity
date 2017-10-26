using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePanel : MonoBehaviour {

	public void LoadAdsManagerTest()
    {
        SceneManager.LoadScene("IberianAdsTest");
    }
	
	public void LoadIberiandentityTest()
    {
        SceneManager.LoadScene("IberianIdentityTest");
    }        
}
