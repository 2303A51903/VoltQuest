using UnityEngine;

namespace BatteryRun.Enemies
{
    public class RollingHazard : MonoBehaviour
    {
        public Vector3 rollDirection = Vector3.forward;
        public float rollSpeed = 6.0f;
        public float maxTravelDistance = 25.0f;
        public float batteryDamage = 20.0f;

        private Vector3 startPos;
        private float distanceTraveled = 0f;

        private void Start()
        {
            startPos = transform.position;
            rollDirection = rollDirection.normalized;
        }

        private void Update()
        {
            if (Time.timeScale <= 0f) return;

            float moveStep = rollSpeed * Time.deltaTime;
            transform.position += rollDirection * moveStep;
            distanceTraveled += moveStep;

            // Rotate visually like a rolling boulder
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, rollDirection);
            transform.Rotate(rotationAxis, (rollSpeed * 40.0f) * Time.deltaTime, Space.World);

            if (distanceTraveled >= maxTravelDistance)
            {
                transform.position = startPos;
                distanceTraveled = 0f;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<Player.PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(batteryDamage, (other.transform.position - transform.position).normalized + Vector3.up * 0.5f);
                }
            }
        }
    }
}