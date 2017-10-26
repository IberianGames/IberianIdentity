namespace IberianSystem
{
    using System;
    using System.Collections;
    using UnityEngine;

    public class StandaloneLogin : PlatformLogin
    {
        const string UserIdPrefsKey = "STANDALONE_LOGIN_USER_ID";

        public float artificialDelay;
        public float artificialErrorChance;

        public override event Action OnLogInSuccess = delegate { };
        public override event Action<string> OnLogInFail = delegate { };
        public override event Action<LoginStatus> OnConnectionChange = delegate { };

        public override string UserId
        {
            get
            {
                string modifiedUserIdPrefsKey = UserIdPrefsKey + IdentityManager.Instance.IsGuest;

                string savedUserId = PlayerPrefs.GetString(modifiedUserIdPrefsKey);
                if (savedUserId == "")
                {
                    savedUserId = GenerateRandomId();
                    PlayerPrefs.SetString(modifiedUserIdPrefsKey, savedUserId);
                }

                return savedUserId;
            }
        }



        bool isLoggedIn;

        public override void Initialize()
        {
            isLoggedIn = false;

            StartCoroutine(
                WatchForConnectionChange());
        }

        public override void LogIn()
        {
            StartCoroutine(
                OnLogInDelayed());
        }

        public override void LogOut()
        {
            isLoggedIn = false;
        }

        public override bool IsLoggedIn()
        {
            return isLoggedIn;
        }

        string GenerateRandomId()
        {
            return
                "user-" +
                UnityEngine.Random.Range(1000, 9999) + "-" +
                UnityEngine.Random.Range(1000, 9999) + "-" +
                UnityEngine.Random.Range(1000, 9999) + "-" +
                UnityEngine.Random.Range(1000, 9999) + "-" +
                (IdentityManager.Instance.IsGuest ? "1" : "0");
        }

        IEnumerator WatchForConnectionChange()
        {
            bool oldIsLoggedIn = false;
            while (true)
            {
                if (oldIsLoggedIn != isLoggedIn)
                {
                    if (isLoggedIn)
                    {
                        OnConnectionChange(LoginStatus.Connected);
                    }
                    else
                    {
                        OnConnectionChange(LoginStatus.Disconnected);
                    }

                }

                oldIsLoggedIn = isLoggedIn;
                yield return null;
            }
        }

        IEnumerator OnLogInDelayed()
        {
            yield return new WaitForSeconds(artificialDelay);

            if (!IsArtificialErrorProduced())
            {
                isLoggedIn = true;
                OnLogInSuccess();
            }
            else
            {
                isLoggedIn = false;
                OnLogInFail("Artificial error");
            }
        }

        bool IsArtificialErrorProduced()
        {
            return UnityEngine.Random.Range(0f, 1f) <= artificialErrorChance;
        }
    }
}
