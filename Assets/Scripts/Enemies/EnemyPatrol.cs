using UnityEngine;

namespace BatteryRun.Enemies
{
    public class EnemyPatrol : MonoBehaviour
    {
        [Header("Patrol Settings")]
        public Transform[] waypoints;
        public float patrolSpeed = 2.5f;
        public float chaseSpeed = 4.2f;
        public float waypointTolerance = 0.5f;

        [Header("Detection Settings")]
        public float detectionRadius = 6.0f;
        public float attackRange = 1.2f;
        public float batteryDamage = 15.0f;

        [Header("Visuals")]
        public Transform eyeLight;
        public Renderer bodyRenderer;

        private int currentWaypointIndex = 0;
        private Transform playerTarget;
        private bool isAlerted = false;
        private Vector3 startPosition;
        private Material eyeMat;

        private void Start()
        {
            startPosition = transform.position;

            var player = GameObject.FindWithTag("Player");
            if (player != null) playerTarget = player.transform;

            if (transform.childCount == 0)
            {
                BuildDroneVisual();
            }

            if (eyeLight != null)
            {
                var r = eyeLight.GetComponent<Renderer>();
                if (r != null) eyeMat = r.material;
            }
        }

        private void Update()
        {
            if (Time.timeScale <= 0f) return;

            if (playerTarget == null)
            {
                var p = GameObject.FindWithTag("Player");
                if (p != null) playerTarget = p.transform;
                return;
            }

            float distToPlayer = Vector3.Distance(transform.position, playerTarget.position);

            if (distToPlayer <= detectionRadius)
            {
                if (!isAlerted)
                {
                    isAlerted = true;
                    Audio.SoundManager.Instance?.PlaySFX("EnemyAlert");
                    SetEyeColor(Color.red);
                }
                ChasePlayer();
            }
            else
            {
                if (isAlerted)
                {
                    isAlerted = false;
                    SetEyeColor(new Color(0.9f, 0.6f, 0.1f)); // Orange-yellow
                }
                Patrol();
            }
        }

        private void Patrol()
        {
            if (waypoints == null || waypoints.Length == 0)
            {
                // Ping-pong along X axis
                float offset = Mathf.Sin(Time.time * patrolSpeed * 0.5f) * 4.0f;
                Vector3 target = startPosition + transform.right * offset;
                MoveTowards(target, patrolSpeed);
                return;
            }

            Transform targetWp = waypoints[currentWaypointIndex];
            if (targetWp == null) return;

            MoveTowards(targetWp.position, patrolSpeed);

            if (Vector3.Distance(transform.position, targetWp.position) <= waypointTolerance)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
        }

        private void ChasePlayer()
        {
            MoveTowards(playerTarget.position, chaseSpeed);

            if (Vector3.Distance(transform.position, playerTarget.position) <= attackRange)
            {
                var playerCtrl = playerTarget.GetComponent<Player.PlayerController>();
                if (playerCtrl != null)
                {
                    Vector3 knockback = (playerTarget.position - transform.position).normalized;
                    knockback.y = 0.5f;
                    playerCtrl.TakeDamage(batteryDamage, knockback);
                }
            }
        }

        private void MoveTowards(Vector3 target, float speed)
        {
            Vector3 dir = (target - transform.position);
            dir.y = 0;

            if (dir.magnitude > 0.1f)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 6.0f);
                transform.position += transform.forward * speed * Time.deltaTime;
            }
        }

        private void SetEyeColor(Color c)
        {
            if (eyeMat != null)
            {
                eyeMat.color = c;
                if (eyeMat.HasProperty("_EmissionColor"))
                {
                    eyeMat.EnableKeyword("_EMISSION");
                    eyeMat.SetColor("_EmissionColor", c * 2.0f);
                }
            }
        }

        private void BuildDroneVisual()
        {
            // Hover bot body
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            body.name = "DroneBody";
            body.transform.SetParent(transform);
            body.transform.localPosition = new Vector3(0, 0.6f, 0);
            body.transform.localScale = new Vector3(0.8f, 0.6f, 0.9f);
            DestroyImmediate(body.GetComponent<Collider>());

            Material m = Levels.MaterialUtil.NewMaterial();
            m.color = new Color(0.25f, 0.28f, 0.32f); // Dark gunmetal
            body.GetComponent<Renderer>().material = m;

            // Red scanner eye
            GameObject eye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            eye.name = "ScannerEye";
            eye.transform.SetParent(body.transform);
            eye.transform.localPosition = new Vector3(0, 0.1f, 0.45f);
            eye.transform.localScale = new Vector3(0.4f, 0.25f, 0.3f);
            DestroyImmediate(eye.GetComponent<Collider>());

            Material eyeM = Levels.MaterialUtil.NewMaterial();
            eyeM.color = new Color(0.9f, 0.6f, 0.1f);
            if (eyeM.HasProperty("_EmissionColor"))
            {
                eyeM.EnableKeyword("_EMISSION");
                eyeM.SetColor("_EmissionColor", new Color(0.9f, 0.6f, 0.1f) * 2.0f);
            }
            eye.GetComponent<Renderer>().material = eyeM;
            eyeLight = eye.transform;
            eyeMat = eyeM;
        }
    }
}