namespace IberianSystem
{
    using System;

    public class NativeGooglePlayLogin : PlatformLogin
    {
        public override event Action OnLogInSuccess = delegate { };
        public override event Action<string> OnLogInFail = delegate { };
        public override event Action<LoginStatus> OnConnectionChange = delegate { };

        public override string UserId
        {
            get { return GooglePlayManager.Instance.player.playerId; }
        }

        public override void Initialize()
        {
            GooglePlayConnection.ActionConnectionStateChanged -= ActionConnectionStateChanged;
            GooglePlayConnection.ActionConnectionStateChanged += ActionConnectionStateChanged;
        }

        public override void LogIn()
        {
            GooglePlayConnection.ActionConnectionResultReceived -= ActionConnectionResultReceived;
            GooglePlayConnection.ActionConnectionResultReceived += ActionConnectionResultReceived;
            GooglePlayConnection.Instance.Connect();
        }

        public override void LogOut()
        {
            GooglePlayConnection.Instance.Disconnect();
        }

        public override bool IsLoggedIn()
        {
            return GooglePlayConnection.Instance.IsConnected;
        }

        void ActionConnectionResultReceived(GooglePlayConnectionResult result)
        {
            GooglePlayConnection.ActionConnectionResultReceived -= ActionConnectionResultReceived;

            if (result.IsSuccess)
            {
                OnLogInSuccess();
            }
            else
            {
                OnLogInFail("Login fail code: " + result.code);
            }
        }

        void ActionConnectionStateChanged(GPConnectionState connectionState)
        {
            if (connectionState == GPConnectionState.STATE_CONNECTED)
            {
                OnConnectionChange(LoginStatus.Connected);
            }

            else if (connectionState == GPConnectionState.STATE_DISCONNECTED)
            {
                OnConnectionChange(LoginStatus.Disconnected);
            }

            else if (connectionState == GPConnectionState.STATE_UNCONFIGURED)
            {
                OnConnectionChange(LoginStatus.Disconnected);
            }
        }

    }

}
