namespace IberianSystem
{
    using System;
    using System.Collections.Generic;

    public interface IAchievements
    {
        void UnlockAchievement(string achievementId);
		void UpdateAchievementProgress(string achievementId, int steps, int totalSteps);
        void ShowAchievements();
    }

}
