using UnityEngine;
using UnityEngine.UI;

namespace BatteryRun.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        public Button resumeButton;
        public Button restartButton;
        public Button settingsButton;
        public Button mainMenuButton;
        public GameObject settingsModal;

        private void Start()
        {
            if (resumeButton != null) resumeButton.onClick.AddListener(OnResumeClicked);
            if (restartButton != null) restartButton.onClick.AddListener(OnRestartClicked);
            if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsClicked);
            if (mainMenuButton != null) mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }

        private void OnResumeClicked()
        {
            Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            Managers.GameManager.Instance?.ResumeGame();
        }

        private void OnRestartClicked()
        {
            Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            Managers.GameManager.Instance?.RestartCurrentLevel();
        }

        private void OnSettingsClicked()
        {
            Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            if (settingsModal != null) settingsModal.SetActive(true);
        }

        private void OnMainMenuClicked()
        {
            Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            Managers.GameManager.Instance?.LoadMainMenu();
        }
    }
}