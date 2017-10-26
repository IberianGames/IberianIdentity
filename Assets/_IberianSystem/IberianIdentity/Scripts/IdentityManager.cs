namespace IberianSystem
{
    using System;
    using UnityEngine;

    public class IdentityManager : Singleton<IdentityManager>, IIdentityLogin, IPersistentGame, IAchievements
    {
        public event Action OnLogInSuccess = delegate { };
        public event Action<string> OnLogInFail = delegate { };
        public event Action<LoginStatus> OnConnectionChange = delegate { };

        public event Action<object> OnGameLoadSuccess = delegate { };
        public event Action<string> OnGameLoadFail = delegate { };
        public event Action OnEmptyGameLoad = delegate { };

        public event Action OnGameSaveSuccess = delegate { };
        public event Action<string> OnGameSaveFail = delegate { };

        public event Action<object> OnSwitchComplete = delegate { };
        public event Action<string> OnSwitchFail = delegate { };

        public delegate float CalculateGameValue(object savedGame);
        public CalculateGameValue calculateGameValue;

        [SerializeField]
        string fileName;

        [SerializeField]
        string guestFileName;

        bool isGuest;
        public bool IsGuest
        {
            get { return isGuest; }
        }

        public string UserId
        {
            get { return platformLogin.UserId; }
        }

        [Header("Android Services")]
        public GameObject androidServices;
        public GameObject androidGuestServices;

        [Header("iOS Services")]
        public GameObject iOSServices;
        public GameObject iOSGuestServices;

        [Header("Standalone Services")]
        public GameObject standaloneServices;
        public GameObject standaloneGuestServices;

        private PlatformLogin platformLogin;
        private PlatformStorage platformStorage;
        private PlatformAchievements platformAchievements;

        private IdentitySwitcher identitySwitcher;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Initialize()
        {
            identitySwitcher = GetComponent<IdentitySwitcher>();
            SelectCurrentPlatform();
            WarnIfMissingPlatforms();

            platformLogin.Initialize();

            if (platformStorage != null)
            {
                platformStorage.fileName = isGuest ? guestFileName : fileName;
            }

            UnregisterEvents();
            RegisterEvents();
        }

        new void OnDestroy()
        {
            UnregisterEvents();
            base.OnDestroy();
        }

        public void InitializeAsAuthenticated()
        {
            isGuest = false;
            Initialize();
        }

        public void InitializeAsGuest()
        {
            isGuest = true;
            Initialize();
        }

        public void SwitchToAuthenticated(Type gameType)
        {
            identitySwitcher = GetComponent<IdentitySwitcher>();
            identitySwitcher.SwitchToAuthenticated(gameType);
        }

        private void OnSwitchCompleted(object savedGame)
        {
            OnSwitchComplete(savedGame);
        }

        private void OnSwitchFailed(string failMessage)
        {
            OnSwitchFail(failMessage);
        }

        #region identity login

        public void LogIn()
        {
            platformLogin.LogIn();
        }

        public void LogOut()
        {
            platformLogin.LogOut();
        }

        #endregion



        #region persistent game

        public void Load(Type gameType)
        {
            if (IsLoggedIn())
            {
                platformStorage.Load(gameType);
            }
            else
            {
                OnGameLoadFail("Not logged in"); // TODO use variable for message
            }
        }

        public void Save<T>(T game)
        {
            if (IsLoggedIn())
            {
                platformStorage.Save(game);
            }
            else
            {
                OnGameSaveFail("Not logged in"); // TODO use variable for message
            }
        }

        public void ShowSavedGames()
        {
            if (IsLoggedIn())
            {
                platformStorage.ShowSavedGames();
            }
        }

        public bool IsLoggedIn()
        {
            return platformLogin.IsLoggedIn();
        }

        #endregion



        #region achievements

        public void UnlockAchievement(string achievementId)
        {
            platformAchievements.UnlockAchievement(achievementId);
        }

        public void UpdateAchievementProgress(string achievementId, int steps, int totalSteps)
        {
            platformAchievements.UpdateAchievementProgress(achievementId, steps, totalSteps);
        }

        public void ShowAchievements()
        {
            platformAchievements.ShowAchievements();
        }

        #endregion


        void SelectCurrentPlatform()
        {
            GameObject selectedServices;

#if UNITY_EDITOR || UNITY_STANDALONE
            Debug.Log("Editor/Standalone platform");
            selectedServices = IsGuest ? standaloneGuestServices : standaloneServices;
#elif UNITY_ANDROID
            Debug.Log("Android platform");
            selectedServices = IsGuest ? androidGuestServices : androidServices;
#elif UNITY_IOS
            Debug.Log("iOS platform");
            selectedServices = IsGuest ? iOSGuestServices : iOSServices;
#endif

            platformLogin = selectedServices.GetComponent<PlatformLogin>();
            platformStorage = selectedServices.GetComponent<PlatformStorage>();
            platformAchievements = selectedServices.GetComponent<PlatformAchievements>();
        }

        private void WarnIfMissingPlatforms()
        {
            if (platformLogin == null)
            {
                Debug.LogError("No platform login service.");
            }

            if (platformStorage == null)
            {
                Debug.LogWarning("No platform storage service.");
            }

            if (platformAchievements == null)
            {
                Debug.LogWarning("No platform achievements service.");
            }
        }

        #region event wrapping 

        void RegisterEvents()
        {
            platformLogin.OnLogInSuccess += OnLogInSucceeded;
            platformLogin.OnLogInFail += OnLogInFailed;
            platformLogin.OnConnectionChange += OnConnectionChanged;

            platformStorage.OnGameLoadSuccess += OnGameLoadSucceeded;
            platformStorage.OnGameLoadFail += OnGameLoadFailed;

            platformStorage.OnGameSaveSuccess += OnGameSaveSucceeded;
            platformStorage.OnGameSaveFail += OnGameSaveFailed;
            platformStorage.OnEmptyGameLoad += OnEmptyGameLoaded;

            identitySwitcher.OnSwitchComplete += OnSwitchCompleted;
            identitySwitcher.OnSwitchFail += OnSwitchFailed;
        }

        void UnregisterEvents()
        {
            if (platformLogin != null)
            {
                platformLogin.OnLogInSuccess -= OnLogInSucceeded;
                platformLogin.OnLogInFail -= OnLogInFailed;
                platformLogin.OnConnectionChange -= OnConnectionChanged;
            }

            if (platformStorage != null)
            {
                platformStorage.OnGameLoadSuccess -= OnGameLoadSucceeded;
                platformStorage.OnGameLoadFail -= OnGameLoadFailed;

                platformStorage.OnGameSaveSuccess -= OnGameSaveSucceeded;
                platformStorage.OnGameSaveFail -= OnGameSaveFailed;
                platformStorage.OnEmptyGameLoad -= OnEmptyGameLoaded;
            }

            if (identitySwitcher != null)
            {
                identitySwitcher.OnSwitchComplete -= OnSwitchCompleted;
                identitySwitcher.OnSwitchFail -= OnSwitchFailed;
            }
        }

        void OnGameSaveSucceeded()
        {
            OnGameSaveSuccess();
        }

        void OnGameSaveFailed(string failMessage)
        {
            OnGameSaveFail(failMessage);
        }

        void OnGameLoadSucceeded(object game)
        {
            OnGameLoadSuccess(game);
        }

        void OnGameLoadFailed(string failMessage)
        {
            OnGameLoadFail(failMessage);
        }

        void OnEmptyGameLoaded()
        {
            OnEmptyGameLoad();
        }

        void OnLogInSucceeded()
        {
            OnLogInSuccess();
        }

        void OnLogInFailed(string failMessage)
        {
            OnLogInFail(failMessage);
        }

        void OnConnectionChanged(LoginStatus status)
        {
            OnConnectionChange(status);
        }

        #endregion

    }

}
