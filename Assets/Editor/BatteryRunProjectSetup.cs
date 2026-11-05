#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BatteryRun.Editor
{
    public static class BatteryRunProjectSetup
    {
        [MenuItem("Battery Run/Play Game", false, 0)]
        public static void PlayGame()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
                EditorApplication.isPlaying = true;
            }
        }

        [MenuItem("Battery Run/Open Main Menu", false, 1)]
        public static void OpenMainMenu()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        }

        [MenuItem("Battery Run/Open Level 1 (Lost Island)", false, 10)]
        public static void OpenLevel1()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Level1_LostIsland.unity");
        }

        [MenuItem("Battery Run/Open Level 2 (Dangerous Jungle)", false, 11)]
        public static void OpenLevel2()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Level2_DangerousJungle.unity");
        }

        [MenuItem("Battery Run/Open Level 3 (Crystal Cave)", false, 12)]
        public static void OpenLevel3()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Level3_CrystalCave.unity");
        }

        [MenuItem("Battery Run/Open Level 4 (Final Core)", false, 13)]
        public static void OpenLevel4()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Level4_FinalCore.unity");
        }

        [MenuItem("Battery Run/Setup Build Settings and Tags", false, 50)]
        public static void SetupBuildSettingsAndTags()
        {
            // Register Scenes in Build Settings
            string[] scenePaths = new string[]
            {
                "Assets/Scenes/MainMenu.unity",
                "Assets/Scenes/Level1_LostIsland.unity",
                "Assets/Scenes/Level2_DangerousJungle.unity",
                "Assets/Scenes/Level3_CrystalCave.unity",
                "Assets/Scenes/Level4_FinalCore.unity"
            };

            EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[scenePaths.Length];
            for (int i = 0; i < scenePaths.Length; i++)
            {
                buildScenes[i] = new EditorBuildSettingsScene(scenePaths[i], true);
            }
            EditorBuildSettings.scenes = buildScenes;

            Debug.Log("[Battery Run] All 5 scenes registered in EditorBuildSettings!");
        }
    }
}
#endif
