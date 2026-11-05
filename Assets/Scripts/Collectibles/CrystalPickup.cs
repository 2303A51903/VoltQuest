using UnityEngine;

namespace BatteryRun.Collectibles
{
    public class CrystalPickup : CollectibleItem
    {
        public bool isSecret = true;

        protected override void Start()
        {
            scoreValue = 250;
            batteryRestoreAmount = 35.0f;
            rotationSpeed = 120.0f; // Spins faster with mystical energy
            bobSpeed = 3.0f;
            base.Start();

            if (transform.childCount == 0)
            {
                BuildCrystalVisual();
            }
        }

        public override void Collect(GameObject player, Player.BatterySystem batterySystem)
        {
            if (isCollected) return;

            Audio.SoundManager.Instance?.PlaySFX("CrystalPickup");
            Managers.LevelManager.Instance?.RecordCrystalCollected();

            base.Collect(player, batterySystem);
        }

        protected override Color GetEffectColor()
        {
            return new Color(0.8f, 0.2f, 1.0f); // Radiant Magenta/Purple
        }

        private void BuildCrystalVisual()
        {
            // Octahedral / Double Cone Crystal using rotated cubes or spheres
            GameObject mainCrystal = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mainCrystal.name = "CrystalCore";
            mainCrystal.transform.SetParent(transform);
            mainCrystal.transform.localPosition = Vector3.zero;
            mainCrystal.transform.localRotation = Quaternion.Euler(45f, 45f, 45f);
            mainCrystal.transform.localScale = new Vector3(0.5f, 0.7f, 0.5f);
            DestroyImmediate(mainCrystal.GetComponent<Collider>());

            // Outer floating ring
            GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "EnergyRing";
            ring.transform.SetParent(transform);
            ring.transform.localPosition = Vector3.zero;
            ring.transform.localScale = new Vector3(0.85f, 0.02f, 0.85f);
            DestroyImmediate(ring.GetComponent<Collider>());

            // Materials
            Material crystalMat = Levels.MaterialUtil.NewMaterial();
            crystalMat.color = new Color(0.85f, 0.25f, 1.0f); // Glowing Violet
            if (crystalMat.HasProperty("_EmissionColor"))
            {
                crystalMat.EnableKeyword("_EMISSION");
                crystalMat.SetColor("_EmissionColor", new Color(0.85f, 0.25f, 1.0f) * 2.5f);
            }
            mainCrystal.GetComponent<Renderer>().material = crystalMat;

            Material ringMat = Levels.MaterialUtil.NewMaterial();
            ringMat.color = new Color(0.2f, 0.85f, 1.0f); // Cyan Orbit
            if (ringMat.HasProperty("_EmissionColor"))
            {
                ringMat.EnableKeyword("_EMISSION");
                ringMat.SetColor("_EmissionColor", new Color(0.2f, 0.85f, 1.0f) * 2.0f);
            }
            ring.GetComponent<Renderer>().material = ringMat;
        }
    }
}