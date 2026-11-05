using UnityEngine;
using UnityEngine.UI;

namespace BatteryRun.UI
{
    public class VictoryUI : MonoBehaviour
    {
        public Text congratulationsText;
        public Text totalScoreText;
        public Text totalStarsText;
        public Button playAgainButton;
        public Button mainMenuButton;

        private void Start()
        {
            if (playAgainButton != null) playAgainButton.onClick.AddListener(OnPlayAgainClicked);
            if (mainMenuButton != null) mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }

        public void PopulateDetails()
        {
            int totalScore = SaveSystem.SaveManager.GetTotalScore();
            int totalStars = SaveSystem.SaveManager.GetTotalStars();

            if (congratulationsText != null)
            {
                congratulationsText.text = "ISLAND ENERGY RESTORED!\nBOLT SUCCESSFULLY ESCAPED!";
            }

            if (totalScoreText != null)
            {
                totalScoreText.text = string.Format("CUMULATIVE SCORE: {0:N0}", totalScore);
            }

            if (totalStarsText != null)
            {
                totalStarsText.text = string.Format("TOTAL STARS EARNED: {0} / 12", totalStars);
            }
        }

        private void OnPlayAgainClicked()
        {
            Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            Managers.GameManager.Instance?.LoadLevel(1);
        }

        private void OnMainMenuClicked()
        {
            Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            Managers.GameManager.Instance?.LoadMainMenu();
        }
    }
}