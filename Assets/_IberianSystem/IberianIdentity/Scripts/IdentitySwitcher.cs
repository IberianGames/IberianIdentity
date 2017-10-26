namespace IberianSystem
{
    using UnityEngine;
    using System;

    public class IdentitySwitcher : MonoBehaviour
    {
        public event Action<object> OnSwitchComplete = delegate { };
        public event Action<string> OnSwitchFail = delegate { };

        object guestGame;
        private bool guestGameIsEmpty;

        Type gameType;
        public void SwitchToAuthenticated(Type gameType)
        {
            this.gameType = gameType;

            UnregisterGuestGameLoadEvents();
            RegisterGuestGameLoadEvents();

            IdentityManager.Instance.Load(gameType);
        }

        void OnGuestGameLoadSucceeded(object loadedGame)
        {
            UnregisterGuestGameLoadEvents();
            guestGame = loadedGame;
            guestGameIsEmpty = false;
            LogOutGuestLogInAuth();
        }

        private void OnGuestGameLoadFailed(string failMessage)
        {
            UnregisterGuestGameLoadEvents();
            OnSwitchFail("Guest load fail: " + failMessage);
        }

        private void OnGuestEmptyGameLoaded()
        {
            UnregisterGuestGameLoadEvents();
            guestGameIsEmpty = true;
            LogOutGuestLogInAuth();
        }

        private void LogOutGuestLogInAuth()
        {
            IdentityManager.Instance.LogOut();
            IdentityManager.Instance.InitializeAsAuthenticated();

            UnregisterLoginEvents();
            RegisterLoginEvents();

            IdentityManager.Instance.LogIn();
        }

        private void OnLogInSucceeded()
        {
            UnregisterAuthGameLoadEvents();
            RegisterAuthGameLoadEvents();

            IdentityManager.Instance.Load(gameType);
        }

        private void OnLogInFailed(string failMessage)
        {
            RevertToGuest();
            OnSwitchFail("Authenticated login fail: " + failMessage);
        }


        void OnAuthGameLoadSucceeded(object loadedAuthGame)
        {
            UnregisterAuthGameLoadEvents();
            object mostValuableGame = loadedAuthGame;

            if (!guestGameIsEmpty)
            {
                float authValue = IdentityManager.Instance.calculateGameValue(loadedAuthGame);
                float guestValue = IdentityManager.Instance.calculateGameValue(guestGame);

                mostValuableGame = (authValue >= guestValue) ? loadedAuthGame : guestGame;
            }

            OnSwitchComplete(mostValuableGame);
        }


        private void OnAuthGameLoadFailed(string failMessage)
        {
            UnregisterAuthGameLoadEvents();
            RevertToGuest();
            OnSwitchFail("Authenticated login fail: " + failMessage );
        }

        private void OnAuthEmptyGameLoaded()
        {
            UnregisterAuthGameLoadEvents();
            if (!guestGameIsEmpty)
            {
                OnSwitchComplete(guestGame);
            }
            else
            {
                OnSwitchComplete(null);
            }
        }

        private void RevertToGuest()
        {
            IdentityManager.Instance.LogOut();

            IdentityManager.Instance.InitializeAsGuest();
            IdentityManager.Instance.LogIn();
        }

        private void RegisterGuestGameLoadEvents()
        {
            IdentityManager.Instance.OnGameLoadSuccess += OnGuestGameLoadSucceeded;
            IdentityManager.Instance.OnGameLoadFail += OnGuestGameLoadFailed;
            IdentityManager.Instance.OnEmptyGameLoad += OnGuestEmptyGameLoaded;
        }

        private void UnregisterGuestGameLoadEvents()
        {
            IdentityManager.Instance.OnGameLoadSuccess -= OnGuestGameLoadSucceeded;
            IdentityManager.Instance.OnGameLoadFail -= OnGuestGameLoadFailed;
            IdentityManager.Instance.OnEmptyGameLoad -= OnGuestEmptyGameLoaded;
        }

        private void RegisterLoginEvents()
        {
            IdentityManager.Instance.OnLogInSuccess += OnLogInSucceeded;
            IdentityManager.Instance.OnLogInFail += OnLogInFailed;
        }

        private void UnregisterLoginEvents()
        {
            IdentityManager.Instance.OnLogInSuccess -= OnLogInSucceeded;
            IdentityManager.Instance.OnLogInFail -= OnLogInFailed;
        }

        private void RegisterAuthGameLoadEvents()
        {
            IdentityManager.Instance.OnGameLoadSuccess += OnAuthGameLoadSucceeded;
            IdentityManager.Instance.OnGameLoadFail += OnAuthGameLoadFailed;
            IdentityManager.Instance.OnEmptyGameLoad += OnAuthEmptyGameLoaded;
        }

        private void UnregisterAuthGameLoadEvents()
        {
            IdentityManager.Instance.OnGameLoadSuccess -= OnAuthGameLoadSucceeded;
            IdentityManager.Instance.OnGameLoadFail -= OnAuthGameLoadFailed;
            IdentityManager.Instance.OnEmptyGameLoad -= OnAuthEmptyGameLoaded;
        }

    }
}
