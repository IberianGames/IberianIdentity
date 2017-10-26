namespace IberianSystem
{
    using System.Collections;
    using System;
    using SA.Common.Models;
    using UnityEngine;

    public class NativeGameCenterLogin : PlatformLogin
    {
        public override event Action OnLogInSuccess = delegate { };
        public override event Action<string> OnLogInFail = delegate { };
        public override event Action<LoginStatus> OnConnectionChange = delegate { };

        public override string UserId
        {
            get { return GameCenterManager.Player.Id; }
        }

        public override void Initialize()
        {
            StartCoroutine(
                WatchForConnectionChange());
        }

        public override void LogIn()
        {
            GameCenterManager.OnAuthFinished += OnAuthFinished;
            GameCenterManager.Init();
        }

        public override void LogOut()
        {
            // do nothing
            Debug.LogWarning("LogOut called. Game Center has no connect/disconnect functionality");
        }

        public override bool IsLoggedIn()
        {
            return GameCenterManager.IsPlayerAuthenticated;
        }

        private IEnumerator WatchForConnectionChange()
        {
            bool oldIsPlayerAuthenticated = false;
            while (true)
            {
                if (oldIsPlayerAuthenticated != GameCenterManager.IsPlayerAuthenticated)
                {
                    if (GameCenterManager.IsPlayerAuthenticated)
                    {
                        OnConnectionChange(LoginStatus.Connected);
                    }
                    else
                    {
                        OnConnectionChange(LoginStatus.Disconnected);
                    }

                }

                oldIsPlayerAuthenticated = GameCenterManager.IsPlayerAuthenticated;
                yield return null;
            }
        }

        private void OnAuthFinished(Result result)
        {
            GameCenterManager.OnAuthFinished -= OnAuthFinished;

            if (result.IsSucceeded)
            {
                OnLogInSuccess();
            }
            else
            {
                OnLogInFail(result.Error.Message);
            }
        }


    }
}
