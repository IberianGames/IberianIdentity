namespace IberianSystem
{
    using System;

    public enum LoginStatus
    {
        Connected = 0,
        Disconnected = 1
    }

    public interface IIdentityLogin
    {
        event Action OnLogInSuccess;
        event Action<string> OnLogInFail;
        event Action<LoginStatus> OnConnectionChange;

        string UserId { get; }

        bool IsLoggedIn();
        void LogIn();
        void LogOut();
    }
}
