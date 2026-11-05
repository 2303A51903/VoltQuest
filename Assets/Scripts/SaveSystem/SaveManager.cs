using UnityEngine;

namespace BatteryRun.SaveSystem
{
    public static class SaveManager
    {
        private const string KEY_UNLOCKED_LEVEL = "BatteryRun_UnlockedLevel";
        private const string KEY_SCORE_PREFIX = "BatteryRun_Score_Level_";
        private const string KEY_STARS_PREFIX = "BatteryRun_Stars_Level_";
        private const string KEY_VOL_MASTER = "BatteryRun_VolMaster";
        private const string KEY_VOL_MUSIC = "BatteryRun_VolMusic";
        private const string KEY_VOL_SFX = "BatteryRun_VolSFX";
        private const string KEY_MUTED = "BatteryRun_Muted";

        public static int GetUnlockedLevel()
        {
            return PlayerPrefs.GetInt(KEY_UNLOCKED_LEVEL, 1);
        }

        public static void UnlockNextLevel(int completedLevel)
        {
            int current = GetUnlockedLevel();
            if (completedLevel + 1 > current)
            {
                PlayerPrefs.SetInt(KEY_UNLOCKED_LEVEL, Mathf.Clamp(completedLevel + 1, 1, 4));
                PlayerPrefs.Save();
            }
        }

        public static void SaveLevelPerformance(int level, int score, int stars)
        {
            int oldScore = PlayerPrefs.GetInt(KEY_SCORE_PREFIX + level, 0);
            if (score > oldScore)
            {
                PlayerPrefs.SetInt(KEY_SCORE_PREFIX + level, score);
            }

            int oldStars = PlayerPrefs.GetInt(KEY_STARS_PREFIX + level, 0);
            if (stars > oldStars)
            {
                PlayerPrefs.SetInt(KEY_STARS_PREFIX + level, stars);
            }

            UnlockNextLevel(level);
            PlayerPrefs.Save();
        }

        public static int GetLevelHighScore(int level)
        {
            return PlayerPrefs.GetInt(KEY_SCORE_PREFIX + level, 0);
        }

        public static int GetLevelStars(int level)
        {
            return PlayerPrefs.GetInt(KEY_STARS_PREFIX + level, 0);
        }

        public static int GetTotalScore()
        {
            int total = 0;
            for (int i = 1; i <= 4; i++)
            {
                total += GetLevelHighScore(i);
            }
            return total;
        }

        public static int GetTotalStars()
        {
            int total = 0;
            for (int i = 1; i <= 4; i++)
            {
                total += GetLevelStars(i);
            }
            return total;
        }

        public static float GetMasterVolume() => PlayerPrefs.GetFloat(KEY_VOL_MASTER, 1.0f);
        public static void SetMasterVolume(float v) { PlayerPrefs.SetFloat(KEY_VOL_MASTER, v); PlayerPrefs.Save(); }

        public static float GetMusicVolume() => PlayerPrefs.GetFloat(KEY_VOL_MUSIC, 0.8f);
        public static void SetMusicVolume(float v) { PlayerPrefs.SetFloat(KEY_VOL_MUSIC, v); PlayerPrefs.Save(); }

        public static float GetSFXVolume() => PlayerPrefs.GetFloat(KEY_VOL_SFX, 0.9f);
        public static void SetSFXVolume(float v) { PlayerPrefs.SetFloat(KEY_VOL_SFX, v); PlayerPrefs.Save(); }

        public static bool IsMuted() => PlayerPrefs.GetInt(KEY_MUTED, 0) == 1;
        public static void SetMuted(bool muted) { PlayerPrefs.SetInt(KEY_MUTED, muted ? 1 : 0); PlayerPrefs.Save(); }

        public static void ResetAllProgress()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}