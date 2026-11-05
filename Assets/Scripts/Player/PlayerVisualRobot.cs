using UnityEngine;

namespace BatteryRun.Player
{
    public class PlayerVisualRobot : MonoBehaviour
    {
        [Header("References")]
        public Transform modelRoot;
        public Transform headBone;
        public Transform leftArm;
        public Transform rightArm;
        public Transform leftLeg;
        public Transform rightLeg;
        public Transform eyeLeft;
        public Transform eyeRight;
        public Transform antennaBall;
        public Renderer chestBatteryRenderer;
        public Renderer eyeRenderer;
        public ParticleSystem jetpackParticles;

        private PlayerController player;
        private BatterySystem batterySystem;

        private float animTime = 0f;
        private float blinkTimer = 3.0f;
        private bool isBlinking = false;
        private Material chestMaterial;
        private Material eyeMaterial;

        private Vector3 initialHeadPos;
        private Vector3 initialLeftArmPos;
        private Vector3 initialRightArmPos;

        private void Awake()
        {
            player = GetComponent<PlayerController>();
            batterySystem = GetComponent<BatterySystem>();

            if (modelRoot == null)
            {
                BuildProceduralRobot();
            }
        }

        private void Start()
        {
            if (headBone != null) initialHeadPos = headBone.localPosition;
            if (leftArm != null) initialLeftArmPos = leftArm.localPosition;
            if (rightArm != null) initialRightArmPos = rightArm.localPosition;

            if (chestBatteryRenderer != null)
            {
                chestMaterial = chestBatteryRenderer.material;
            }
            if (eyeRenderer != null)
            {
                eyeMaterial = eyeRenderer.material;
            }
        }

        private void Update()
        {
            AnimateRobot();
            UpdateBatteryVisuals();
            UpdateEyeBlink();
        }

        private void AnimateRobot()
        {
            if (player == null) return;

            bool isMoving = player.IsMoving;
            bool isSprinting = player.IsSprinting && isMoving;
            bool isGrounded = player.IsGrounded;

            float speedMultiplier = isSprinting ? 12.0f : 8.0f;
            if (!isMoving) speedMultiplier = 2.5f;

            animTime += Time.deltaTime * speedMultiplier;

            if (!isGrounded)
            {
                // In-air / jumping pose
                if (leftLeg != null) leftLeg.localRotation = Quaternion.Euler(-30f, 0, 0);
                if (rightLeg != null) rightLeg.localRotation = Quaternion.Euler(-30f, 0, 0);
                if (leftArm != null) leftArm.localRotation = Quaternion.Euler(45f, 0, -20f);
                if (rightArm != null) rightArm.localRotation = Quaternion.Euler(45f, 0, 20f);
                if (headBone != null) headBone.localRotation = Quaternion.Euler(-10f, 0, 0);

                if (jetpackParticles != null && !jetpackParticles.isPlaying)
                {
                    jetpackParticles.Play();
                }
            }
            else if (isMoving)
            {
                // Running / Walking cycle
                float swingAngle = isSprinting ? 45.0f : 30.0f;
                float legSwing = Mathf.Sin(animTime) * swingAngle;
                float armSwing = -Mathf.Sin(animTime) * swingAngle;

                if (leftLeg != null) leftLeg.localRotation = Quaternion.Euler(legSwing, 0, 0);
                if (rightLeg != null) rightLeg.localRotation = Quaternion.Euler(-legSwing, 0, 0);
                if (leftArm != null) leftArm.localRotation = Quaternion.Euler(armSwing, 0, -10f);
                if (rightArm != null) rightArm.localRotation = Quaternion.Euler(-armSwing, 0, 10f);

                // Body bounce & forward tilt
                float bounce = Mathf.Abs(Mathf.Sin(animTime * 2f)) * 0.08f;
                if (headBone != null)
                {
                    headBone.localPosition = initialHeadPos + new Vector3(0, bounce, 0);
                    headBone.localRotation = Quaternion.Euler(isSprinting ? 15f : 8f, 0, Mathf.Sin(animTime) * 3f);
                }

                if (jetpackParticles != null && jetpackParticles.isPlaying)
                {
                    jetpackParticles.Stop();
                }
            }
            else
            {
                // Idle breathing / gentle hover bobbing
                float idleBob = Mathf.Sin(animTime) * 0.04f;
                if (headBone != null)
                {
                    headBone.localPosition = initialHeadPos + new Vector3(0, idleBob, 0);
                    headBone.localRotation = Quaternion.Euler(Mathf.Sin(animTime * 0.5f) * 4f, 0, 0);
                }
                if (leftLeg != null) leftLeg.localRotation = Quaternion.identity;
                if (rightLeg != null) rightLeg.localRotation = Quaternion.identity;
                if (leftArm != null) leftArm.localRotation = Quaternion.Euler(Mathf.Sin(animTime) * 5f, 0, -5f);
                if (rightArm != null) rightArm.localRotation = Quaternion.Euler(-Mathf.Sin(animTime) * 5f, 0, 5f);

                if (jetpackParticles != null && jetpackParticles.isPlaying)
                {
                    jetpackParticles.Stop();
                }
            }
        }

