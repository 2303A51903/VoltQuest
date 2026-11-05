using UnityEngine;

namespace BatteryRun.Environment
{
    public class GoalBeacon : MonoBehaviour
    {
        public Light beamLight;
        public Renderer beamRenderer;

        private void Start()
        {
            if (Managers.LevelManager.Instance != null)
            {
                Managers.LevelManager.Instance.OnExitUnlocked += OnExitUnlockedHandler;
            }
        }

        private void OnDestroy()
        {
            if (Managers.LevelManager.Instance != null)
            {
                Managers.LevelManager.Instance.OnExitUnlocked -= OnExitUnlockedHandler;
            }
        }

        private void OnExitUnlockedHandler()
        {
            if (beamLight != null)
            {
                beamLight.color = new Color(0.2f, 1.0f, 0.4f);
                beamLight.intensity = 5.0f;
            }
            if (beamRenderer != null)
            {
                beamRenderer.material.color = new Color(0.2f, 1.0f, 0.4f);
                beamRenderer.material.SetColor("_EmissionColor", new Color(0.2f, 1.0f, 0.4f) * 3.0f);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            OnTriggerEnter(other);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.GetComponent<Player.PlayerController>() != null)
            {
                Managers.LevelManager.Instance?.ReachLevelGoal();
            }
        }
    }
}