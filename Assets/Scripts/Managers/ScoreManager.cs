using System;
using UnityEngine;

namespace BatteryRun.Managers
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [Header("Scores")]
        public int currentLevelScore = 0;
        public int totalCumulativeScore = 0;

        public event Action<int> OnScoreChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Load saved total score
            totalCumulativeScore = SaveSystem.SaveManager.GetTotalScore();
        }

        public void ResetLevelScore()
        {
            currentLevelScore = 0;
            OnScoreChanged?.Invoke(currentLevelScore);
        }

        public void AddScore(int amount)
        {
            currentLevelScore += amount;
            totalCumulativeScore += amount;
            OnScoreChanged?.Invoke(currentLevelScore);
        }

        public void AddObjectiveBonus(int amount = 300)
        {
            AddScore(amount);
        }

        public void FinalizeLevelScore(int level, int stars)
        {
            // Save level highscore and stars
            SaveSystem.SaveManager.SaveLevelPerformance(level, currentLevelScore, stars);
            totalCumulativeScore = SaveSystem.SaveManager.GetTotalScore();
        }
    }
}