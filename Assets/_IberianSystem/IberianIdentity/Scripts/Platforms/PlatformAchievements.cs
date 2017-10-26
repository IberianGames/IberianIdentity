namespace IberianSystem
{
    using UnityEngine;

    public abstract class PlatformAchievements : MonoBehaviour, IAchievements
    {
        public abstract void UnlockAchievement(string achievementId);
        public abstract void UpdateAchievementProgress(string achievementId, int steps, int totalSteps);
        public abstract void ShowAchievements();
    }
}
