using UnityEngine;

namespace BatteryRun.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(BatterySystem))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Speeds")]
        public float walkSpeed = 6.0f;
        public float sprintSpeed = 9.5f;
        public float rotationSpeed = 12.0f;
        public float acceleration = 10.0f;

        [Header("Jumping & Gravity")]
        public float jumpForce = 8.0f;
        public float gravity = -22.0f;
        public float groundCheckDistance = 0.2f;

        [Header("Footstep Audio")]
        public float walkStepInterval = 0.45f;
        public float sprintStepInterval = 0.30f;

        // Components
        private CharacterController characterController;
        private BatterySystem batterySystem;
        private Transform cameraTransform;

        // Movement State
        private Vector3 currentVelocity;
        private Vector3 targetMoveVelocity;
        private float verticalVelocity;
        private bool isGrounded;
        private bool isSprinting;
        private bool isMoving;
        private float stepTimer;

        // Respawn & Invulnerability
        private Vector3 spawnPoint;
        private Quaternion spawnRotation;
        private bool isInvulnerable = false;
        private float invulnerableTimer = 0f;

        public bool IsMoving => isMoving;
        public bool IsSprinting => isSprinting;
        public bool IsGrounded => isGrounded;
        public float CurrentSpeed => new Vector3(characterController.velocity.x, 0, characterController.velocity.z).magnitude;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            batterySystem = GetComponent<BatterySystem>();
            spawnPoint = transform.position;
            spawnRotation = transform.rotation;
        }

        private void Start()
        {
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        private void Update()
        {
            if (batterySystem.IsDepleted || Time.timeScale <= 0f) return;

            // Invulnerability timer
            if (isInvulnerable)
            {
                invulnerableTimer -= Time.deltaTime;
                if (invulnerableTimer <= 0f)
                {
                    isInvulnerable = false;
                }
            }

            // Fell out of the world (no hazard volume under some levels)
            if (transform.position.y < -40f)
            {
                RespawnAtCheckpoint();
                return;
            }

            HandleGroundCheck();
            HandlePlatformRiding();
            HandleMovementInput();
            HandleJump();
            HandleAudioSteps();
        }

        // CharacterController is not carried by moving platforms automatically.
        private Transform ridingPlatform;
        private Vector3 lastPlatformPos;

        private void HandlePlatformRiding()
        {
            Transform found = null;
            if (Physics.Raycast(transform.position + Vector3.up * 0.3f, Vector3.down, out RaycastHit hit, 0.8f, ~0, QueryTriggerInteraction.Ignore))
            {
                var mp = hit.collider.GetComponentInParent<Environment.MovingPlatform>();
                if (mp != null) found = mp.transform;
            }

            if (found != null && found == ridingPlatform)
            {
                Vector3 delta = found.position - lastPlatformPos;
                if (delta.sqrMagnitude > 0f) characterController.Move(delta);
            }
            ridingPlatform = found;
            if (found != null) lastPlatformPos = found.position;
        }

        private void HandleGroundCheck()
        {
            isGrounded = characterController.isGrounded;
            if (isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2.0f; // slight grounding force
            }
        }

        private void HandleMovementInput()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;
            isMoving = inputDir.magnitude > 0.1f;
            isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            float speed = isSprinting ? sprintSpeed : walkSpeed;

            Vector3 moveDirection = Vector3.zero;

            if (isMoving)
            {
                if (cameraTransform == null && Camera.main != null)
                {
                    cameraTransform = Camera.main.transform;
                }

                if (cameraTransform != null)
                {
                    Vector3 camForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
                    Vector3 camRight = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1)).normalized;
                    moveDirection = (camForward * inputDir.z + camRight * inputDir.x).normalized;
                }
                else
                {
                    moveDirection = inputDir;
                }

                // Smooth rotation towards movement direction
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Smooth horizontal movement acceleration
            targetMoveVelocity = moveDirection * speed;
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetMoveVelocity, acceleration * Time.deltaTime * 10f);

            // Apply gravity
            verticalVelocity += gravity * Time.deltaTime;

            // Final velocity combination
            Vector3 finalMove = currentVelocity;
            finalMove.y = verticalVelocity;

            characterController.Move(finalMove * Time.deltaTime);
        }

        private void HandleJump()
        {
            if (isGrounded && (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space)))
            {
                verticalVelocity = jumpForce;
                Audio.SoundManager.Instance?.PlaySFX("Jump");
            }
        }

        private void HandleAudioSteps()
        {
            if (isGrounded && isMoving)
            {
                stepTimer += Time.deltaTime;
                float interval = isSprinting ? sprintStepInterval : walkStepInterval;
                if (stepTimer >= interval)
                {
                    stepTimer = 0f;
                    Audio.SoundManager.Instance?.PlaySFX("Walk");
                }
            }
            else
            {
                stepTimer = 0f;
            }
        }

        public void SetCheckpoint(Vector3 position, Quaternion rotation)
        {
            spawnPoint = position;
            spawnRotation = rotation;
        }

        public void TakeDamage(float batteryDrain, Vector3 knockbackDir)
        {
            if (isInvulnerable || batterySystem.IsDepleted) return;

            isInvulnerable = true;
            invulnerableTimer = 1.0f;

            batterySystem.ConsumeBattery(batteryDrain);
            Audio.SoundManager.Instance?.PlaySFX("ObstacleHit");

            // Apply brief knockback
            currentVelocity = knockbackDir * 8f;
            verticalVelocity = 4f;
        }

        public void RespawnAtCheckpoint()
        {
            characterController.enabled = false;
            transform.position = spawnPoint + Vector3.up * 0.5f;
            transform.rotation = spawnRotation;
            characterController.enabled = true;

            currentVelocity = Vector3.zero;
            verticalVelocity = 0f;
            ridingPlatform = null;

            batterySystem.RespawnSafetyBoost();
            Managers.LevelManager.Instance?.RecordPlayerDeath();
            Audio.SoundManager.Instance?.PlaySFX("ObstacleHit");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hazard") || other.CompareTag("Water"))
            {
                RespawnAtCheckpoint();
            }
        }
    }
}