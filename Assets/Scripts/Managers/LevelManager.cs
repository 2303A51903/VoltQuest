using System;
using UnityEngine;

namespace BatteryRun.Managers
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [Header("Level Info")]
        public int levelNumber = 1;
        public string levelTitle = "The Lost Island";
        public string levelObjective = "Collect 5 Batteries & Reach the Beacon";

        [Header("Collectible Targets")]
        public int totalBatteriesInLevel = 5;
        public int totalCrystalsInLevel = 1;

        [Header("Status")]
        public int batteriesCollected = 0;
        public int crystalsCollected = 0;
        public float levelTimer = 0f;
        public int deathCount = 0;
        public bool isExitUnlocked = false;

        public event Action<int, int> OnBatteryCollected;
        public event Action<int, int> OnCrystalCollected;
        public event Action OnExitUnlocked;
        public event Action<string> OnMessage;
        private float lastHintTime = -10f;

        public void ShowMessage(string msg)
        {
            OnMessage?.Invoke(msg);
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            ConfigureLevelDefaults();
            ScoreManager.Instance?.ResetLevelScore(); // score was leaking between levels/retries
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.currentState == GameState.Playing)
            {
                levelTimer += Time.deltaTime;
            }
        }

        private void ConfigureLevelDefaults()
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (sceneName.Contains("1") || levelNumber == 1)
            {
                levelNumber = 1;
                levelTitle = "The Lost Island";
                totalBatteriesInLevel = 5;
                totalCrystalsInLevel = 1;
                levelObjective = "Collect 5 Batteries & reach the exit checkpoint!";
            }
            else if (sceneName.Contains("2") || levelNumber == 2)
            {
                levelNumber = 2;
                levelTitle = "Dangerous Jungle";
                totalBatteriesInLevel = 8;
                totalCrystalsInLevel = 2;
                levelObjective = "Collect 8 Batteries, avoid traps & reach the portal!";
            }
            else if (sceneName.Contains("3") || levelNumber == 3)
            {
                levelNumber = 3;
                levelTitle = "Crystal Cave";
                totalBatteriesInLevel = 10;
                totalCrystalsInLevel = 3;
                levelObjective = "Collect 10 Batteries, step on pedestals RED > BLUE > GREEN, escape!";
            }
            else if (sceneName.Contains("4") || levelNumber == 4)
            {
                levelNumber = 4;
                levelTitle = "The Final Energy Core";
                totalBatteriesInLevel = 12;
                totalCrystalsInLevel = 4;
                levelObjective = "Collect all energy cells & recharge the island core!";
            }
        }

        public void RecordBatteryCollected()
        {
            batteriesCollected++;
            OnBatteryCollected?.Invoke(batteriesCollected, totalBatteriesInLevel);

            if (batteriesCollected >= totalBatteriesInLevel && !isExitUnlocked)
            {
                isExitUnlocked = true;
                ScoreManager.Instance?.AddObjectiveBonus(300);
                Audio.SoundManager.Instance?.PlaySFX("DoorUnlock");
                OnExitUnlocked?.Invoke();
            }
        }

        public void RecordCrystalCollected()
        {
            crystalsCollected++;
            OnCrystalCollected?.Invoke(crystalsCollected, totalCrystalsInLevel);
        }

        public void RecordPlayerDeath()
        {
            deathCount++;
        }

        public int CalculateStars(float batteryRemaining, out int bonusScore)
        {
            bonusScore = 0;
            // Finish Level base bonus
            bonusScore += 500;

            // No death bonus
            if (deathCount == 0) bonusScore += 200;

            // Remaining battery > 50% bonus
            if (batteryRemaining >= 50.0f) bonusScore += 200;

            int stars = 1; // Completed level gets at least 1 star
            bool collectedAllBatteries = batteriesCollected >= totalBatteriesInLevel;
            bool collectedAnyCrystal = crystalsCollected >= 1;
            bool collectedAllCrystals = crystalsCollected >= totalCrystalsInLevel;

            if (collectedAllBatteries && collectedAnyCrystal)
            {
                stars = 2;
            }

            if (collectedAllBatteries && collectedAllCrystals && (deathCount == 0 || batteryRemaining >= 50f))
            {
                stars = 3;
            }

            return stars;
        }

        public void ReachLevelGoal()
        {
            if (GameManager.Instance != null && GameManager.Instance.currentState != GameState.Playing) return;

            if (!isExitUnlocked && batteriesCollected < totalBatteriesInLevel)
            {
                if (Time.unscaledTime - lastHintTime > 2.5f)
                {
                    lastHintTime = Time.unscaledTime;
                    ShowMessage("Collect all batteries first! (" + batteriesCollected + "/" + totalBatteriesInLevel + ")");
                }
                return;
            }

            if (levelNumber >= 4)
            {
                var gen = FindObjectOfType<Environment.EnergyGenerator>();
                if (gen != null) gen.ActivateCore();
            }

            var player = FindObjectOfType<Player.BatterySystem>();
            float batteryRemaining = player != null ? player.BatteryPercentage : 0f;

            int bonusScore;
            int stars = CalculateStars(batteryRemaining, out bonusScore);

            ScoreManager.Instance?.AddScore(bonusScore);
            ScoreManager.Instance?.FinalizeLevelScore(levelNumber, stars);

            if (levelNumber >= 4)
            {
                GameManager.Instance?.TriggerVictory();
            }
            else
            {
                GameManager.Instance?.TriggerLevelComplete();
            }
        }
    }
}