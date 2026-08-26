using System.Collections.Generic;
using UnityEngine;

namespace HowToFishTrainer
{
    // The little status pills in the corner. Always drawn, menu open or not.
    public static class Buffs
    {
        // Flags for features we implement ourselves. Game-owned states are read
        // straight from the game below, so they can never fall out of sync.
        public static float SpeedMultiplier = 1f;
        public static bool InfiniteHealth;

        private static readonly List<string> Rows = new List<string>();
        private static float _slide;   // eases in from the right

        public static void Draw()
        {
            Theme.Build();
            Rows.Clear();

            Add(PlayerManager.InGodMode, "GOD MODE");
            Add(NoRecoil.Enabled, "NO RECOIL");
            Add(InfiniteJump.Enabled, "INF JUMPS");
            Add(RiggedRoulette.Enabled, "RIGGED WHEEL");
            Add(ThirdPerson.Enabled, "3RD PERSON");
            Add(OneShotOn(), "ONE SHOT");
            Add(InfiniteHealth, "INF HEALTH");
            Add(!Mathf.Approximately(SpeedMultiplier, 1f), "SPEED  x" + SpeedMultiplier.ToString("0.0"));

            // slide the whole stack in when there's something to show
            float target = Rows.Count > 0 ? 1f : 0f;
            _slide = Mathf.MoveTowards(_slide, target, Time.unscaledDeltaTime * 6f);
            if (_slide <= 0.001f) return;

            float ease = 1f - Mathf.Pow(1f - _slide, 3f);
            float y = 18f;

            for (int i = 0; i < Rows.Count; i++)
            {
                var content = new GUIContent(Rows[i]);
                Vector2 size = Theme.Pill.CalcSize(content);
                float w = size.x + 6f;
                float h = Mathf.Max(size.y + 2f, 30f);

                // off-screen to the right at ease 0, resting position at ease 1
                float x = Mathf.Lerp(Screen.width + 10f, Screen.width - w - 18f, ease);
                var r = new Rect(x, y, w, h);

                var old = GUI.color;
                GUI.color = new Color(1f, 1f, 1f, ease);
                GUI.Label(r, content, Theme.Pill);

                // the pulsing dot on the left of each pill
                Theme.Fill(new Rect(r.x + 10f, r.y + h / 2f - 3f, 6f, 6f),
                           new Color(Theme.Pulse.r, Theme.Pulse.g, Theme.Pulse.b, ease));
                GUI.color = old;

                y += h + 6f;
            }
        }

        private static void Add(bool on, string label)
        {
            if (on) Rows.Add(label);
        }

        // OneShotEnabled reaches through Instance, which is null outside a game.
        private static bool OneShotOn()
        {
            return ServerSettings.Instance != null && ServerSettings.OneShotEnabled;
        }
    }
}
