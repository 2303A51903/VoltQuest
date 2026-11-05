using UnityEngine;

namespace BatteryRun.Collectibles
{
    public class BatteryPickup : CollectibleItem
    {
        protected override void Start()
        {
            scoreValue = 100;
            batteryRestoreAmount = 20.0f;
            base.Start();

            if (transform.childCount == 0)
            {
                BuildBatteryVisual();
            }
        }

        public override void Collect(GameObject player, Player.BatterySystem batterySystem)
        {
            if (isCollected) return;

            Audio.SoundManager.Instance?.PlaySFX("BatteryPickup");
            Managers.LevelManager.Instance?.RecordBatteryCollected();

            base.Collect(player, batterySystem);
        }

        protected override Color GetEffectColor()
        {
            return new Color(0.2f, 0.95f, 0.3f); // Neon Green
        }

        private void BuildBatteryVisual()
        {
            // Outer Yellow/Black casing
            GameObject casing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            casing.name = "BatteryCasing";
            casing.transform.SetParent(transform);
            casing.transform.localPosition = Vector3.zero;
            casing.transform.localScale = new Vector3(0.4f, 0.45f, 0.4f);
            DestroyImmediate(casing.GetComponent<Collider>());

            // Glowing energy core inside
            GameObject core = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            core.name = "EnergyCore";
            core.transform.SetParent(casing.transform);
            core.transform.localPosition = Vector3.zero;
            core.transform.localScale = new Vector3(0.92f, 0.7f, 0.92f);
            DestroyImmediate(core.GetComponent<Collider>());

            // Positive terminal on top
            GameObject terminal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            terminal.name = "PositiveTerminal";
            terminal.transform.SetParent(casing.transform);
            terminal.transform.localPosition = new Vector3(0, 1.1f, 0);
            terminal.transform.localScale = new Vector3(0.45f, 0.18f, 0.45f);
            DestroyImmediate(terminal.GetComponent<Collider>());

            // Materials
            Material caseMat = Levels.MaterialUtil.NewMaterial();
            caseMat.color = new Color(0.95f, 0.75f, 0.1f); // Vibrant Gold
            if (caseMat.HasProperty("_Metallic")) caseMat.SetFloat("_Metallic", 0.6f);
            casing.GetComponent<Renderer>().material = caseMat;

            Material coreMat = Levels.MaterialUtil.NewMaterial();
            coreMat.color = new Color(0.1f, 1.0f, 0.4f); // Neon Energy Green
            if (coreMat.HasProperty("_EmissionColor"))
            {
                coreMat.EnableKeyword("_EMISSION");
                coreMat.SetColor("_EmissionColor", new Color(0.1f, 1.0f, 0.4f) * 2.0f);
            }
            core.GetComponent<Renderer>().material = coreMat;

            Material termMat = Levels.MaterialUtil.NewMaterial();
            termMat.color = new Color(0.8f, 0.8f, 0.85f);
            terminal.GetComponent<Renderer>().material = termMat;
        }
    }
}