using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BatteryRun.Levels
{
    [DefaultExecutionOrder(-100)]
    public class LevelBuilder : MonoBehaviour
    {
        public int forcedLevelIndex = 0; // 0 = auto-detect by scene name

        private Material sandMat;
        private Material grassMat;
        private Material jungleMat;
        private Material caveMat;
        private Material templeMat;
        private Material waterMat;
        private Material woodMat;
        private Material leavesMat;
        private Material crystalMat;
        private Material goldMat;

        private void Awake()
        {
            EnsureEventSystem();
            EnsureManagers();
            CreateMaterials();

            int level = forcedLevelIndex;
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (level == 0)
            {
                if (sceneName.Contains("MainMenu")) level = 0;
                else if (sceneName.Contains("1")) level = 1;
                else if (sceneName.Contains("2")) level = 2;
                else if (sceneName.Contains("3")) level = 3;
                else if (sceneName.Contains("4")) level = 4;
                else level = 1;
            }

            if (level == 0)
            {
                BuildMainMenuLevel();
            }
            else
            {
                BuildPlayableLevel(level);
            }
        }

        private void CreateMaterials()
        {
            sandMat = CreateColorMat("Mat_Sand", new Color(0.92f, 0.82f, 0.55f));
            grassMat = CreateColorMat("Mat_Grass", new Color(0.28f, 0.72f, 0.25f));
            jungleMat = CreateColorMat("Mat_JungleGrass", new Color(0.15f, 0.45f, 0.20f));
            caveMat = CreateColorMat("Mat_CaveRock", new Color(0.18f, 0.20f, 0.24f));
            templeMat = CreateColorMat("Mat_TempleTile", new Color(0.12f, 0.14f, 0.18f), 0.6f);
            waterMat = CreateColorMat("Mat_Water", new Color(0.1f, 0.65f, 0.85f, 0.75f), 0.8f);
            woodMat = CreateColorMat("Mat_Wood", new Color(0.48f, 0.32f, 0.18f));
            leavesMat = CreateColorMat("Mat_PalmLeaves", new Color(0.18f, 0.65f, 0.22f));
            crystalMat = CreateColorMat("Mat_Crystal", new Color(0.85f, 0.25f, 1.0f), 0.5f, true);
            goldMat = CreateColorMat("Mat_Gold", new Color(0.95f, 0.78f, 0.15f), 0.8f);
        }

        private Material CreateColorMat(string name, Color c, float metallic = 0f, bool emissive = false)
        {
            return MaterialUtil.Create(name, c, metallic, emissive);
        }

        private void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }
        }

        private void EnsureManagers()
        {
            if (Managers.GameManager.Instance == null)
            {
                GameObject gmObj = new GameObject("GameManager");
                gmObj.AddComponent<Managers.GameManager>();
            }

            if (Audio.SoundManager.Instance == null)
            {
                GameObject audioObj = new GameObject("SoundManager");
                audioObj.AddComponent<Audio.SoundManager>();
            }

            if (Managers.ScoreManager.Instance == null)
            {
                GameObject scoreObj = new GameObject("ScoreManager");
                scoreObj.AddComponent<Managers.ScoreManager>();
            }
        }

        #region Level 0: Main Menu
        private void BuildMainMenuLevel()
        {
            RenderSettings.ambientLight = new Color(0.8f, 0.85f, 0.95f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.65f, 0.85f, 0.95f);
            RenderSettings.fogDensity = 0.008f;

            // Directional Light
            GameObject dirLight = new GameObject("SunLight");
            var l = dirLight.AddComponent<Light>();
            l.type = LightType.Directional;
            l.color = new Color(1.0f, 0.95f, 0.85f);
            l.intensity = 1.3f;
            dirLight.transform.rotation = Quaternion.Euler(45f, -30f, 0);

            // Turntable Diorama Island
            GameObject diorama = new GameObject("MainMenuDiorama");
            diorama.transform.position = Vector3.zero;

            // Sand base
            GameObject island = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            island.name = "IslandBase";
            island.transform.SetParent(diorama.transform);
            island.transform.localPosition = new Vector3(0, -0.5f, 0);
            island.transform.localScale = new Vector3(18f, 1f, 18f);
            island.GetComponent<Renderer>().material = sandMat;

            // Grass Plateau
            GameObject plateau = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            plateau.name = "GrassPlateau";
            plateau.transform.SetParent(diorama.transform);
            plateau.transform.localPosition = new Vector3(0, 0.2f, 0);
            plateau.transform.localScale = new Vector3(12f, 0.6f, 12f);
            plateau.GetComponent<Renderer>().material = grassMat;

            // Water Ring
            GameObject ocean = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ocean.name = "Ocean";
            ocean.transform.SetParent(diorama.transform);
            ocean.transform.localPosition = new Vector3(0, -0.6f, 0);
            ocean.transform.localScale = new Vector3(35f, 0.4f, 35f);
            ocean.GetComponent<Renderer>().material = waterMat;

            // Palm trees
            CreatePalmTree(diorama.transform, new Vector3(-3.5f, 0.5f, -2.5f));
            CreatePalmTree(diorama.transform, new Vector3(4.0f, 0.5f, 1.5f));

            // Cute BOLT robot on island
            GameObject robot = new GameObject("BOLT_Showcase");
            robot.transform.SetParent(diorama.transform);
            robot.transform.localPosition = new Vector3(0, 0.5f, 0);
            robot.transform.localRotation = Quaternion.Euler(0, 160f, 0);
            var visual = robot.AddComponent<Player.PlayerVisualRobot>();
            visual.BuildProceduralRobot();

            // Rotating Showcase Camera
            GameObject camObj = new GameObject("ShowcaseCamera");
            camObj.tag = "MainCamera";
            var cam = camObj.AddComponent<UnityEngine.Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            camObj.AddComponent<AudioListener>(); // without this the main menu is silent
            camObj.transform.position = new Vector3(0, 4.5f, -9.5f);
            camObj.transform.LookAt(new Vector3(0, 1.2f, 0));

            // Slow rotator for diorama
            var rotator = diorama.AddComponent<Environment.SimpleRotator>();
            rotator.rotationSpeed = 8.0f;

            // Build Main Menu UI Canvas
            BuildMainMenuCanvas();
        }
        #endregion

        #region Playable Levels (1 to 4)
        private void BuildPlayableLevel(int level)
        {
            // Level Manager & Checkpoint Manager
            GameObject lmObj = new GameObject("LevelManager");
            var lm = lmObj.AddComponent<Managers.LevelManager>();
            lm.levelNumber = level;

            GameObject cpObj = new GameObject("CheckpointManager");
            cpObj.AddComponent<Managers.CheckpointManager>();

            // Setup Lighting & Environment styling
            GameObject dirLight = new GameObject("SunLight");
            var l = dirLight.AddComponent<Light>();
            l.type = LightType.Directional;
            dirLight.transform.rotation = Quaternion.Euler(50f, -40f, 0);

            if (level == 1) // Lost Island
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = new Color(0.68f, 0.88f, 0.98f);
                RenderSettings.fogDensity = 0.005f;
                l.color = new Color(1.0f, 0.96f, 0.88f);
                l.intensity = 1.35f;

                BuildLevel1Environment();
            }
            else if (level == 2) // Dangerous Jungle
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = new Color(0.2f, 0.45f, 0.25f);
                RenderSettings.fogDensity = 0.015f;
                l.color = new Color(0.85f, 0.95f, 0.75f);
                l.intensity = 1.1f;

                BuildLevel2Environment();
            }
            else if (level == 3) // Crystal Cave
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = new Color(0.12f, 0.08f, 0.22f);
                RenderSettings.fogDensity = 0.025f;
                l.color = new Color(0.45f, 0.4f, 0.75f);
                l.intensity = 0.6f;

                BuildLevel3Environment();
            }
            else if (level == 4) // Final Core Temple
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = new Color(0.18f, 0.15f, 0.28f);
                RenderSettings.fogDensity = 0.012f;
                l.color = new Color(0.95f, 0.85f, 0.7f);
                l.intensity = 1.25f;

                BuildLevel4Environment();
            }

            // Spawn Player & Camera
            GameObject player = SpawnPlayer(new Vector3(0, 1.0f, 0));
            SpawnCamera(player.transform);

            // Build HUD & In-Game Screens Canvas
            BuildInGameUICanvas();
        }

        private GameObject SpawnPlayer(Vector3 position)
        {
            GameObject player = new GameObject("Player_BOLT");
            player.tag = "Player";
            player.transform.position = position;

            var cc = player.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.radius = 0.45f;
            cc.center = new Vector3(0, 0.9f, 0);

            var bs = player.AddComponent<Player.BatterySystem>();
            var pc = player.AddComponent<Player.PlayerController>();
            var pvr = player.AddComponent<Player.PlayerVisualRobot>();
            pvr.BuildProceduralRobot();

            return player;
        }

        private void SpawnCamera(Transform playerTarget)
        {
            GameObject camObj = new GameObject("MainCamera");
            camObj.tag = "MainCamera";
            var cam = camObj.AddComponent<UnityEngine.Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            camObj.AddComponent<AudioListener>();

            var tpc = camObj.AddComponent<Cameras.ThirdPersonCameraController>();
            tpc.target = playerTarget;
        }

        private void BuildLevel1Environment()
        {
            GameObject env = new GameObject("Environment_Level1");

            // Island Main Ground
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "IslandGround";
            ground.transform.SetParent(env.transform);
            ground.transform.position = new Vector3(0, -0.5f, 25f);
            ground.transform.localScale = new Vector3(30f, 1f, 70f);
            ground.GetComponent<Renderer>().material = grassMat;

            // Sand Coastline
            GameObject sand = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sand.name = "SandCoast";
            sand.transform.SetParent(env.transform);
            sand.transform.position = new Vector3(0, -0.7f, 25f);
            sand.transform.localScale = new Vector3(38f, 0.8f, 78f);
            sand.GetComponent<Renderer>().material = sandMat;

            // Turquoise Ocean
            GameObject ocean = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ocean.name = "WaterZone";
            ocean.tag = "Water";
            ocean.transform.SetParent(env.transform);
            ocean.transform.position = new Vector3(0, -1.2f, 25f);
            ocean.transform.localScale = new Vector3(140f, 0.8f, 140f);
            ocean.GetComponent<Renderer>().material = waterMat;
            ocean.GetComponent<Collider>().isTrigger = true;

            // Palm Trees along the path
            CreatePalmTree(env.transform, new Vector3(-8f, 0f, 8f));
            CreatePalmTree(env.transform, new Vector3(9f, 0f, 15f));
            CreatePalmTree(env.transform, new Vector3(-7f, 0f, 28f));
            CreatePalmTree(env.transform, new Vector3(8f, 0f, 42f));
            CreatePalmTree(env.transform, new Vector3(-6f, 0f, 54f));

            // Stepping stones / bridge gap
            CreateSteppingStone(env.transform, new Vector3(0, 0, 22f), 3.5f);
            CreateSteppingStone(env.transform, new Vector3(0, 0.4f, 27f), 3.0f);
            CreateSteppingStone(env.transform, new Vector3(0, 0.8f, 32f), 3.5f);

            // 5 Normal Batteries
            SpawnBattery(new Vector3(0, 0.8f, 6f));
            SpawnBattery(new Vector3(-4f, 0.8f, 16f));
            SpawnBattery(new Vector3(0, 1.6f, 27f));
            SpawnBattery(new Vector3(5f, 0.8f, 38f));
            SpawnBattery(new Vector3(0, 0.8f, 50f));

            // 1 Hidden Rare Crystal
            SpawnCrystal(new Vector3(-11f, 1.2f, 32f)); // Hidden behind rocky palms

            // Checkpoint midway
            CreateCheckpointObject(env.transform, new Vector3(0, 0.5f, 20f), 1);

            // Goal Exit Beacon at end of island
            CreateGoalBeacon(env.transform, new Vector3(0, 0.5f, 58f));
        }

        private void BuildLevel2Environment()
        {
            GameObject env = new GameObject("Environment_Level2");

            // Jungle segments with ravines
            CreatePlatform(env.transform, new Vector3(0, -0.5f, 0), new Vector3(18f, 1f, 20f), jungleMat);
            CreatePlatform(env.transform, new Vector3(0, -0.5f, 30f), new Vector3(18f, 1f, 20f), jungleMat);
            CreatePlatform(env.transform, new Vector3(0, -0.5f, 60f), new Vector3(18f, 1f, 20f), jungleMat);
            CreatePlatform(env.transform, new Vector3(0, -0.5f, 85f), new Vector3(20f, 1f, 20f), jungleMat);

            // Hazard mud/water in ravines
            GameObject mud = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mud.tag = "Water";
            mud.transform.SetParent(env.transform);
            mud.transform.position = new Vector3(0, -5f, 42f);
            mud.transform.localScale = new Vector3(60f, 1f, 120f);
            mud.GetComponent<Renderer>().material = caveMat;
            mud.GetComponent<Collider>().isTrigger = true;

            // Moving Platforms crossing the ravines
            CreateMovingPlatform(env.transform, new Vector3(0, 0f, 14f), new Vector3(0, 0, 7f), 2.2f);
            CreateMovingPlatform(env.transform, new Vector3(0, 0f, 45f), new Vector3(6f, 0, 0), 2.5f);
            CreateMovingPlatform(env.transform, new Vector3(0, 0f, 72.5f), new Vector3(6f, 0, 0), 2.2f); // bridges the 5m gap before the portal

            // Rolling Hazard boulders
            CreateRollingHazard(env.transform, new Vector3(-3f, 1.2f, 38f), Vector3.back);

            // Enemy Patrol Drones
            CreateEnemy(env.transform, new Vector3(0, 0.5f, 28f));
            CreateEnemy(env.transform, new Vector3(0, 0.5f, 62f));

            // 8 Batteries
            SpawnBattery(new Vector3(0, 0.8f, 5f));
            SpawnBattery(new Vector3(-4f, 0.8f, 9f));
            SpawnBattery(new Vector3(0, 1.0f, 17f));
            SpawnBattery(new Vector3(4f, 0.8f, 25f));
            SpawnBattery(new Vector3(-4f, 0.8f, 33f));
            SpawnBattery(new Vector3(0, 1.0f, 47f));
            SpawnBattery(new Vector3(5f, 0.8f, 58f));
            SpawnBattery(new Vector3(-3f, 0.8f, 70f));

            // 2 Rare Crystals
            SpawnCrystal(new Vector3(7f, 1.2f, 30f));
            SpawnCrystal(new Vector3(-7f, 1.2f, 62f));

            // Checkpoints
            CreateCheckpointObject(env.transform, new Vector3(0, 0.5f, 22f), 1);
            CreateCheckpointObject(env.transform, new Vector3(0, 0.5f, 52f), 2);

            // Goal Beacon
            CreateGoalBeacon(env.transform, new Vector3(0, 0.5f, 92f));
        }

        private void BuildLevel3Environment()
        {
            GameObject env = new GameObject("Environment_Level3");

            // Cavern Floor
            CreatePlatform(env.transform, new Vector3(0, -0.5f, 45f), new Vector3(25f, 1f, 100f), caveMat);

            // Cave Walls & Arches
            CreateRockWall(env.transform, new Vector3(-13f, 4f, 45f), new Vector3(2f, 10f, 100f));
            CreateRockWall(env.transform, new Vector3(13f, 4f, 45f), new Vector3(2f, 10f, 100f));

            // Glowing Crystal Stalagmites
            CreateCrystalStalagmite(env.transform, new Vector3(-8f, 0f, 15f), new Color(0.3f, 0.6f, 1.0f));
            CreateCrystalStalagmite(env.transform, new Vector3(9f, 0f, 28f), new Color(0.9f, 0.3f, 1.0f));
            CreateCrystalStalagmite(env.transform, new Vector3(-9f, 0f, 52f), new Color(0.2f, 0.9f, 0.8f));

            // Falling Rocks
            CreateFallingRock(env.transform, new Vector3(0, 9f, 20f));
            CreateFallingRock(env.transform, new Vector3(2f, 9f, 35f));
            CreateFallingRock(env.transform, new Vector3(-2f, 9f, 65f));

            // Color-Matching Puzzle Setup
            CreateColorPuzzleArea(env.transform, new Vector3(0, 0, 48f));

            // 10 Batteries
            SpawnBattery(new Vector3(0, 0.8f, 8f));
            SpawnBattery(new Vector3(-4f, 0.8f, 14f));
            SpawnBattery(new Vector3(3f, 0.8f, 22f));
            SpawnBattery(new Vector3(-3f, 0.8f, 29f));
            SpawnBattery(new Vector3(0, 0.8f, 36f));
            SpawnBattery(new Vector3(4f, 0.8f, 42f));
            SpawnBattery(new Vector3(-4f, 0.8f, 58f));
            SpawnBattery(new Vector3(2f, 0.8f, 66f));
            SpawnBattery(new Vector3(-2f, 0.8f, 74f));
            SpawnBattery(new Vector3(0, 0.8f, 82f));

            // 3 Rare Crystals
            SpawnCrystal(new Vector3(-8f, 1.5f, 24f));
            SpawnCrystal(new Vector3(8f, 1.5f, 48f));
            SpawnCrystal(new Vector3(7f, 1.5f, 78f));

            // Checkpoints
            CreateCheckpointObject(env.transform, new Vector3(0, 0.5f, 32f), 1);
            CreateCheckpointObject(env.transform, new Vector3(0, 0.5f, 60f), 2);

            // Goal
            CreateGoalBeacon(env.transform, new Vector3(0, 0.5f, 90f));
        }

        private void BuildLevel4Environment()
        {
            GameObject env = new GameObject("Environment_Level4");

            // Ancient Energy Temple Platforms over Void
            CreatePlatform(env.transform, new Vector3(0, -0.5f, 0), new Vector3(16f, 1f, 16f), templeMat);
            CreatePlatform(env.transform, new Vector3(0, -0.5f, 28f), new Vector3(20f, 1f, 20f), templeMat);
            CreatePlatform(env.transform, new Vector3(-18f, 1f, 48f), new Vector3(14f, 1f, 14f), templeMat);
            CreatePlatform(env.transform, new Vector3(18f, 1f, 48f), new Vector3(14f, 1f, 14f), templeMat);
            CreatePlatform(env.transform, new Vector3(0, 2f, 75f), new Vector3(28f, 1f, 28f), templeMat);
            // Bridge pads between the side towers and the core platform (the moving bridge alone cannot be reached)
            CreatePlatform(env.transform, new Vector3(-12f, 2f, 58f), new Vector3(6f, 1f, 6f), templeMat);
            CreatePlatform(env.transform, new Vector3(12f, 2f, 58f), new Vector3(6f, 1f, 6f), templeMat);

            // Energy Void hazard below
            GameObject voidZone = GameObject.CreatePrimitive(PrimitiveType.Cube);
            voidZone.tag = "Hazard";
            voidZone.transform.SetParent(env.transform);
            voidZone.transform.position = new Vector3(0, -8f, 40f);
            voidZone.transform.localScale = new Vector3(100f, 1f, 120f);
            voidZone.GetComponent<Collider>().isTrigger = true;
            voidZone.GetComponent<Renderer>().material = caveMat;

            // Moving platforms between towers
            CreateMovingPlatform(env.transform, new Vector3(0, 0f, 12f), new Vector3(0, 0, 8f), 2.6f);
            CreateMovingPlatform(env.transform, new Vector3(-10f, 0.5f, 38f), new Vector3(0, 0, 6f), 2.4f);
            CreateMovingPlatform(env.transform, new Vector3(10f, 0.5f, 38f), new Vector3(0, 0, 6f), 2.4f);
            CreateMovingPlatform(env.transform, new Vector3(0, 1.5f, 56f), new Vector3(0, 0, 10f), 2.8f);

            // Central Grand Energy Core Generator
            CreateMasterGenerator(env.transform, new Vector3(0, 2.5f, 75f));

            // Enemy sentinels
            CreateEnemy(env.transform, new Vector3(0, 0.5f, 28f));
            CreateEnemy(env.transform, new Vector3(-18f, 2.0f, 48f));
            CreateEnemy(env.transform, new Vector3(18f, 2.0f, 48f));

            // 12 Batteries
            SpawnBattery(new Vector3(0, 0.8f, 4f));
            SpawnBattery(new Vector3(-4f, 0.8f, 8f));
            SpawnBattery(new Vector3(4f, 0.8f, 8f));
            SpawnBattery(new Vector3(0, 0.8f, 24f));
            SpawnBattery(new Vector3(-5f, 0.8f, 28f));
            SpawnBattery(new Vector3(5f, 0.8f, 28f));
            SpawnBattery(new Vector3(-18f, 2.2f, 45f));
            SpawnBattery(new Vector3(-18f, 2.2f, 51f));
            SpawnBattery(new Vector3(18f, 2.2f, 45f));
            SpawnBattery(new Vector3(18f, 2.2f, 51f));
            SpawnBattery(new Vector3(-6f, 3.2f, 72f));
            SpawnBattery(new Vector3(6f, 3.2f, 72f));

            // 4 Rare Crystals
            SpawnCrystal(new Vector3(0, 1.2f, 32f));
            SpawnCrystal(new Vector3(-21f, 2.5f, 48f));
            SpawnCrystal(new Vector3(21f, 2.5f, 48f));
            SpawnCrystal(new Vector3(0, 4.0f, 82f));

            // Checkpoints
            CreateCheckpointObject(env.transform, new Vector3(0, 0.5f, 22f), 1);
            CreateCheckpointObject(env.transform, new Vector3(0, 2.5f, 68f), 2);

            // Goal Escape Portal inside the Generator
            CreateGoalBeacon(env.transform, new Vector3(0, 3.0f, 75f));
        }
        #endregion

        #region Helper Constructors
        private void CreatePlatform(Transform parent, Vector3 pos, Vector3 size, Material mat)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = "Platform";
            obj.transform.SetParent(parent);
            obj.transform.position = pos;
            obj.transform.localScale = size;
            obj.GetComponent<Renderer>().material = mat;
        }

        private void CreatePalmTree(Transform parent, Vector3 pos)
        {
            GameObject tree = new GameObject("PalmTree");
            tree.transform.SetParent(parent);
            tree.transform.position = pos;

            // Trunk
            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Trunk";
            trunk.transform.SetParent(tree.transform);
            trunk.transform.localPosition = new Vector3(0, 2.5f, 0);
            trunk.transform.localScale = new Vector3(0.4f, 2.5f, 0.4f);
            trunk.GetComponent<Renderer>().material = woodMat;

            // Fronds / Crown
            GameObject crown = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            crown.name = "Crown";
            crown.transform.SetParent(tree.transform);
            crown.transform.localPosition = new Vector3(0, 5.0f, 0);
            crown.transform.localScale = new Vector3(4.5f, 0.8f, 4.5f);
            crown.GetComponent<Renderer>().material = leavesMat;
        }

        private void CreateSteppingStone(Transform parent, Vector3 pos, float scale)
        {
            GameObject stone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stone.name = "SteppingStone";
            stone.transform.SetParent(parent);
            stone.transform.position = pos;
            stone.transform.localScale = new Vector3(scale, 0.4f, scale);
            stone.GetComponent<Renderer>().material = caveMat;
            MaterialUtil.FixCylinderCollider(stone);
        }

        private void CreateMovingPlatform(Transform parent, Vector3 pos, Vector3 offset, float speed)
        {
            GameObject plat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plat.name = "MovingPlatform";
            plat.transform.SetParent(parent);
            plat.transform.position = pos;
            plat.transform.localScale = new Vector3(4.0f, 0.6f, 4.0f);
            plat.GetComponent<Renderer>().material = woodMat;

            var mp = plat.AddComponent<Environment.MovingPlatform>();
            mp.moveOffset = offset;
            mp.speed = speed;
        }

        private void CreateRollingHazard(Transform parent, Vector3 pos, Vector3 dir)
        {
            GameObject boulder = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            boulder.name = "RollingHazard";
            boulder.tag = "Enemy"; // damage handled by RollingHazard; "Hazard" means instant respawn
            boulder.transform.SetParent(parent);
            boulder.transform.position = pos;
            boulder.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
            boulder.GetComponent<Renderer>().material = caveMat;
            boulder.GetComponent<Collider>().isTrigger = true;

            var rh = boulder.AddComponent<Enemies.RollingHazard>();
            rh.rollDirection = dir;
            rh.rollSpeed = 7.0f;
        }

        private void CreateEnemy(Transform parent, Vector3 pos)
        {
            GameObject enemy = new GameObject("PatrolDrone");
            enemy.tag = "Enemy";
            enemy.transform.SetParent(parent);
            enemy.transform.position = pos;

            var col = enemy.AddComponent<SphereCollider>();
            col.radius = 0.8f;
            col.isTrigger = true;

            enemy.AddComponent<Enemies.EnemyPatrol>();
        }

        private void CreateRockWall(Transform parent, Vector3 pos, Vector3 size)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "RockWall";
            wall.transform.SetParent(parent);
            wall.transform.position = pos;
            wall.transform.localScale = size;
            wall.GetComponent<Renderer>().material = caveMat;
        }

        private void CreateCrystalStalagmite(Transform parent, Vector3 pos, Color c)
        {
            GameObject crystal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            crystal.name = "Stalagmite";
            crystal.transform.SetParent(parent);
            crystal.transform.position = pos + Vector3.up * 1.5f;
            crystal.transform.localScale = new Vector3(0.8f, 1.5f, 0.8f);

            Material mat = CreateColorMat("CrystalGlow", c, 0.4f, true);
            crystal.GetComponent<Renderer>().material = mat;
        }

        private void CreateFallingRock(Transform parent, Vector3 pos)
        {
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.name = "FallingRock";
            rock.transform.SetParent(parent);
            rock.transform.position = pos;
            rock.transform.localScale = new Vector3(1.5f, 2.2f, 1.5f);
            rock.GetComponent<Renderer>().material = caveMat;

            var col = rock.GetComponent<Collider>();
            col.isTrigger = true;

            rock.AddComponent<Environment.FallingRock>();
        }

        private void CreateColorPuzzleArea(Transform parent, Vector3 pos)
        {
            GameObject puzzle = new GameObject("ColorPuzzleArea");
            puzzle.transform.SetParent(parent);
            puzzle.transform.position = pos;

            var cp = puzzle.AddComponent<Environment.ColorPuzzle>();

            // Barrier Gate
            GameObject gate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gate.name = "CrystalGate";
            gate.transform.SetParent(puzzle.transform);
            gate.transform.localPosition = new Vector3(0, 2.5f, 10f);
            gate.transform.localScale = new Vector3(24f, 5f, 0.8f); // spans the whole cave so it cannot be walked around
            gate.GetComponent<Renderer>().material = crystalMat;
            cp.barrierGate = gate;

            // 3 Pedestals (0 = Red, 1 = Green, 2 = Blue)
            Color[] colors = { Color.red, Color.green, Color.blue };
            Transform[] peds = new Transform[3];
            Renderer[] runes = new Renderer[3];

            for (int i = 0; i < 3; i++)
            {
                float x = (i - 1) * 4.0f;
                GameObject ped = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                ped.name = "Pedestal_" + i;
                ped.transform.SetParent(puzzle.transform);
                ped.transform.localPosition = new Vector3(x, 0.2f, 0);
                ped.transform.localScale = new Vector3(2.0f, 0.2f, 2.0f);

                Material pMat = CreateColorMat("PedMat_" + i, colors[i] * 0.6f);
                ped.GetComponent<Renderer>().material = pMat;

                int colorIdx = i;
                MaterialUtil.FixCylinderCollider(ped, true);
                var trig = ped.AddComponent<PedestalTrigger>();
                trig.puzzle = cp;
                trig.colorIndex = colorIdx;

                peds[i] = ped.transform;

                // Rune light above gate
                GameObject rune = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                rune.name = "RuneGlyph_" + i;
                rune.transform.SetParent(gate.transform);
                rune.transform.localPosition = new Vector3((i - 1) * 0.12f, 0.35f, -0.7f);
                rune.transform.localScale = new Vector3(0.0375f, 0.18f, 1.1f);
                DestroyImmediate(rune.GetComponent<Collider>());

                Material rMat = CreateColorMat("RuneMat_" + i, Color.gray);
                rune.GetComponent<Renderer>().material = rMat;
                runes[i] = rune.GetComponent<Renderer>();
            }

            cp.pedestals = peds;
            cp.runeGlyphs = runes;
        }

        private void CreateMasterGenerator(Transform parent, Vector3 pos)
        {
            GameObject gen = new GameObject("AncientEnergyCore");
            gen.transform.SetParent(parent);
            gen.transform.position = pos;

            var eg = gen.AddComponent<Environment.EnergyGenerator>();

            // Base dais
            GameObject dais = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            dais.name = "GeneratorDais";
            dais.transform.SetParent(gen.transform);
            dais.transform.localPosition = new Vector3(0, -0.25f, 0); // top sits 0.25 above the platform (within step offset)
            dais.transform.localScale = new Vector3(10f, 0.5f, 10f);
            dais.GetComponent<Renderer>().material = goldMat;
            MaterialUtil.FixCylinderCollider(dais);

            // Rotating Core Ring 1
            GameObject ring1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring1.name = "CoreRing1";
            ring1.transform.SetParent(gen.transform);
            ring1.transform.localPosition = new Vector3(0, 3f, 0);
            ring1.transform.localScale = new Vector3(6f, 0.2f, 6f);
            DestroyImmediate(ring1.GetComponent<Collider>());
            ring1.GetComponent<Renderer>().material = crystalMat;
            eg.rotatingCoreRing1 = ring1.transform;

            // Rotating Core Ring 2
            GameObject ring2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring2.name = "CoreRing2";
            ring2.transform.SetParent(ring1.transform);
            ring2.transform.localPosition = Vector3.zero;
            ring2.transform.localRotation = Quaternion.Euler(45f, 0, 0);
            ring2.transform.localScale = new Vector3(0.7f, 1.2f, 0.7f);
            DestroyImmediate(ring2.GetComponent<Collider>());
            ring2.GetComponent<Renderer>().material = goldMat;
            eg.rotatingCoreRing2 = ring2.transform;

            // Central Power Beam Light
            GameObject beamLightObj = new GameObject("CentralPowerBeam");
            beamLightObj.transform.SetParent(gen.transform);
            beamLightObj.transform.localPosition = new Vector3(0, 2f, 0);
            var bLight = beamLightObj.AddComponent<Light>();
            bLight.type = LightType.Point;
            bLight.color = new Color(0.2f, 0.9f, 1.0f);
            bLight.range = 25f;
            bLight.intensity = 4.0f;
            eg.centralPowerBeam = bLight;
        }

        private void CreateCheckpointObject(Transform parent, Vector3 pos, int index)
        {
            GameObject cp = new GameObject("Checkpoint_" + index);
            cp.tag = "Checkpoint";
            cp.transform.SetParent(parent);
            cp.transform.position = pos;

            var col = cp.AddComponent<BoxCollider>();
            col.size = new Vector3(4f, 3f, 4f);
            col.isTrigger = true;

            var script = cp.AddComponent<Environment.Checkpoint>();
            script.checkpointIndex = index;

            // Visual Pillar
            GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pillar.transform.SetParent(cp.transform);
            pillar.transform.localPosition = new Vector3(0, 1.5f, 0);
            pillar.transform.localScale = new Vector3(0.4f, 1.5f, 0.4f);
            pillar.GetComponent<Renderer>().material = caveMat;

            // Beacon Orb
            GameObject orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            orb.transform.SetParent(cp.transform);
            orb.transform.localPosition = new Vector3(0, 3.2f, 0);
            orb.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            DestroyImmediate(orb.GetComponent<Collider>());

            Material orbMat = CreateColorMat("OrbMat", new Color(0.95f, 0.8f, 0.1f), 0.3f, true);
            var r = orb.GetComponent<Renderer>();
            r.material = orbMat;
            script.beaconRenderer = r;

            // Light
            GameObject lightObj = new GameObject("BeaconLight");
            lightObj.transform.SetParent(orb.transform);
            lightObj.transform.localPosition = Vector3.zero;
            var l = lightObj.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = new Color(0.95f, 0.8f, 0.1f);
            l.range = 8.0f;
            script.beaconLight = l;
        }

        private void CreateGoalBeacon(Transform parent, Vector3 pos)
        {
            GameObject goal = new GameObject("GoalExitBeacon");
            goal.tag = "Goal";
            goal.transform.SetParent(parent);
            goal.transform.position = pos;

            var col = goal.AddComponent<BoxCollider>();
            col.size = new Vector3(4f, 5f, 4f);
            col.isTrigger = true;

            var script = goal.AddComponent<Environment.GoalBeacon>();

            // Vertical Light Cylinder
            GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            beam.name = "BeaconColumn";
            beam.transform.SetParent(goal.transform);
            beam.transform.localPosition = new Vector3(0, 8f, 0);
            beam.transform.localScale = new Vector3(2.5f, 8f, 2.5f);
            DestroyImmediate(beam.GetComponent<Collider>());

            Material beamMat = CreateColorMat("GoalBeamMat", new Color(0.95f, 0.7f, 0.1f, 0.5f), 0.2f, true);
            beam.GetComponent<Renderer>().material = beamMat;
            script.beamRenderer = beam.GetComponent<Renderer>();

            // Light
            GameObject lObj = new GameObject("GoalLight");
            lObj.transform.SetParent(goal.transform);
            lObj.transform.localPosition = new Vector3(0, 2f, 0);
            var l = lObj.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = new Color(0.95f, 0.7f, 0.1f);
            l.range = 15f;
            l.intensity = 3.5f;
            script.beamLight = l;
        }

        private void SpawnBattery(Vector3 pos)
        {
            GameObject bat = new GameObject("BatteryPickup");
            bat.tag = "Battery";
            bat.transform.position = pos;

            var col = bat.AddComponent<SphereCollider>();
            col.radius = 0.6f;
            col.isTrigger = true;

            bat.AddComponent<Collectibles.BatteryPickup>();
        }

        private void SpawnCrystal(Vector3 pos)
        {
            GameObject cry = new GameObject("CrystalPickup");
            cry.tag = "Crystal";
            cry.transform.position = pos;

            var col = cry.AddComponent<SphereCollider>();
            col.radius = 0.8f;
            col.isTrigger = true;

            cry.AddComponent<Collectibles.CrystalPickup>();
        }
        #endregion

        #region UI Canvases Construction
        private void BuildMainMenuCanvas()
        {
            GameObject canvasObj = new GameObject("MainMenuCanvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();
            var menuUI = canvasObj.AddComponent<UI.MainMenuUI>();

            // Title Container
            GameObject titleObj = CreateText(canvasObj.transform, "BATTERY RUN", 76, new Vector2(0, 360), new Vector2(900, 100), TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            CreateText(titleObj.transform, "THE LOST ENERGY ISLAND", 32, new Vector2(0, -65), new Vector2(800, 50), TextAnchor.MiddleCenter, new Color(0.2f, 0.95f, 0.9f), FontStyle.Bold);

            // Main Buttons Panel
            GameObject panel = new GameObject("ButtonsPanel");
            panel.transform.SetParent(canvasObj.transform, false);
            var rt = panel.AddComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, -80);
            rt.sizeDelta = new Vector2(360, 420);

            var playBtn = CreateUIButton(panel.transform, "PLAY GAME", new Vector2(0, 150), new Vector2(340, 65), new Color(0.15f, 0.75f, 0.35f));
            var lvlBtn = CreateUIButton(panel.transform, "LEVEL SELECT", new Vector2(0, 75), new Vector2(340, 65), new Color(0.2f, 0.55f, 0.85f));
            var insBtn = CreateUIButton(panel.transform, "INSTRUCTIONS", new Vector2(0, 0), new Vector2(340, 65), new Color(0.45f, 0.35f, 0.75f));
            var setBtn = CreateUIButton(panel.transform, "SETTINGS", new Vector2(0, -75), new Vector2(340, 65), new Color(0.35f, 0.4f, 0.5f));
            var qutBtn = CreateUIButton(panel.transform, "QUIT", new Vector2(0, -150), new Vector2(340, 65), new Color(0.85f, 0.25f, 0.25f));

            menuUI.playButton = playBtn.GetComponent<Button>();
            menuUI.levelSelectButton = lvlBtn.GetComponent<Button>();
            menuUI.instructionsButton = insBtn.GetComponent<Button>();
            menuUI.settingsButton = setBtn.GetComponent<Button>();
            menuUI.quitButton = qutBtn.GetComponent<Button>();

            // Level Select Modal
            GameObject lsm = CreateModal(canvasObj.transform, "SELECT LEVEL", new Vector2(750, 500));
            menuUI.levelSelectModal = lsm;
            menuUI.levelStarsText = new Text[4];

            string[] lvlNames = { "1. The Lost Island", "2. Dangerous Jungle", "3. Crystal Cave", "4. The Final Core" };
            Button[] lvlBtns = new Button[4];
            for (int i = 0; i < 4; i++)
            {
                float y = 90 - (i * 75);
                var b = CreateUIButton(lsm.transform, lvlNames[i], new Vector2(-70, y), new Vector2(340, 55), new Color(0.2f, 0.5f, 0.8f));
                lvlBtns[i] = b.GetComponent<Button>();

                var starT = CreateText(lsm.transform, "☆☆☆", 28, new Vector2(180, y), new Vector2(150, 40), TextAnchor.MiddleCenter, new Color(1f, 0.85f, 0.2f));
                menuUI.levelStarsText[i] = starT.GetComponent<Text>();
            }
            menuUI.level1Button = lvlBtns[0];
            menuUI.level2Button = lvlBtns[1];
            menuUI.level3Button = lvlBtns[2];
            menuUI.level4Button = lvlBtns[3];

            var closeLs = CreateUIButton(lsm.transform, "CLOSE", new Vector2(0, -200), new Vector2(200, 45), new Color(0.5f, 0.5f, 0.5f));
            closeLs.GetComponent<Button>().onClick.AddListener(() => menuUI.CloseModals());
            lsm.SetActive(false);

            // Instructions Modal
            GameObject inm = CreateModal(canvasObj.transform, "HOW TO PLAY", new Vector2(800, 550));
            menuUI.instructionsModal = inm;
            string instText = "• W, A, S, D / Arrows: Move BOLT\n• Left Shift: Sprint (Consumes more battery!)\n• Spacebar: Jump\n• Mouse: Orbit Third-Person Camera\n• ESC / P: Pause Game\n\nOBJECTIVE:\nExplore the island, gather batteries (+20% power) and energy crystals (+35% power).\nAvoid traps, rolling boulders, and patrolling rogue drones.\nActivate checkpoints and unlock the final energy core to escape!";
            CreateText(inm.transform, instText, 24, new Vector2(0, 20), new Vector2(720, 320), TextAnchor.UpperLeft, Color.white);
            var closeIn = CreateUIButton(inm.transform, "GOT IT!", new Vector2(0, -220), new Vector2(220, 50), new Color(0.2f, 0.65f, 0.35f));
            closeIn.GetComponent<Button>().onClick.AddListener(() => menuUI.CloseModals());
            inm.SetActive(false);

            // Settings Modal
            GameObject stm = CreateSettingsModal(canvasObj.transform);
            menuUI.settingsModal = stm;
            stm.SetActive(false);
        }

        private void BuildInGameUICanvas()
        {
            GameObject canvasObj = new GameObject("InGameUICanvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();
            var uiMgr = canvasObj.AddComponent<UI.UIManager>();

            // 1. HUD Screen
            GameObject hud = new GameObject("HUDScreen");
            hud.transform.SetParent(canvasObj.transform, false);
            var hudRT = hud.AddComponent<RectTransform>();
            hudRT.anchorMin = Vector2.zero;
            hudRT.anchorMax = Vector2.one;
            hudRT.sizeDelta = Vector2.zero;
            uiMgr.hudScreen = hud;
            var hudCtrl = hud.AddComponent<UI.HUDController>();

            // Battery Panel Top-Left
            GameObject batPanel = new GameObject("BatteryPanel");
            batPanel.transform.SetParent(hud.transform, false);
            var bpRT = batPanel.AddComponent<RectTransform>();
            bpRT.anchorMin = new Vector2(0, 1);
            bpRT.anchorMax = new Vector2(0, 1);
            bpRT.anchoredPosition = new Vector2(240, -75);
            bpRT.sizeDelta = new Vector2(400, 90);

            CreateText(batPanel.transform, "ENERGY", 20, new Vector2(-130, 25), new Vector2(120, 30), TextAnchor.MiddleLeft, Color.white, FontStyle.Bold);

            // Battery Bar BG & Fill
            GameObject barBG = new GameObject("BatteryBarBG");
            barBG.transform.SetParent(batPanel.transform, false);
            var bgRT = barBG.AddComponent<RectTransform>();
            bgRT.anchoredPosition = new Vector2(20, 25);
            bgRT.sizeDelta = new Vector2(220, 28);
            var bgImg = barBG.AddComponent<Image>();
            bgImg.color = new Color(0.15f, 0.15f, 0.18f, 0.85f);

            GameObject barFill = new GameObject("BatteryBarFill");
            barFill.transform.SetParent(barBG.transform, false);
            var fillRT = barFill.AddComponent<RectTransform>();
            fillRT.anchorMin = Vector2.zero;
            fillRT.anchorMax = Vector2.one;
            fillRT.sizeDelta = Vector2.zero;
            var fillImg = barFill.AddComponent<Image>();
            fillImg.sprite = MaterialUtil.GetWhiteSprite(); // fillAmount is ignored without a sprite
            fillImg.type = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Horizontal;
            fillImg.fillAmount = 1.0f;
            fillImg.color = new Color(0.2f, 0.9f, 0.3f);
            hudCtrl.batteryFillImage = fillImg;

            var pctText = CreateText(barBG.transform, "100%", 18, Vector2.zero, new Vector2(100, 25), TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            hudCtrl.batteryPercentageText = pctText.GetComponent<Text>();

            // Counters Under Battery Bar
            var batCnt = CreateText(batPanel.transform, "BATTERIES: 0 / 5", 20, new Vector2(-50, -18), new Vector2(200, 30), TextAnchor.MiddleLeft, new Color(1f, 0.85f, 0.2f), FontStyle.Bold);
            hudCtrl.batteryCountText = batCnt.GetComponent<Text>();

            var cryCnt = CreateText(batPanel.transform, "CRYSTALS: 0 / 1", 20, new Vector2(110, -18), new Vector2(200, 30), TextAnchor.MiddleLeft, new Color(0.85f, 0.4f, 1f), FontStyle.Bold);
            hudCtrl.crystalCountText = cryCnt.GetComponent<Text>();

            // Top Center: Level Name & Mission Objective
            GameObject missionPanel = new GameObject("MissionPanel");
            missionPanel.transform.SetParent(hud.transform, false);
            var mpRT = missionPanel.AddComponent<RectTransform>();
            mpRT.anchorMin = new Vector2(0.5f, 1);
            mpRT.anchorMax = new Vector2(0.5f, 1);
            mpRT.anchoredPosition = new Vector2(0, -60);
            mpRT.sizeDelta = new Vector2(700, 80);

            var lvlTitle = CreateText(missionPanel.transform, "LEVEL 1: THE LOST ISLAND", 28, new Vector2(0, 15), new Vector2(700, 40), TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            hudCtrl.levelNameText = lvlTitle.GetComponent<Text>();

            var objText = CreateText(missionPanel.transform, "Collect all 5 batteries & reach the beacon!", 19, new Vector2(0, -18), new Vector2(700, 30), TextAnchor.MiddleCenter, new Color(0.8f, 0.9f, 1.0f));
            hudCtrl.objectiveText = objText.GetComponent<Text>();

            var msgT = CreateText(hud.transform, "", 26, new Vector2(0, 140), new Vector2(900, 50), TextAnchor.MiddleCenter, new Color(1f, 0.9f, 0.4f), FontStyle.Bold);
            hudCtrl.messageText = msgT.GetComponent<Text>();

            // Top Right: Score & Timer & Pause Button
            GameObject rightPanel = new GameObject("RightHUDPanel");
            rightPanel.transform.SetParent(hud.transform, false);
            var rpRT = rightPanel.AddComponent<RectTransform>();
            rpRT.anchorMin = new Vector2(1, 1);
            rpRT.anchorMax = new Vector2(1, 1);
            rpRT.anchoredPosition = new Vector2(-180, -65);
            rpRT.sizeDelta = new Vector2(300, 80);

            var scoreT = CreateText(rightPanel.transform, "SCORE: 0", 24, new Vector2(-70, 15), new Vector2(200, 30), TextAnchor.MiddleRight, Color.white, FontStyle.Bold);
            hudCtrl.scoreText = scoreT.GetComponent<Text>();

            var timerT = CreateText(rightPanel.transform, "TIME: 00:00", 20, new Vector2(-70, -18), new Vector2(200, 30), TextAnchor.MiddleRight, new Color(0.8f, 0.85f, 0.9f));
            hudCtrl.timerText = timerT.GetComponent<Text>();

            var pauseBtn = CreateUIButton(rightPanel.transform, "||", new Vector2(85, 0), new Vector2(50, 50), new Color(0.25f, 0.3f, 0.4f));
            hudCtrl.pauseButton = pauseBtn.GetComponent<Button>();

            // Low Battery Warning Banner
            GameObject warnBanner = new GameObject("LowBatteryWarningBanner");
            warnBanner.transform.SetParent(hud.transform, false);
            var wbRT = warnBanner.AddComponent<RectTransform>();
            wbRT.anchorMin = new Vector2(0.5f, 0.5f);
            wbRT.anchorMax = new Vector2(0.5f, 0.5f);
            wbRT.anchoredPosition = new Vector2(0, 220);
            wbRT.sizeDelta = new Vector2(550, 60);

            var wbImg = warnBanner.AddComponent<Image>();
            wbImg.color = new Color(0.8f, 0.1f, 0.1f, 0.85f);
            warnBanner.AddComponent<CanvasGroup>();

            CreateText(warnBanner.transform, "⚠ LOW BATTERY WARNING! RECHARGE IMMINENT ⚠", 22, Vector2.zero, new Vector2(550, 60), TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            hudCtrl.lowBatteryWarningBanner = warnBanner;
            warnBanner.SetActive(false);

            // 2. Pause Menu Screen
            GameObject pauseObj = CreateModal(canvasObj.transform, "GAME PAUSED", new Vector2(500, 480));
            uiMgr.pauseScreen = pauseObj;
            var pauseUI = pauseObj.AddComponent<UI.PauseMenuUI>();

            var resBtn = CreateUIButton(pauseObj.transform, "RESUME", new Vector2(0, 90), new Vector2(300, 55), new Color(0.2f, 0.7f, 0.35f));
            var rstBtn = CreateUIButton(pauseObj.transform, "RESTART LEVEL", new Vector2(0, 20), new Vector2(300, 55), new Color(0.8f, 0.45f, 0.15f));
            var setPBtn = CreateUIButton(pauseObj.transform, "SETTINGS", new Vector2(0, -50), new Vector2(300, 55), new Color(0.35f, 0.45f, 0.6f));
            var menBtn = CreateUIButton(pauseObj.transform, "MAIN MENU", new Vector2(0, -120), new Vector2(300, 55), new Color(0.7f, 0.25f, 0.25f));

            pauseUI.resumeButton = resBtn.GetComponent<Button>();
            pauseUI.restartButton = rstBtn.GetComponent<Button>();
            pauseUI.settingsButton = setPBtn.GetComponent<Button>();
            pauseUI.mainMenuButton = menBtn.GetComponent<Button>();
            pauseObj.SetActive(false);

            // 3. Level Complete Screen
            GameObject lcObj = CreateModal(canvasObj.transform, "LEVEL COMPLETE!", new Vector2(650, 580));
            uiMgr.levelCompleteScreen = lcObj;
            var lcUI = lcObj.AddComponent<UI.LevelCompleteUI>();

            var starsT = CreateText(lcObj.transform, "★★★", 48, new Vector2(0, 150), new Vector2(400, 60), TextAnchor.MiddleCenter, new Color(1f, 0.85f, 0.15f), FontStyle.Bold);
            var scT = CreateText(lcObj.transform, "LEVEL SCORE: 0", 28, new Vector2(0, 95), new Vector2(500, 40), TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            var bT = CreateText(lcObj.transform, "BATTERIES: 0 / 0", 22, new Vector2(0, 50), new Vector2(500, 35), TextAnchor.MiddleCenter, new Color(0.9f, 0.9f, 0.9f));
            var cT = CreateText(lcObj.transform, "CRYSTALS: 0 / 0", 22, new Vector2(0, 15), new Vector2(500, 35), TextAnchor.MiddleCenter, new Color(0.9f, 0.9f, 0.9f));
            var tT = CreateText(lcObj.transform, "TIME TAKEN: 00:00", 22, new Vector2(0, -20), new Vector2(500, 35), TextAnchor.MiddleCenter, new Color(0.9f, 0.9f, 0.9f));

            var nxtBtn = CreateUIButton(lcObj.transform, "NEXT LEVEL", new Vector2(0, -90), new Vector2(280, 50), new Color(0.2f, 0.75f, 0.35f));
            var repBtn = CreateUIButton(lcObj.transform, "REPLAY LEVEL", new Vector2(0, -150), new Vector2(280, 50), new Color(0.25f, 0.5f, 0.8f));
            var mmBtn = CreateUIButton(lcObj.transform, "MAIN MENU", new Vector2(0, -210), new Vector2(280, 50), new Color(0.5f, 0.5f, 0.5f));

            lcUI.starsText = starsT.GetComponent<Text>();
            lcUI.finalScoreText = scT.GetComponent<Text>();
            lcUI.batteriesText = bT.GetComponent<Text>();
            lcUI.crystalsText = cT.GetComponent<Text>();
            lcUI.timeText = tT.GetComponent<Text>();
            lcUI.nextLevelButton = nxtBtn.GetComponent<Button>();
            lcUI.replayButton = repBtn.GetComponent<Button>();
            lcUI.mainMenuButton = mmBtn.GetComponent<Button>();
            lcObj.SetActive(false);

            // 4. Game Over Screen
            GameObject goObj = CreateModal(canvasObj.transform, "SYSTEM SHUTDOWN", new Vector2(550, 480));
            uiMgr.gameOverScreen = goObj;
            var goUI = goObj.AddComponent<UI.GameOverUI>();

            var rText = CreateText(goObj.transform, "BATTERY DEPLETED", 26, new Vector2(0, 100), new Vector2(500, 40), TextAnchor.MiddleCenter, new Color(1f, 0.3f, 0.3f), FontStyle.Bold);
            var fsText = CreateText(goObj.transform, "FINAL SCORE: 0", 24, new Vector2(0, 40), new Vector2(500, 40), TextAnchor.MiddleCenter, Color.white);

            var rtrBtn = CreateUIButton(goObj.transform, "RETRY", new Vector2(0, -40), new Vector2(280, 55), new Color(0.8f, 0.3f, 0.2f));
            var goMMBtn = CreateUIButton(goObj.transform, "MAIN MENU", new Vector2(0, -110), new Vector2(280, 55), new Color(0.4f, 0.45f, 0.5f));

            goUI.reasonText = rText.GetComponent<Text>();
            goUI.scoreText = fsText.GetComponent<Text>();
            goUI.retryButton = rtrBtn.GetComponent<Button>();
            goUI.mainMenuButton = goMMBtn.GetComponent<Button>();
            goObj.SetActive(false);

            // 5. Victory Screen
            GameObject vicObj = CreateModal(canvasObj.transform, "VICTORY!", new Vector2(700, 560));
            uiMgr.victoryScreen = vicObj;
            var vicUI = vicObj.AddComponent<UI.VictoryUI>();

            var congText = CreateText(vicObj.transform, "ISLAND ENERGY RESTORED!\nBOLT SUCCESSFULLY ESCAPED!", 26, new Vector2(0, 120), new Vector2(650, 70), TextAnchor.MiddleCenter, new Color(0.2f, 1f, 0.7f), FontStyle.Bold);
            var totScText = CreateText(vicObj.transform, "CUMULATIVE SCORE: 0", 28, new Vector2(0, 40), new Vector2(550, 40), TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            var totStText = CreateText(vicObj.transform, "TOTAL STARS EARNED: 12 / 12", 24, new Vector2(0, -10), new Vector2(550, 40), TextAnchor.MiddleCenter, new Color(1f, 0.85f, 0.2f), FontStyle.Bold);

            var playAgainBtn = CreateUIButton(vicObj.transform, "PLAY AGAIN", new Vector2(0, -90), new Vector2(300, 55), new Color(0.15f, 0.75f, 0.35f));
            var vicMMBtn = CreateUIButton(vicObj.transform, "MAIN MENU", new Vector2(0, -160), new Vector2(300, 55), new Color(0.45f, 0.5f, 0.6f));

            vicUI.congratulationsText = congText.GetComponent<Text>();
            vicUI.totalScoreText = totScText.GetComponent<Text>();
            vicUI.totalStarsText = totStText.GetComponent<Text>();
            vicUI.playAgainButton = playAgainBtn.GetComponent<Button>();
            vicUI.mainMenuButton = vicMMBtn.GetComponent<Button>();
            vicObj.SetActive(false);

            // 6. Settings Modal
            GameObject stm = CreateSettingsModal(canvasObj.transform);
            uiMgr.settingsModal = stm;
            pauseUI.settingsModal = stm;
            stm.SetActive(false);
        }

        private GameObject CreateModal(Transform parent, string title, Vector2 size)
        {
            GameObject modal = new GameObject("Modal_" + title);
            modal.transform.SetParent(parent, false);
            var rt = modal.AddComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = size;

            var img = modal.AddComponent<Image>();
            img.color = new Color(0.1f, 0.12f, 0.16f, 0.95f);

            CreateText(modal.transform, title, 32, new Vector2(0, (size.y * 0.5f) - 45), new Vector2(size.x - 40, 50), TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);

            return modal;
        }

        private GameObject CreateSettingsModal(Transform parent)
        {
            GameObject sm = CreateModal(parent, "SETTINGS", new Vector2(550, 480));
            var settingsUI = sm.AddComponent<UI.SettingsMenuUI>();

            // Sliders: Master, Music, SFX
            settingsUI.masterSlider = CreateSlider(sm.transform, "Master Volume", new Vector2(0, 80));
            settingsUI.musicSlider = CreateSlider(sm.transform, "Music Volume", new Vector2(0, 10));
            settingsUI.sfxSlider = CreateSlider(sm.transform, "SFX Volume", new Vector2(0, -60));

            settingsUI.muteToggle = CreateToggle(sm.transform, "Mute All", new Vector2(0, -120));

            // Close button
            var close = CreateUIButton(sm.transform, "CLOSE", new Vector2(0, -180), new Vector2(180, 45), new Color(0.5f, 0.5f, 0.5f));
            settingsUI.closeButton = close.GetComponent<Button>();

            return sm;
        }

        private Toggle CreateToggle(Transform parent, string label, Vector2 pos)
        {
            GameObject row = new GameObject("ToggleRow_" + label);
            row.transform.SetParent(parent, false);
            var rt = row.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(450, 40);

            GameObject box = new GameObject("Box");
            box.transform.SetParent(row.transform, false);
            var brt = box.AddComponent<RectTransform>();
            brt.anchoredPosition = new Vector2(-120, 0);
            brt.sizeDelta = new Vector2(30, 30);
            var bimg = box.AddComponent<Image>();
            bimg.color = new Color(0.2f, 0.22f, 0.28f);

            GameObject check = new GameObject("Checkmark");
            check.transform.SetParent(box.transform, false);
            var crt = check.AddComponent<RectTransform>();
            crt.anchorMin = Vector2.zero;
            crt.anchorMax = Vector2.one;
            crt.sizeDelta = new Vector2(-8, -8);
            var cimg = check.AddComponent<Image>();
            cimg.color = new Color(0.2f, 0.9f, 0.4f);

            CreateText(row.transform, label, 20, new Vector2(20, 0), new Vector2(200, 40), TextAnchor.MiddleLeft, Color.white);

            var toggle = row.AddComponent<Toggle>();
            toggle.targetGraphic = bimg;
            toggle.graphic = cimg;
            toggle.isOn = false;
            return toggle;
        }

        private Slider CreateSlider(Transform parent, string label, Vector2 pos)
        {
            GameObject row = new GameObject("SliderRow_" + label);
            row.transform.SetParent(parent, false);
            var rt = row.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(450, 50);

            CreateText(row.transform, label, 20, new Vector2(-120, 0), new Vector2(180, 40), TextAnchor.MiddleLeft, Color.white);

            GameObject sliderObj = new GameObject("Slider");
            sliderObj.transform.SetParent(row.transform, false);
            var sRT = sliderObj.AddComponent<RectTransform>();
            sRT.anchoredPosition = new Vector2(80, 0);
            sRT.sizeDelta = new Vector2(220, 20);

            var slider = sliderObj.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;

            // Background
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(sliderObj.transform, false);
            var bgRT = bg.AddComponent<RectTransform>();
            bgRT.anchorMin = Vector2.zero;
            bgRT.anchorMax = Vector2.one;
            bgRT.sizeDelta = Vector2.zero;
            var bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(0.2f, 0.22f, 0.28f);

            // Fill Area
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderObj.transform, false);
            var faRT = fillArea.AddComponent<RectTransform>();
            faRT.anchorMin = new Vector2(0, 0.25f);
            faRT.anchorMax = new Vector2(1, 0.75f);
            faRT.sizeDelta = Vector2.zero;

            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            var fRT = fill.AddComponent<RectTransform>();
            fRT.anchorMin = Vector2.zero;
            fRT.anchorMax = Vector2.one;
            fRT.sizeDelta = Vector2.zero;
            var fImg = fill.AddComponent<Image>();
            fImg.color = new Color(0.2f, 0.75f, 0.9f);
            slider.fillRect = fRT;

            return slider;
        }

        private GameObject CreateUIButton(Transform parent, string text, Vector2 pos, Vector2 size, Color btnColor)
        {
            GameObject btnObj = new GameObject("Button_" + text);
            btnObj.transform.SetParent(parent, false);
            var rt = btnObj.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            var img = btnObj.AddComponent<Image>();
            img.color = btnColor;

            var btn = btnObj.AddComponent<Button>();
            var colors = btn.colors;
            colors.highlightedColor = btnColor * 1.25f;
            colors.pressedColor = btnColor * 0.8f;
            btn.colors = colors;

            CreateText(btnObj.transform, text, 22, Vector2.zero, size, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);

            return btnObj;
        }

        private GameObject CreateText(Transform parent, string content, int fontSize, Vector2 pos, Vector2 size, TextAnchor anchor, Color color, FontStyle style = FontStyle.Normal)
        {
            GameObject tObj = new GameObject("Text");
            tObj.transform.SetParent(parent, false);
            var rt = tObj.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            var t = tObj.AddComponent<Text>();
            t.text = content;
            t.fontSize = fontSize;
            t.alignment = anchor;
            t.color = color;
            t.fontStyle = style;
            t.font = MaterialUtil.GetUIFont();

            return tObj;
        }
        #endregion
    }

    public class PedestalTrigger : MonoBehaviour
    {
        public Environment.ColorPuzzle puzzle;
        public int colorIndex;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.GetComponent<Player.PlayerController>() != null)
            {
                puzzle?.StepOnPedestal(colorIndex);
            }
        }
    }
}

namespace BatteryRun.Environment
{
    public class SimpleRotator : MonoBehaviour
    {
        public float rotationSpeed = 10f;
        private void Update()
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}
