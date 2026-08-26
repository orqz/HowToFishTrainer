using UnityEngine;

namespace HowToFishTrainer
{
    public static class Theme
    {
        public static Color BgTop    = new Color(0.09f, 0.10f, 0.14f, 0.98f);
        public static Color BgBottom = new Color(0.04f, 0.04f, 0.06f, 0.98f);
        public static Color Panel    = new Color(0.13f, 0.14f, 0.18f, 1f);
        public static Color PanelHot = new Color(0.18f, 0.20f, 0.26f, 1f);
        public static Color Accent   = new Color(0.15f, 0.90f, 0.80f, 1f);
        public static Color Accent2  = new Color(0.60f, 0.40f, 1.00f, 1f);
        public static Color Text     = new Color(0.86f, 0.89f, 0.94f, 1f);

        public static GUIStyle Window, Button, Label, Header, Tab, TextField, Small, Big, Pill;
        public static Texture2D AccentTex, ShadowTex, PanelTex;

        private static bool _built;

        public static Color Pulse
        {
            get
            {
                float t = Mathf.PingPong(Time.unscaledTime * 0.4f, 1f);
                return Color.Lerp(Accent, Accent2, Mathf.SmoothStep(0f, 1f, t));
            }
        }

        private static Texture2D Solid(Color c)
        {
            var t = new Texture2D(1, 1) { hideFlags = HideFlags.HideAndDontSave };
            t.SetPixel(0, 0, c);
            t.Apply();
            return t;
        }

        private static Texture2D Rounded(Color fill, Color edge, int size = 32, float radius = 10f, float edgeWidth = 1.2f)
        {
            var t = new Texture2D(size, size, TextureFormat.ARGB32, false)
            {
                hideFlags = HideFlags.HideAndDontSave,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            float half = size / 2f;
            var b = new Vector2(half - radius, half - radius);
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                var p = new Vector2(x + 0.5f - half, y + 0.5f - half);
                var q = new Vector2(Mathf.Abs(p.x) - b.x, Mathf.Abs(p.y) - b.y);
                float d = Vector2.Max(q, Vector2.zero).magnitude
                        + Mathf.Min(Mathf.Max(q.x, q.y), 0f) - radius;

                Color c = fill;
                if (d > -edgeWidth) c = Color.Lerp(edge, fill, Mathf.Clamp01(-d / edgeWidth));
                c.a *= Mathf.Clamp01(0.5f - d);
                t.SetPixel(x, y, c);
            }
            t.Apply();
            return t;
        }

        private static Texture2D Gradient(Color bottom, Color top, int h = 128)
        {
            var t = new Texture2D(1, h) { hideFlags = HideFlags.HideAndDontSave };
            t.wrapMode = TextureWrapMode.Clamp;
            for (int y = 0; y < h; y++)
                t.SetPixel(0, y, Color.Lerp(bottom, top, y / (float)(h - 1)));
            t.Apply();
            return t;
        }

