using UnityEngine;
using UnityEngine.UI;

namespace BatteryRun.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Main Buttons")]
        public Button playButton;
        public Button levelSelectButton;
        public Button instructionsButton;
        public Button settingsButton;
        public Button quitButton;

        [Header("Modals")]
        public GameObject levelSelectModal;
        public GameObject instructionsModal;
        public GameObject settingsModal;

        [Header("Level Select Buttons")]
        public Button level1Button;
        public Button level2Button;
        public Button level3Button;
        public Button level4Button;
        public Text[] levelStarsText;

        private void Start()
        {
            Cameras.ThirdPersonCameraController.LockCursor(false);

            if (playButton != null) playButton.onClick.AddListener(OnPlayClicked);
            if (levelSelectButton != null) levelSelectButton.onClick.AddListener(OnLevelSelectClicked);
            if (instructionsButton != null) instructionsButton.onClick.AddListener(OnInstructionsClicked);
            if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsClicked);
            if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);

            SetupLevelSelectButtons();
        }

        private void SetupLevelSelectButtons()
        {
            int unlocked = SaveSystem.SaveManager.GetUnlockedLevel();

            if (level1Button != null)
            {
                level1Button.onClick.AddListener(() => LoadLevel(1));
                level1Button.interactable = true;
            }
            if (level2Button != null)
            {
                level2Button.onClick.AddListener(() => LoadLevel(2));
                level2Button.interactable = unlocked >= 2;
            }
            if (level3Button != null)
            {
                level3Button.onClick.AddListener(() => LoadLevel(3));
                level3Button.interactable = unlocked >= 3;
            }
            if (level4Button != null)
            {
                level4Button.onClick.AddListener(() => LoadLevel(4));
                level4Button.interactable = unlocked >= 4;
            }

            if (levelStarsText != null && levelStarsText.Length >= 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    int stars = SaveSystem.SaveManager.GetLevelStars(i + 1);
                    string starStr = "";
                    for (int s = 0; s < 3; s++) starStr += (s < stars) ? "★" : "☆";
                    levelStarsText[i].text = starStr;
                }
            }
        }

        private void OnPlayClicked()
        {
            Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            Managers.GameManager.Instance?.StartGame();
        }

        private void OnLevelSelectClicked()
        {
            Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            if (levelSelectModal != null) levelSelectModal.SetActive(true);
        }

        private void OnInstructionsClicked()
        {
            Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            if (instructionsModal != null) instructionsModal.SetActive(true);
        }

        private void OnSettingsClicked()
        {
            Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            if (settingsModal != null) settingsModal.SetActive(true);
        }

        private void OnQuitClicked()
        {
            Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            Managers.GameManager.Instance?.QuitGame();
        }

        private void LoadLevel(int level)
        {
            Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            Managers.GameManager.Instance?.LoadLevel(level);
        }

        public void CloseModals()
        {
            Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            if (levelSelectModal != null) levelSelectModal.SetActive(false);
            if (instructionsModal != null) instructionsModal.SetActive(false);
            if (settingsModal != null) settingsModal.SetActive(false);
        }
    }
}