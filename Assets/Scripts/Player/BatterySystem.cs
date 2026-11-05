using System;
using UnityEngine;

namespace BatteryRun.Player
{
    public class BatterySystem : MonoBehaviour
    {
        [Header("Battery Settings")]
        [Range(0f, 100f)]
        public float maxBattery = 100f;
        public float currentBattery = 100f;

        [Tooltip("Battery drain per second during normal movement/idle")]
        public float drainRatePerSecond = 1.2f;

        [Tooltip("Additional battery drain multiplier when sprinting")]
        public float sprintDrainMultiplier = 1.8f;

        [Header("Warning Thresholds")]
        public float yellowThreshold = 60f;
        public float redThreshold = 30f;
        public float criticalWarningThreshold = 25f;

        public event Action<float, float> OnBatteryChanged;
        public event Action OnBatteryDepleted;
        public event Action<bool> OnLowBatteryWarning;

        private bool isDepleted = false;
        private bool isWarningActive = false;
        private float warningAudioCooldown = 0f;

        public bool IsDepleted => isDepleted;
        public float BatteryPercentage => Mathf.Clamp01(currentBattery / maxBattery) * 100f;

        private void Start()
        {
            currentBattery = maxBattery;
            OnBatteryChanged?.Invoke(currentBattery, maxBattery);
        }

        private void Update()
        {
            if (isDepleted || Time.timeScale <= 0f) return;

            // Check if player is sprinting
            var player = GetComponent<PlayerController>();
            bool isSprinting = player != null && player.IsSprinting && player.IsMoving;

            float drainRate = drainRatePerSecond * (isSprinting ? sprintDrainMultiplier : 1.0f);
            ConsumeBattery(drainRate * Time.deltaTime);

            // Handle warning audio timer
            if (currentBattery <= criticalWarningThreshold && !isDepleted)
            {
                if (!isWarningActive)
                {
                    isWarningActive = true;
                    OnLowBatteryWarning?.Invoke(true);
                }

                warningAudioCooldown -= Time.deltaTime;
                if (warningAudioCooldown <= 0f)
                {
                    warningAudioCooldown = 3.5f;
                    Audio.SoundManager.Instance?.PlaySFX("LowBattery");
                }
            }
            else if (isWarningActive && currentBattery > criticalWarningThreshold)
            {
                isWarningActive = false;
                OnLowBatteryWarning?.Invoke(false);
            }
        }

        public void ConsumeBattery(float amount)
        {
            if (isDepleted) return;

            currentBattery = Mathf.Max(0f, currentBattery - amount);
            OnBatteryChanged?.Invoke(currentBattery, maxBattery);

            if (currentBattery <= 0f && !isDepleted)
            {
                isDepleted = true;
                OnBatteryDepleted?.Invoke();
                Managers.GameManager.Instance?.TriggerGameOver("Battery Depleted! System Shut Down.");
            }
        }

        public void AddBattery(float amount)
        {
            if (isDepleted) return;

            currentBattery = Mathf.Min(maxBattery, currentBattery + amount);
            OnBatteryChanged?.Invoke(currentBattery, maxBattery);

            if (currentBattery > criticalWarningThreshold && isWarningActive)
            {
                isWarningActive = false;
                OnLowBatteryWarning?.Invoke(false);
            }
        }

        public void ReplenishFull()
        {
            isDepleted = false;
            currentBattery = maxBattery;
            isWarningActive = false;
            OnBatteryChanged?.Invoke(currentBattery, maxBattery);
            OnLowBatteryWarning?.Invoke(false);
        }

        public void RespawnSafetyBoost()
        {
            isDepleted = false;
            if (currentBattery < 35f)
            {
                currentBattery = 35f;
            }
            OnBatteryChanged?.Invoke(currentBattery, maxBattery);
        }

        public Color GetCurrentBatteryColor()
        {
            float percent = BatteryPercentage;
            if (percent >= yellowThreshold)
                return new Color(0.2f, 0.9f, 0.3f); // Vibrant Green
            if (percent >= redThreshold)
                return new Color(0.95f, 0.8f, 0.1f); // Vibrant Yellow
            return new Color(0.95f, 0.2f, 0.2f); // Vibrant Red
        }
    }
}