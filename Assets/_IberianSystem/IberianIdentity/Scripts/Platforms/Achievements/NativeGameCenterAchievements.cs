namespace IberianSystem
{
    using UnityEngine;

    public class NativeGameCenterAchievements : PlatformAchievements
    {
        public override void UpdateAchievementProgress(string achievementId, int currentSteps, int totalSteps)
        {
            float currentPercent = (100f / totalSteps) * currentSteps;

            if (currentPercent > 100f)
            {
                Debug.Log("the steps can´t be more than the totalSteps. Reducing to the max");
                currentPercent = 100.0f;
            }

            GameCenterManager.SubmitAchievement(currentPercent, achievementId);
        }

        public override void ShowAchievements()
        {
            GameCenterManager.ShowAchievements();
        }

        public override void UnlockAchievement(string achievementId)
        {
            GameCenterManager.SubmitAchievement(100f, achievementId);
        }
    }
}
