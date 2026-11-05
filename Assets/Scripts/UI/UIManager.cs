using UnityEngine;

namespace BatteryRun.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Screens")]
        public GameObject hudScreen;
        public GameObject pauseScreen;
        public GameObject levelCompleteScreen;
        public GameObject gameOverScreen;
        public GameObject victoryScreen;
        public GameObject settingsModal;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (Managers.GameManager.Instance != null)
            {
                Managers.GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
                HandleGameStateChanged(Managers.GameManager.Instance.currentState);
            }
        }

        private void OnDestroy()
        {
            if (Managers.GameManager.Instance != null)
            {
                Managers.GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
            }
        }

        public void HandleGameStateChanged(Managers.GameState state)
        {
            HideAllScreens();

            switch (state)
            {
                case Managers.GameState.Playing:
                    if (hudScreen != null) hudScreen.SetActive(true);
                    break;

                case Managers.GameState.Paused:
                    if (hudScreen != null) hudScreen.SetActive(true);
                    if (pauseScreen != null) pauseScreen.SetActive(true);
                    break;

                case Managers.GameState.LevelComplete:
                    if (levelCompleteScreen != null)
                    {
                        levelCompleteScreen.SetActive(true);
                        var comp = levelCompleteScreen.GetComponent<LevelCompleteUI>();
                        if (comp != null) comp.PopulateDetails();
                    }
                    break;

                case Managers.GameState.GameOver:
                    if (gameOverScreen != null)
                    {
                        gameOverScreen.SetActive(true);
                        var go = gameOverScreen.GetComponent<GameOverUI>();
                        if (go != null) go.PopulateDetails(Managers.GameManager.Instance != null ? Managers.GameManager.Instance.lastGameOverReason : "Battery Depleted");
                    }
                    break;

                case Managers.GameState.Victory:
                    if (victoryScreen != null)
                    {
                        victoryScreen.SetActive(true);
                        var vic = victoryScreen.GetComponent<VictoryUI>();
                        if (vic != null) vic.PopulateDetails();
                    }
                    break;
            }
        }

        private void HideAllScreens()
        {
            if (hudScreen != null) hudScreen.SetActive(false);
            if (pauseScreen != null) pauseScreen.SetActive(false);
            if (levelCompleteScreen != null) levelCompleteScreen.SetActive(false);
            if (gameOverScreen != null) gameOverScreen.SetActive(false);
            if (victoryScreen != null) victoryScreen.SetActive(false);
            if (settingsModal != null) settingsModal.SetActive(false);
        }
    }
}