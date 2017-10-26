using UnityEngine;
using IberianSystem;
using System;
using UnityEngine.UI;

public class LoginStatusWatcher : MonoBehaviour
{

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        IdentityManager.Instance.OnConnectionChange += OnLogInChanged;
        if (IdentityManager.Instance.IsLoggedIn())
        {
            SetAsConnected();
        }
        else
        {
            SetAsDisconnected();
        }
    }

    private void OnLogInChanged(IberianSystem.LoginStatus loginStatus)
    {
        switch (loginStatus)
        {
            case IberianSystem.LoginStatus.Connected:
                SetAsConnected();
                break;
            case IberianSystem.LoginStatus.Disconnected:
                SetAsDisconnected();
                break;
        }
    }

    private void SetAsConnected()
    {
        GetComponent<Text>().text = "CONNECTED";
        GetComponent<Text>().color = Color.green;
    }


    private void SetAsDisconnected()
    {
        GetComponent<Text>().text = "DISCONNECTED";
        GetComponent<Text>().color = Color.red;
    }

    private void OnDisable()
    {
        IdentityManager.Instance.OnConnectionChange -= OnLogInChanged;
    }
}
