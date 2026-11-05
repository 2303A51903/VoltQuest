using UnityEngine;

namespace BatteryRun.Environment
{
    public class FallingRock : MonoBehaviour
    {
        public float triggerDistance = 4.0f;
        public float shakeDuration = 0.5f;
        public float fallSpeed = 16.0f;
        public float batteryDamage = 25.0f;

        private Vector3 initialPos;
        private bool isTriggered = false;
        private bool isFalling = false;
        private float shakeTimer = 0f;
        private Transform player;

        private void Start()
        {
            initialPos = transform.position;
            var p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }

        private void Update()
        {
            if (Time.timeScale <= 0f) return;

            if (player == null)
            {
                var p = GameObject.FindWithTag("Player");
                if (p != null) player = p.transform;
                return;
            }

            if (!isTriggered)
            {
                Vector3 flatRock = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 flatPlayer = new Vector3(player.position.x, 0, player.position.z);

                if (Vector3.Distance(flatRock, flatPlayer) <= triggerDistance && player.position.y < transform.position.y)
                {
                    isTriggered = true;
                    shakeTimer = shakeDuration;
                }
            }
            else if (shakeTimer > 0f)
            {
                shakeTimer -= Time.deltaTime;
                // Wobble effect
                transform.position = initialPos + (Random.insideUnitSphere * 0.08f);

                if (shakeTimer <= 0f)
                {
                    isFalling = true;
                }
            }
            else if (isFalling)
            {
                transform.position += Vector3.down * fallSpeed * Time.deltaTime;

                // Stop at ground level
                if (transform.position.y <= 0.5f)
                {
                    isFalling = false;
                    Audio.SoundManager.Instance?.PlaySFX("ObstacleHit");
                    Destroy(gameObject, 1.5f);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isFalling && other.CompareTag("Player"))
            {
                var playerCtrl = other.GetComponent<Player.PlayerController>();
                if (playerCtrl != null)
                {
                    playerCtrl.TakeDamage(batteryDamage, Vector3.up);
                }
            }
        }
    }
}