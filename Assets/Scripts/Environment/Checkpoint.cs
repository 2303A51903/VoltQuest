using UnityEngine;

namespace BatteryRun.Environment
{
    public class Checkpoint : MonoBehaviour
    {
        public int checkpointIndex = 1;
        public Light beaconLight;
        public Renderer beaconRenderer;

        private bool isActivated = false;

        private void OnTriggerEnter(Collider other)
        {
            if (isActivated) return;

            if (other.CompareTag("Player") || other.GetComponent<Player.PlayerController>() != null)
            {
                isActivated = true;
                Managers.CheckpointManager.Instance?.SetCheckpoint(transform.position, transform.rotation, checkpointIndex);

                if (beaconLight != null)
                {
                    beaconLight.color = new Color(0.2f, 1.0f, 0.4f);
                }

                if (beaconRenderer != null)
                {
                    beaconRenderer.material.color = new Color(0.2f, 1.0f, 0.4f);
                    beaconRenderer.material.SetColor("_EmissionColor", new Color(0.2f, 1.0f, 0.4f) * 2.0f);
                }

                Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            }
        }
    }
}