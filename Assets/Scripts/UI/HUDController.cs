using UnityEngine;
using UnityEngine.UI;

namespace BatteryRun.UI
{
    public class HUDController : MonoBehaviour
    {
        [Header("Battery UI")]
        public Image batteryFillImage;
        public Text batteryPercentageText;
        public GameObject lowBatteryWarningBanner;

        [Header("Counters")]
        public Text batteryCountText;
        public Text crystalCountText;
        public Text scoreText;
        public Text timerText;
        public Text levelNameText;
        public Text objectiveText;
        public Text messageText;
        private float messageTimer = 0f;

        [Header("Controls")]
        public Button pauseButton;

        private Player.BatterySystem batterySystem;
        private float warningFlashTimer = 0f;

        private void Start()
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                batterySystem = player.GetComponent<Player.BatterySystem>();
                if (batterySystem != null)
                {
                    batterySystem.OnBatteryChanged += UpdateBatteryUI;
                    batterySystem.OnLowBatteryWarning += SetLowBatteryWarning;
                    UpdateBatteryUI(batterySystem.currentBattery, batterySystem.maxBattery);
                }
            }

            if (Managers.LevelManager.Instance != null)
            {
                Managers.LevelManager.Instance.OnBatteryCollected += UpdateBatteryCounter;
                Managers.LevelManager.Instance.OnCrystalCollected += UpdateCrystalCounter;
                Managers.LevelManager.Instance.OnMessage += ShowMessage;

                if (levelNameText != null)
                {
                    levelNameText.text = "LEVEL " + Managers.LevelManager.Instance.levelNumber + ": " + Managers.LevelManager.Instance.levelTitle.ToUpper();
                }

                if (objectiveText != null)
                {
                    objectiveText.text = Managers.LevelManager.Instance.levelObjective;
                }

                UpdateBatteryCounter(Managers.LevelManager.Instance.batteriesCollected, Managers.LevelManager.Instance.totalBatteriesInLevel);
                UpdateCrystalCounter(Managers.LevelManager.Instance.crystalsCollected, Managers.LevelManager.Instance.totalCrystalsInLevel);
            }

            if (Managers.ScoreManager.Instance != null)
            {
                Managers.ScoreManager.Instance.OnScoreChanged += UpdateScoreUI;
                UpdateScoreUI(Managers.ScoreManager.Instance.currentLevelScore);
            }

            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(() =>
                {
                    Managers.GameManager.Instance?.TogglePause();
                });
            }
        }

        private void Update()
        {
            if (batterySystem == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    batterySystem = player.GetComponent<Player.BatterySystem>();
                    if (batterySystem != null)
                    {
                        batterySystem.OnBatteryChanged += UpdateBatteryUI;
                        batterySystem.OnLowBatteryWarning += SetLowBatteryWarning;
                        UpdateBatteryUI(batterySystem.currentBattery, batterySystem.maxBattery);
                    }
                }
            }

            if (messageText != null && messageTimer > 0f)
            {
                messageTimer -= Time.unscaledDeltaTime;
                if (messageTimer <= 0f) messageText.text = "";
            }

            // Update timer
            if (Managers.LevelManager.Instance != null && timerText != null)
            {
                float t = Managers.LevelManager.Instance.levelTimer;
                int mins = (int)(t / 60);
                int secs = (int)(t % 60);
                timerText.text = string.Format("TIME: {0:00}:{1:00}", mins, secs);
            }

            // Flash warning banner
            if (lowBatteryWarningBanner != null && lowBatteryWarningBanner.activeSelf)
            {
                warningFlashTimer += Time.deltaTime * 5f;
                var canvasGroup = lowBatteryWarningBanner.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0.5f + Mathf.PingPong(warningFlashTimer, 0.5f);
                }
            }
        }

        private void ShowMessage(string msg)
        {
            if (messageText == null) return;
            messageText.text = msg;
            messageTimer = 2.5f;
        }

        private void OnDestroy()
        {
            if (Managers.ScoreManager.Instance != null) Managers.ScoreManager.Instance.OnScoreChanged -= UpdateScoreUI;
            if (Managers.LevelManager.Instance != null)
            {
                Managers.LevelManager.Instance.OnBatteryCollected -= UpdateBatteryCounter;
                Managers.LevelManager.Instance.OnCrystalCollected -= UpdateCrystalCounter;
                Managers.LevelManager.Instance.OnMessage -= ShowMessage;
            }
        }

        private void UpdateBatteryUI(float current, float max)
        {
            float pct = Mathf.Clamp01(current / max);
            if (batteryFillImage != null)
            {
                batteryFillImage.fillAmount = pct;
                if (batterySystem != null)
                {
                    batteryFillImage.color = batterySystem.GetCurrentBatteryColor();
                }
            }

            if (batteryPercentageText != null)
            {
                batteryPercentageText.text = string.Format("{0:0}%", pct * 100f);
            }
        }

        private void SetLowBatteryWarning(bool active)
        {
            if (lowBatteryWarningBanner != null)
            {
                lowBatteryWarningBanner.SetActive(active);
            }
        }

        private void UpdateBatteryCounter(int current, int total)
        {
            if (batteryCountText != null)
            {
                batteryCountText.text = string.Format("BATTERIES: {0} / {1}", current, total);
            }
        }

        private void UpdateCrystalCounter(int current, int total)
        {
            if (crystalCountText != null)
            {
                crystalCountText.text = string.Format("CRYSTALS: {0} / {1}", current, total);
            }
        }

        private void UpdateScoreUI(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = string.Format("SCORE: {0:N0}", score);
            }
        }
    }
}