namespace IberianSystem
{
    using System;
    using UnityEngine;

    public abstract class PlatformLogin : MonoBehaviour, IIdentityLogin
    {


        public abstract event Action OnLogInSuccess;
        public abstract event Action<string> OnLogInFail;
        public abstract event Action<LoginStatus> OnConnectionChange;

        public abstract string UserId { get; }

        public abstract bool IsLoggedIn();
        public abstract void Initialize();
        public abstract void LogIn();
        public abstract void LogOut();
    }
}