        private void UpdateBatteryVisuals()
        {
            if (batterySystem == null || chestMaterial == null) return;

            Color batteryColor = batterySystem.GetCurrentBatteryColor();
            chestMaterial.color = batteryColor;
            chestMaterial.SetColor("_EmissionColor", batteryColor * 1.5f);

            if (antennaBall != null)
            {
                var r = antennaBall.GetComponent<Renderer>();
                if (r != null)
                {
                    r.material.color = batteryColor;
                    r.material.SetColor("_EmissionColor", batteryColor * 2.0f);
                }
            }
        }

        private void UpdateEyeBlink()
        {
            blinkTimer -= Time.deltaTime;
            if (blinkTimer <= 0f)
            {
                if (!isBlinking)
                {
                    isBlinking = true;
                    if (eyeLeft != null) eyeLeft.localScale = new Vector3(1f, 0.1f, 1f);
                    if (eyeRight != null) eyeRight.localScale = new Vector3(1f, 0.1f, 1f);
                    blinkTimer = 0.15f;
                }
                else
                {
                    isBlinking = false;
                    if (eyeLeft != null) eyeLeft.localScale = Vector3.one;
                    if (eyeRight != null) eyeRight.localScale = Vector3.one;
                    blinkTimer = Random.Range(2.5f, 5.0f);
                }
            }
        }

