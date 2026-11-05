using UnityEngine;

namespace BatteryRun.Collectibles
{
    [RequireComponent(typeof(Collider))]
    public abstract class CollectibleItem : MonoBehaviour
    {
        [Header("Animation Settings")]
        public float rotationSpeed = 90.0f;
        public float bobSpeed = 2.5f;
        public float bobHeight = 0.25f;

        [Header("Score & Value")]
        public int scoreValue = 100;
        public float batteryRestoreAmount = 20.0f;

        protected bool isCollected = false;
        private Vector3 startPos;

        protected virtual void Start()
        {
            startPos = transform.position;
            var col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }

        protected virtual void Update()
        {
            if (isCollected) return;

            // Idle rotation and bobbing
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
            float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isCollected) return;

            if (other.CompareTag("Player") || other.GetComponent<Player.PlayerController>() != null)
            {
                var playerBattery = other.GetComponent<Player.BatterySystem>();
                Collect(other.gameObject, playerBattery);
            }
        }

        public virtual void Collect(GameObject player, Player.BatterySystem batterySystem)
        {
            if (isCollected) return;
            isCollected = true;

            // Add score
            Managers.ScoreManager.Instance?.AddScore(scoreValue);

            // Restore battery
            if (batterySystem != null)
            {
                batterySystem.AddBattery(batteryRestoreAmount);
            }

            // Spawn collection visual burst
            SpawnPickupEffect();

            // Destroy object
            Destroy(gameObject, 0.05f);
        }

        protected void SpawnPickupEffect()
        {
            GameObject fx = new GameObject("PickupFX");
            fx.transform.position = transform.position;

            var ps = fx.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startSpeed = 3.5f;
            main.startSize = 0.15f;
            main.startLifetime = 0.6f;
            main.duration = 0.4f;
            main.loop = false;
            main.startColor = GetEffectColor();

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 25) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.3f;

            ps.Play();
            Destroy(fx, 1.0f);
        }

        protected abstract Color GetEffectColor();
    }
}