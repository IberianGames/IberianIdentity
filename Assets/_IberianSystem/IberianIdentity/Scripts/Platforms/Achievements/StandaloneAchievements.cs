namespace IberianSystem
{
    using System;
    using UnityEngine;

    public class StandaloneAchievements : PlatformAchievements
    {
        public override void UpdateAchievementProgress(string achievementId, int steps, int totalSteps)
        {
            Debug.LogWarning("UpdateAchievementProgress not implemented");
        }

        public override void ShowAchievements()
        {
            Debug.LogWarning("ShowAchievements not implemented");
        }

        public override void UnlockAchievement(string achievementId)
        {
            Debug.LogWarning("UnlockAchievement not implemented");
        }
    }
}
