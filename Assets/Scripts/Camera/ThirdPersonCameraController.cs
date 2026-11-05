using UnityEngine;

namespace BatteryRun.Cameras
{
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [Header("Target")]
        public Transform target;
        public Vector3 targetOffset = new Vector3(0, 1.4f, 0);

        [Header("Follow Settings")]
        public float distance = 5.0f;
        public float minDistance = 1.5f;
        public float maxDistance = 7.0f;
        public float smoothSpeed = 15.0f;

        [Header("Rotation Settings")]
        public float mouseSensitivityX = 3.0f;
        public float mouseSensitivityY = 2.0f;
        public float minPitch = -15.0f;
        public float maxPitch = 60.0f;

        [Header("Collision Avoidance")]
        public LayerMask collisionLayers = ~0; // Everything by default
        public float collisionRadius = 0.25f;

        private float currentYaw = 0.0f;
        private float currentPitch = 20.0f;
        private float currentDistance;

        private void Start()
        {
            currentDistance = distance;
            Vector3 angles = transform.eulerAngles;
            currentYaw = angles.y;
            currentPitch = angles.x;

            if (target == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
            }

            // Lock cursor during gameplay
            LockCursor(true);
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
                return;
            }

            if (Time.timeScale > 0f)
            {
                // Only rotate when cursor is locked
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    currentYaw += Input.GetAxis("Mouse X") * mouseSensitivityX;
                    currentPitch -= Input.GetAxis("Mouse Y") * mouseSensitivityY;
                    currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
                }
            }

            // Calculate rotation & target pivot
            Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
            Vector3 pivot = target.position + targetOffset;

            // Collision check
            Vector3 desiredCamPos = pivot - (rotation * Vector3.forward * distance);
            RaycastHit hit;
            float targetDist = distance;

            if (Physics.SphereCast(pivot, collisionRadius, (desiredCamPos - pivot).normalized, out hit, distance, collisionLayers, QueryTriggerInteraction.Ignore))
            {
                if (!hit.collider.CompareTag("Player") && !hit.collider.isTrigger)
                {
                    targetDist = Mathf.Clamp(hit.distance - 0.1f, minDistance, maxDistance);
                }
            }

            currentDistance = Mathf.Lerp(currentDistance, targetDist, Time.deltaTime * smoothSpeed);
            Vector3 finalPosition = pivot - (rotation * Vector3.forward * currentDistance);

            transform.position = finalPosition;
            transform.LookAt(pivot);
        }

        public static void LockCursor(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}