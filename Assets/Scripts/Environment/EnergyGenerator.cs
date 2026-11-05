using UnityEngine;

namespace BatteryRun.Environment
{
    public class EnergyGenerator : MonoBehaviour
    {
        [Header("Generator Visual Elements")]
        public Transform rotatingCoreRing1;
        public Transform rotatingCoreRing2;
        public Light centralPowerBeam;
        public ParticleSystem energySparks;

        private bool isFullyActivated = false;

        private void Update()
        {
            if (Time.timeScale <= 0f) return;

            float speed = isFullyActivated ? 180f : 45f;
            if (rotatingCoreRing1 != null) rotatingCoreRing1.Rotate(Vector3.up, speed * Time.deltaTime, Space.Self);
            if (rotatingCoreRing2 != null) rotatingCoreRing2.Rotate(Vector3.right, (speed * 0.7f) * Time.deltaTime, Space.Self);
        }

        public void ActivateCore()
        {
            if (isFullyActivated) return;
            isFullyActivated = true;

            if (centralPowerBeam != null)
            {
                centralPowerBeam.enabled = true;
                centralPowerBeam.intensity = 8.0f;
            }

            if (energySparks != null)
            {
                energySparks.Play();
            }

            Audio.SoundManager.Instance?.PlaySFX("DoorUnlock");
        }
    }
}