        public void BuildProceduralRobot()
        {
            if (modelRoot != null) return; // Awake already built it; avoids a duplicate robot
            GameObject root = new GameObject("BOLT_Robot_Visual");
            root.transform.SetParent(transform);
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;
            modelRoot = root.transform;

            // Load or find materials
            Material bodyMat = Resources.Load<Material>("Materials/Mat_RobotBody");
            if (bodyMat == null) bodyMat = CreateFallbackMaterial("Mat_RobotBody", new Color(0.9f, 0.95f, 1.0f), 0.7f);

            Material darkMat = CreateFallbackMaterial("Mat_RobotDark", new Color(0.18f, 0.22f, 0.28f), 0.5f);
            Material glowMat = CreateFallbackMaterial("Mat_RobotGlow", new Color(0.0f, 0.9f, 1.0f), 0.0f, true);
            Material batteryMat = CreateFallbackMaterial("Mat_BatteryGauge", new Color(0.2f, 0.9f, 0.3f), 0.0f, true);

            // 1. Torso
            GameObject torso = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            torso.name = "Torso";
            torso.transform.SetParent(root.transform);
            torso.transform.localPosition = new Vector3(0, 0.9f, 0);
            torso.transform.localScale = new Vector3(0.6f, 0.55f, 0.45f);
            DestroyImmediate(torso.GetComponent<Collider>());
            torso.GetComponent<Renderer>().material = bodyMat;

            // Chest Battery Gauge
            GameObject chestGauge = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chestGauge.name = "ChestBattery";
            chestGauge.transform.SetParent(torso.transform);
            chestGauge.transform.localPosition = new Vector3(0, 0.1f, 0.52f);
            chestGauge.transform.localScale = new Vector3(0.5f, 0.35f, 0.1f);
            DestroyImmediate(chestGauge.GetComponent<Collider>());
            chestBatteryRenderer = chestGauge.GetComponent<Renderer>();
            chestBatteryRenderer.material = batteryMat;

            // 2. Head
            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
            head.name = "Head";
            head.transform.SetParent(root.transform);
            head.transform.localPosition = new Vector3(0, 1.55f, 0);
            head.transform.localScale = new Vector3(0.55f, 0.45f, 0.5f);
            DestroyImmediate(head.GetComponent<Collider>());
            head.GetComponent<Renderer>().material = bodyMat;
            headBone = head.transform;

            // Visor / Face Screen
            GameObject visor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visor.name = "VisorScreen";
            visor.transform.SetParent(head.transform);
            visor.transform.localPosition = new Vector3(0, 0, 0.51f);
            visor.transform.localScale = new Vector3(0.85f, 0.7f, 0.05f);
            DestroyImmediate(visor.GetComponent<Collider>());
            visor.GetComponent<Renderer>().material = darkMat;

            // Cute Digital Eyes
            GameObject eyeL = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            eyeL.name = "EyeLeft";
            eyeL.transform.SetParent(visor.transform);
            eyeL.transform.localPosition = new Vector3(-0.25f, 0.05f, 0.55f);
            eyeL.transform.localScale = new Vector3(0.28f, 0.35f, 0.2f);
            DestroyImmediate(eyeL.GetComponent<Collider>());
            eyeRenderer = eyeL.GetComponent<Renderer>();
            eyeRenderer.material = glowMat;
            eyeLeft = eyeL.transform;

            GameObject eyeR = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            eyeR.name = "EyeRight";
            eyeR.transform.SetParent(visor.transform);
            eyeR.transform.localPosition = new Vector3(0.25f, 0.05f, 0.55f);
            eyeR.transform.localScale = new Vector3(0.28f, 0.35f, 0.2f);
            DestroyImmediate(eyeR.GetComponent<Collider>());
            eyeR.GetComponent<Renderer>().material = glowMat;
            eyeRight = eyeR.transform;

            // Antenna
            GameObject antenna = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            antenna.name = "AntennaStem";
            antenna.transform.SetParent(head.transform);
            antenna.transform.localPosition = new Vector3(0, 0.65f, 0);
            antenna.transform.localScale = new Vector3(0.04f, 0.2f, 0.04f);
            DestroyImmediate(antenna.GetComponent<Collider>());
            antenna.GetComponent<Renderer>().material = darkMat;

            GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.name = "AntennaOrb";
            ball.transform.SetParent(antenna.transform);
            ball.transform.localPosition = new Vector3(0, 1.1f, 0);
            ball.transform.localScale = new Vector3(4.0f, 0.8f, 4.0f);
            DestroyImmediate(ball.GetComponent<Collider>());
            ball.GetComponent<Renderer>().material = glowMat;
            antennaBall = ball.transform;

            // 3. Arms
            GameObject armL = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            armL.name = "ArmLeft";
            armL.transform.SetParent(root.transform);
            armL.transform.localPosition = new Vector3(-0.48f, 0.95f, 0);
            armL.transform.localScale = new Vector3(0.18f, 0.35f, 0.18f);
            DestroyImmediate(armL.GetComponent<Collider>());
            armL.GetComponent<Renderer>().material = bodyMat;
            leftArm = armL.transform;

            GameObject armR = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            armR.name = "ArmRight";
            armR.transform.SetParent(root.transform);
            armR.transform.localPosition = new Vector3(0.48f, 0.95f, 0);
            armR.transform.localScale = new Vector3(0.18f, 0.35f, 0.18f);
            DestroyImmediate(armR.GetComponent<Collider>());
            armR.GetComponent<Renderer>().material = bodyMat;
            rightArm = armR.transform;

            // 4. Legs / Feet
            GameObject legL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            legL.name = "LegLeft";
            legL.transform.SetParent(root.transform);
            legL.transform.localPosition = new Vector3(-0.2f, 0.35f, 0);
            legL.transform.localScale = new Vector3(0.18f, 0.28f, 0.18f);
            DestroyImmediate(legL.GetComponent<Collider>());
            legL.GetComponent<Renderer>().material = darkMat;
            leftLeg = legL.transform;

            GameObject legR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            legR.name = "LegRight";
            legR.transform.SetParent(root.transform);
            legR.transform.localPosition = new Vector3(0.2f, 0.35f, 0);
            legR.transform.localScale = new Vector3(0.18f, 0.28f, 0.18f);
            DestroyImmediate(legR.GetComponent<Collider>());
            legR.GetComponent<Renderer>().material = darkMat;
            rightLeg = legR.transform;

            // Cute Feet
            GameObject footL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            footL.name = "FootLeft";
            footL.transform.SetParent(legL.transform);
            footL.transform.localPosition = new Vector3(0, -1.0f, 0.3f);
            footL.transform.localScale = new Vector3(1.3f, 0.4f, 2.0f);
            DestroyImmediate(footL.GetComponent<Collider>());
            footL.GetComponent<Renderer>().material = bodyMat;

            GameObject footR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            footR.name = "FootRight";
            footR.transform.SetParent(legR.transform);
            footR.transform.localPosition = new Vector3(0, -1.0f, 0.3f);
            footR.transform.localScale = new Vector3(1.3f, 0.4f, 2.0f);
            DestroyImmediate(footR.GetComponent<Collider>());
            footR.GetComponent<Renderer>().material = bodyMat;

            // 5. Jetpack on Back
            GameObject jetpack = GameObject.CreatePrimitive(PrimitiveType.Cube);
            jetpack.name = "Jetpack";
            jetpack.transform.SetParent(torso.transform);
            jetpack.transform.localPosition = new Vector3(0, 0.1f, -0.55f);
            jetpack.transform.localScale = new Vector3(0.6f, 0.5f, 0.3f);
            DestroyImmediate(jetpack.GetComponent<Collider>());
            jetpack.GetComponent<Renderer>().material = darkMat;

            // Thruster nozzle
            GameObject nozzle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            nozzle.name = "ThrusterNozzle";
            nozzle.transform.SetParent(jetpack.transform);
            nozzle.transform.localPosition = new Vector3(0, -0.6f, 0);
            nozzle.transform.localScale = new Vector3(0.4f, 0.2f, 0.4f);
            DestroyImmediate(nozzle.GetComponent<Collider>());
            nozzle.GetComponent<Renderer>().material = glowMat;
        }

        private Material CreateFallbackMaterial(string name, Color color, float metallic = 0f, bool emissive = false)
        {
            Material mat = Levels.MaterialUtil.NewMaterial();
            mat.name = name;
            mat.color = color;
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0.5f);

            if (emissive && mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", color);
            }
            return mat;
        }
    }
}