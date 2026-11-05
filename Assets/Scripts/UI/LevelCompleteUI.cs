using UnityEngine;
using UnityEngine.UI;

namespace BatteryRun.UI
{
    public class LevelCompleteUI : MonoBehaviour
    {
        [Header("Stats Display")]
        public Text titleText;
        public Text starsText;
        public Text finalScoreText;
        public Text batteriesText;
        public Text crystalsText;
        public Text timeText;

        [Header("Buttons")]
        public Button nextLevelButton;
        public Button replayButton;
        public Button mainMenuButton;

        private void Start()
        {
            if (nextLevelButton != null) nextLevelButton.onClick.AddListener(OnNextLevelClicked);
            if (replayButton != null) replayButton.onClick.AddListener(OnReplayClicked);
            if (mainMenuButton != null) mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }

        public void PopulateDetails()
        {
            var lm = Managers.LevelManager.Instance;
            var sm = Managers.ScoreManager.Instance;
            var player = FindObjectOfType<Player.BatterySystem>();

            if (lm != null)
            {
                if (titleText != null)
                {
                    titleText.text = "LEVEL " + lm.levelNumber + " COMPLETE!";
                }

                float batteryRemaining = player != null ? player.BatteryPercentage : 0f;
                int bonus;
                int stars = lm.CalculateStars(batteryRemaining, out bonus);

                if (starsText != null)
                {
                    string s = "";
                    for (int i = 0; i < 3; i++) s += (i < stars) ? "★ " : "☆ ";
                    starsText.text = s.Trim();
                }

                if (finalScoreText != null && sm != null)
                {
                    finalScoreText.text = string.Format("LEVEL SCORE: {0:N0}", sm.currentLevelScore);
                }

                if (batteriesText != null)
                {
                    batteriesText.text = string.Format("BATTERIES: {0} / {1}", lm.batteriesCollected, lm.totalBatteriesInLevel);
                }

                if (crystalsText != null)
                {
                    crystalsText.text = string.Format("CRYSTALS: {0} / {1}", lm.crystalsCollected, lm.totalCrystalsInLevel);
                }

                if (timeText != null)
                {
                    int mins = (int)(lm.levelTimer / 60);
                    int secs = (int)(lm.levelTimer % 60);
                    timeText.text = string.Format("TIME TAKEN: {0:00}:{1:00}", mins, secs);
                }

                if (nextLevelButton != null)
                {
                    nextLevelButton.gameObject.SetActive(lm.levelNumber < 4);
                }
            }
        }

        private void OnNextLevelClicked()
        {
            Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            Managers.GameManager.Instance?.LoadNextLevel();
        }

        private void OnReplayClicked()
        {
            Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            Managers.GameManager.Instance?.RestartCurrentLevel();
        }

        private void OnMainMenuClicked()
        {
            Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            Managers.GameManager.Instance?.LoadMainMenu();
        }
    }
}