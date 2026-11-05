using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BatteryRun.Managers
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        LevelComplete,
        GameOver,
        Victory
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("State")]
        public GameState currentState = GameState.Playing;
        public int currentLevelIndex = 1;
        public const int MaxLevels = 4;

        public event Action<GameState> OnGameStateChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private int lastDetectedSceneHandle = -1;
        public string lastGameOverReason = "Battery Depleted";

        private void OnDestroy()
        {
            if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // GameManager persists between scenes, so Start() only runs once. Without this hook the state
        // stayed 'MainMenu' after pressing Play (HUD hidden, pause broken, wrong music).
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            DetectCurrentSceneState();
        }

        private void Start()
        {
            if (lastDetectedSceneHandle != SceneManager.GetActiveScene().handle) DetectCurrentSceneState();
        }

        private void Update()
        {
            if (currentState == GameState.Playing || currentState == GameState.Paused)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
                {
                    TogglePause();
                }
            }
        }

        public void DetectCurrentSceneState()
        {
            lastDetectedSceneHandle = SceneManager.GetActiveScene().handle;
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName.Contains("MainMenu") || sceneName == "MainMenu")
            {
                SetState(GameState.MainMenu);
                Cameras.ThirdPersonCameraController.LockCursor(false);
                Time.timeScale = 1.0f;
                Audio.SoundManager.Instance?.PlayBGM("Music_MainMenu");
            }
            else
            {
                if (sceneName.Contains("1")) currentLevelIndex = 1;
                else if (sceneName.Contains("2")) currentLevelIndex = 2;
                else if (sceneName.Contains("3")) currentLevelIndex = 3;
                else if (sceneName.Contains("4")) currentLevelIndex = 4;

                SetState(GameState.Playing);
                Cameras.ThirdPersonCameraController.LockCursor(true);
                Time.timeScale = 1.0f;

                string bgm = "Music_Level" + currentLevelIndex;
                if (currentLevelIndex == 1) bgm = "Music_Level1_LostIsland";
                else if (currentLevelIndex == 2) bgm = "Music_Level2_DangerousJungle";
                else if (currentLevelIndex == 3) bgm = "Music_Level3_CrystalCave";
                else if (currentLevelIndex == 4) bgm = "Music_Level4_FinalCore";

                Audio.SoundManager.Instance?.PlayBGM(bgm);
            }
        }

        public void SetState(GameState newState)
        {
            currentState = newState;
            OnGameStateChanged?.Invoke(newState);
        }

        public void TogglePause()
        {
            if (currentState == GameState.Playing)
            {
                SetState(GameState.Paused);
                Time.timeScale = 0f;
                Cameras.ThirdPersonCameraController.LockCursor(false);
                Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            }
            else if (currentState == GameState.Paused)
            {
                SetState(GameState.Playing);
                Time.timeScale = 1f;
                Cameras.ThirdPersonCameraController.LockCursor(true);
                Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            }
        }

        public void ResumeGame()
        {
            if (currentState == GameState.Paused)
            {
                TogglePause();
            }
        }

        public void StartGame()
        {
            LoadLevel(1);
        }

        public void LoadLevel(int levelIndex)
        {
            currentLevelIndex = Mathf.Clamp(levelIndex, 1, MaxLevels);
            Time.timeScale = 1f;
            string targetScene = "Level" + currentLevelIndex;
            if (currentLevelIndex == 1) targetScene = "Level1_LostIsland";
            else if (currentLevelIndex == 2) targetScene = "Level2_DangerousJungle";
            else if (currentLevelIndex == 3) targetScene = "Level3_CrystalCave";
            else if (currentLevelIndex == 4) targetScene = "Level4_FinalCore";

            SceneManager.LoadScene(targetScene);
            Time.timeScale = 1.0f;
        }

        public void LoadNextLevel()
        {
            if (currentLevelIndex < MaxLevels)
            {
                LoadLevel(currentLevelIndex + 1);
            }
            else
            {
                TriggerVictory();
            }
        }

        public void RestartCurrentLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        public void TriggerLevelComplete()
        {
            if (currentState == GameState.LevelComplete) return;

            SetState(GameState.LevelComplete);
            Time.timeScale = 0f;
            Cameras.ThirdPersonCameraController.LockCursor(false);

            Audio.SoundManager.Instance?.PlaySFX("LevelComplete");
        }

        public void TriggerGameOver(string reason = "Battery Depleted")
        {
            if (currentState == GameState.GameOver) return;

            lastGameOverReason = reason;
            SetState(GameState.GameOver);
            Time.timeScale = 0f;
            Cameras.ThirdPersonCameraController.LockCursor(false);

            Audio.SoundManager.Instance?.PlayBGM("Music_GameOver", false);
            Audio.SoundManager.Instance?.PlaySFX("GameOver");
        }

        public void TriggerVictory()
        {
            SetState(GameState.Victory);
            Time.timeScale = 0f;
            Cameras.ThirdPersonCameraController.LockCursor(false);

            Audio.SoundManager.Instance?.PlayBGM("Music_Victory", false);
        }

        public void QuitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}