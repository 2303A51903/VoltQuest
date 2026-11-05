using UnityEngine;
using UnityEngine.Rendering;

namespace BatteryRun.Levels
{
    /// <summary>
    /// Central helper for runtime-created materials and UI resources.
    /// Handles missing shaders (stripped builds / URP projects) and Unity 2022.2+ built-in font changes.
    /// </summary>
    public static class MaterialUtil
    {
        private static Shader cachedShader;
        private static Font cachedFont;
        private static Sprite whiteSprite;

        public static Shader GetShader()
        {
            if (cachedShader != null) return cachedShader;

            string[] candidates = GraphicsSettings.currentRenderPipeline != null
                ? new[] { "Universal Render Pipeline/Lit", "HDRP/Lit", "Standard", "Legacy Shaders/Diffuse", "Unlit/Color", "Sprites/Default" }
                : new[] { "Standard", "Legacy Shaders/Diffuse", "Unlit/Color", "Sprites/Default" };

            foreach (string c in candidates)
            {
                Shader s = Shader.Find(c);
                if (s != null)
                {
                    cachedShader = s;
                    return s;
                }
            }
            Debug.LogError("[Battery Run] No usable shader found for runtime materials.");
            return null;
        }

        public static Material NewMaterial()
        {
            return new Material(GetShader());
        }

        public static Material Create(string name, Color c, float metallic = 0f, bool emissive = false)
        {
            Material m = NewMaterial();
            m.name = name;
            m.color = c;
            if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metallic);
            if (m.HasProperty("_Glossiness")) m.SetFloat("_Glossiness", 0.4f);
            if (emissive && m.HasProperty("_EmissionColor"))
            {
                m.EnableKeyword("_EMISSION");
                m.SetColor("_EmissionColor", c * 2.0f);
            }
            if (c.a < 0.99f) MakeTransparent(m);
            return m;
        }

        /// <summary>Switches a Standard-shader material to the Fade/Transparent mode so alpha works.</summary>
        public static void MakeTransparent(Material m)
        {
            if (m == null || m.shader == null || m.shader.name != "Standard") return;
            m.SetFloat("_Mode", 3f);
            m.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            m.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            m.SetInt("_ZWrite", 0);
            m.DisableKeyword("_ALPHATEST_ON");
            m.EnableKeyword("_ALPHABLEND_ON");
            m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            m.renderQueue = 3000;
        }

        /// <summary>
        /// Unity 2022.2+ removed "Arial.ttf" as a built-in font (GetBuiltinResource throws).
        /// Try the new name first, then the old one, then an OS font.
        /// </summary>
        public static Font GetUIFont()
        {
            if (cachedFont != null) return cachedFont;
            try { cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); } catch { cachedFont = null; }
            if (cachedFont == null)
            {
                try { cachedFont = Resources.GetBuiltinResource<Font>("Arial.ttf"); } catch { cachedFont = null; }
            }
            if (cachedFont == null) cachedFont = Font.CreateDynamicFontFromOSFont("Arial", 16);
            return cachedFont;
        }

        /// <summary>A 4x4 white sprite. UI Image.fillAmount is ignored when an Image has no sprite.</summary>
        public static Sprite GetWhiteSprite()
        {
            if (whiteSprite != null) return whiteSprite;
            Texture2D tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
            Color[] px = new Color[16];
            for (int i = 0; i < px.Length; i++) px[i] = Color.white;
            tex.SetPixels(px);
            tex.Apply();
            whiteSprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 100f);
            return whiteSprite;
        }

        /// <summary>
        /// Primitive cylinders ship with a CapsuleCollider. When a cylinder is squashed (e.g. scale 3.5,0.4,3.5)
        /// the capsule height is clamped to its diameter, which turns a flat disc into a big sphere that blocks the
        /// player. Swap it for a convex MeshCollider that matches the real cylinder shape.
        /// </summary>
        public static void FixCylinderCollider(GameObject go, bool trigger = false)
        {
            var cap = go.GetComponent<CapsuleCollider>();
            if (cap != null) Object.DestroyImmediate(cap);
            if (go.GetComponent<MeshCollider>() != null) return;
            var mf = go.GetComponent<MeshFilter>();
            var mc = go.AddComponent<MeshCollider>();
            if (mf != null) mc.sharedMesh = mf.sharedMesh;
            mc.convex = true;
            mc.isTrigger = trigger;
        }
    }
}
