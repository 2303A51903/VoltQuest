using UnityEngine;
using UnityEngine.UI;

namespace BatteryRun.UI
{
    public class GameOverUI : MonoBehaviour
    {
        public Text reasonText;
        public Text scoreText;
        public Button retryButton;
        public Button mainMenuButton;

        private void Start()
        {
            if (retryButton != null) retryButton.onClick.AddListener(OnRetryClicked);
            if (mainMenuButton != null) mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }

        public void PopulateDetails(string reason = "Battery Depleted")
        {
            if (reasonText != null) reasonText.text = reason.ToUpper();
            if (scoreText != null && Managers.ScoreManager.Instance != null)
            {
                scoreText.text = string.Format("FINAL SCORE: {0:N0}", Managers.ScoreManager.Instance.currentLevelScore);
            }
        }

        private void OnRetryClicked()
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