namespace IberianSystem
{
    using System.Collections.Generic;
    using UnityEngine;

    public class NativeGooglePlayAchievements : PlatformAchievements
    {
        class AchievementNotificationRequest
        {
            public string id;
            public int currentSteps;
            public int totalSteps;
        }

        Queue<AchievementNotificationRequest> pendingNotifications = new Queue<AchievementNotificationRequest>();

        AchievementNotificationRequest currentNotification;

        bool isNotifiyingAchievement = false;


        public override void UpdateAchievementProgress(string achievementId, int currentSteps, int totalSteps)
        {
            var notificationRequest = new AchievementNotificationRequest();
            notificationRequest.id = achievementId;
            notificationRequest.currentSteps = currentSteps;
            notificationRequest.totalSteps = totalSteps;

            if (isNotifiyingAchievement)
            {
                Debug.Log("isNotifiyingAchievement");
                pendingNotifications.Enqueue(notificationRequest);
            }
            else
            {
                isNotifiyingAchievement = true;
                NotifyAchievementUpdate(notificationRequest);
            }
        }



        public override void ShowAchievements()
        {
            GooglePlayManager.Instance.ShowAchievementsUI();
        }

        public override void UnlockAchievement(string achievementId)
        {
            GooglePlayManager.Instance.UnlockAchievementById(achievementId);
        }

        void NotifyAchievementUpdate(AchievementNotificationRequest notificationRequest)
        {
            Debug.Log("NotifyAchievementUpdate");
            currentNotification = notificationRequest;

            GooglePlayManager.ActionAchievementsLoaded -= ActionAchievementsLoaded;
            GooglePlayManager.ActionAchievementsLoaded += ActionAchievementsLoaded;
            GooglePlayManager.Instance.LoadAchievements();
        }

        void ActionAchievementsLoaded(GooglePlayResult result)
        {
            Debug.Log("ActionAchievementsLoaded");
            GooglePlayManager.ActionAchievementsLoaded -= ActionAchievementsLoaded;

            GPAchievement loadedAchievement = GooglePlayManager.Instance.GetAchievement(currentNotification.id);

            if (result.IsSucceeded)
            {
                if(loadedAchievement.TotalSteps != currentNotification.totalSteps)
                {
                    Debug.LogWarning("Total steps mismatch for achievement " + loadedAchievement.Name +". Id: " +loadedAchievement.Id + 
                        ". Expected steps:" + currentNotification.totalSteps + ". Declared steps: " + loadedAchievement.TotalSteps);
                }

                int stepIncrement = currentNotification.currentSteps - loadedAchievement.CurrentSteps;
                if (stepIncrement > 0)
                {
                    GooglePlayManager.ActionAchievementUpdated -= ActionAchievementUpdated;
                    GooglePlayManager.ActionAchievementUpdated += ActionAchievementUpdated;
                    GooglePlayManager.Instance.IncrementAchievementById(currentNotification.id, stepIncrement);
                }
                else
                {
                    Debug.LogWarning("Achievement " + loadedAchievement.Name + " not updated. Current progress is not lower than requested.");
                    DispatchNextNotification();
                }
            }
            else
            {
                DispatchNextNotification();
            }

        }

        void ActionAchievementUpdated(GP_AchievementResult result)
        {
            Debug.Log("ActionAchievementUpdated");
            GooglePlayManager.ActionAchievementUpdated -= ActionAchievementUpdated;
            DispatchNextNotification();
        }

        void DispatchNextNotification()
        {
            Debug.Log("DispatchNextNotification");
            if (pendingNotifications.Count > 0)
            {
                NotifyAchievementUpdate(pendingNotifications.Dequeue());
            }
            else
            {
                isNotifiyingAchievement = false;
            }
        }
    }
}