        public static void Build()
        {
            if (_built) return;
            _built = true;

            AccentTex = Solid(Color.white);
            ShadowTex = Solid(new Color(0f, 0f, 0f, 0.45f));
            PanelTex  = Solid(Panel);

            var windowTex = Rounded(BgTop, new Color(1f, 1f, 1f, 0.13f), 40, 12f, 1.4f);
            var btn       = Rounded(Panel,    new Color(1f, 1f, 1f, 0.06f), 24, 6f);
            var btnHover  = Rounded(PanelHot, new Color(1f, 1f, 1f, 0.18f), 24, 6f);
            var btnDown   = Rounded(Accent * 0.55f, Accent, 24, 6f);
            var field     = Rounded(new Color(0.05f, 0.06f, 0.08f, 1f), new Color(1f, 1f, 1f, 0.10f), 24, 6f);
            var fieldHot  = Rounded(new Color(0.06f, 0.08f, 0.11f, 1f), Accent * 0.8f, 24, 6f);
            var tabOff    = Rounded(new Color(1f, 1f, 1f, 0.02f), new Color(0f, 0f, 0f, 0f), 24, 6f);
            var tabOn     = Rounded(new Color(1f, 1f, 1f, 0.09f), new Color(1f, 1f, 1f, 0.14f), 24, 6f);

            var round12 = new RectOffset(14, 14, 14, 14);
            var round6  = new RectOffset(8, 8, 8, 8);

            Window = new GUIStyle(GUI.skin.window);
            Window.normal.background = windowTex;
            Window.onNormal.background = windowTex;
            Window.border = round12;
            Window.normal.textColor = Text;
            Window.onNormal.textColor = Text;
            Window.fontSize = 15;
            Window.fontStyle = FontStyle.Bold;
            Window.alignment = TextAnchor.UpperCenter;
            Window.padding = new RectOffset(18, 18, 34, 16);

            Button = new GUIStyle(GUI.skin.button);
            Button.normal.background = btn;
            Button.hover.background = btnHover;
            Button.active.background = btnDown;
            Button.border = round6;
            Button.normal.textColor = Text;
            Button.hover.textColor = Color.white;
            Button.active.textColor = Color.white;
            Button.fontSize = 13;
            Button.alignment = TextAnchor.MiddleCenter;
            Button.padding = new RectOffset(10, 10, 9, 9);
            Button.margin = new RectOffset(3, 3, 4, 4);

            Label = new GUIStyle(GUI.skin.label);
            Label.normal.textColor = Text;
            Label.fontSize = 13;
            Label.wordWrap = true;
            Label.margin = new RectOffset(3, 3, 2, 2);

            Big = new GUIStyle(GUI.skin.label);
            Big.normal.textColor = Color.white;
            Big.fontSize = 26;
            Big.fontStyle = FontStyle.Bold;
            Big.margin = new RectOffset(3, 3, 0, 6);

            Header = new GUIStyle(GUI.skin.label);
            Header.fontSize = 10;
            Header.fontStyle = FontStyle.Bold;
            Header.normal.textColor = Accent;
            Header.margin = new RectOffset(3, 3, 12, 3);

            TextField = new GUIStyle(GUI.skin.textField);
            TextField.normal.background = field;
            TextField.focused.background = fieldHot;
            TextField.hover.background = fieldHot;
            TextField.border = round6;
            TextField.normal.textColor = Color.white;
            TextField.focused.textColor = Color.white;
            TextField.fontSize = 15;
            TextField.alignment = TextAnchor.MiddleCenter;
            TextField.padding = new RectOffset(8, 8, 9, 9);
            TextField.margin = new RectOffset(3, 3, 2, 6);

            Small = new GUIStyle(GUI.skin.label);
            Small.fontSize = 10;
            Small.wordWrap = true;
            Small.normal.textColor = new Color(0.48f, 0.51f, 0.58f);
            Small.margin = new RectOffset(3, 3, 6, 0);

            Pill = new GUIStyle(GUI.skin.label);
            Pill.normal.background = Rounded(new Color(0.04f, 0.05f, 0.07f, 0.82f),
                                             new Color(1f, 1f, 1f, 0.10f), 24, 8f);
            Pill.border = new RectOffset(10, 10, 10, 10);
            Pill.wordWrap = false;
            Pill.fontSize = 12;
            Pill.fontStyle = FontStyle.Bold;
            Pill.alignment = TextAnchor.MiddleLeft;
            Pill.padding = new RectOffset(24, 14, 7, 7);
            Pill.normal.textColor = Color.white;

            Tab = new GUIStyle(GUI.skin.button);
            Tab.normal.background = tabOff;
            Tab.hover.background = tabOn;
            Tab.onNormal.background = tabOn;
            Tab.onHover.background = tabOn;
            Tab.border = round6;
            Tab.normal.textColor = new Color(0.52f, 0.56f, 0.63f);
            Tab.hover.textColor = Color.white;
            Tab.onNormal.textColor = Color.white;
            Tab.fontSize = 12;
            Tab.fontStyle = FontStyle.Bold;
            Tab.padding = new RectOffset(6, 6, 9, 9);
            Tab.margin = new RectOffset(2, 2, 0, 0);
        }

        public static void Fill(Rect r, Color c)
        {
            var old = GUI.color;
            GUI.color = c;
            GUI.DrawTexture(r, AccentTex);
            GUI.color = old;
        }
    }
}
