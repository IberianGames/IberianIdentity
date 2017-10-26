using UnityEngine;
using System.Collections;

public class AchievementIds : MonoBehaviour {

#if UNITY_ANDROID
    public const string achievement_test_achievement_2 = "CgkIsozo-oEKEAIQAg"; // <GPGSID>
    public const string achievement_test_achievement_3 = "CgkIsozo-oEKEAIQAw"; // <GPGSID>
    public const string achievement_test_achievement_4 = "CgkIsozo-oEKEAIQBA"; // <GPGSID>
    public const string achievement_test_achievement_5 = "CgkIsozo-oEKEAIQBQ"; // <GPGSID>
    public const string achievement_test_achievement_6 = "CgkIsozo-oEKEAIQCA"; // <GPGSID>
    public const string achievement_test_achievement_1 = "CgkIsozo-oEKEAIQAQ"; // <GPGSID>
#endif

#if UNITY_IOS
    public const string achievement_test_achievement_2 = "test_achievement_2"; // <GPGSID>
    public const string achievement_test_achievement_3 = "test_achievement_3"; // <GPGSID>
    public const string achievement_test_achievement_4 = "test_achievement_4"; // <GPGSID>
    public const string achievement_test_achievement_5 = "test_achievement_5"; // <GPGSID>
    public const string achievement_test_achievement_6 = "test_achievement_6"; // <GPGSID>
    public const string achievement_test_achievement_1 = "test_achievement_1"; // <GPGSID>
#endif

    public const int achievement_test_achievement_6_total_steps = 50;
    public const int achievement_test_achievement_5_total_steps = 50;
}
