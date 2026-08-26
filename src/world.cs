using UnityEngine;

namespace HowToFishTrainer
{
    public static class WorldTab
    {
        private static string _armed;
        private static float _armedUntil;

        public static void Draw()
        {
            if (OnlineIslandManager.Instance == null)
            {
                GUILayout.Label("WORLD", Theme.Header);
                GUILayout.Label("Load a world as host first.", Theme.Small);
                return;
            }

            GUILayout.Label("ISLAND  " + OnlineIslandManager.CurIsland
                          + " / " + (IslandManager.TotalIslands - 1)
                          + "   (unlocked up to " + OnlineIslandManager.MaxIslandUnlocked + ")", Theme.Header);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("< Previous", Theme.Button)) OnlineIslandManager.TpToNextIsland(true);
            if (GUILayout.Button("Next >", Theme.Button)) OnlineIslandManager.TpToNextIsland(false);
            GUILayout.EndHorizontal();

            for (int i = 0; i < IslandManager.TotalIslands; i++)
            {
                bool here = i == OnlineIslandManager.CurIsland;
                if (GUILayout.Button(here ? "> Island " + i : "Island " + i, Theme.Button))
                    OnlineIslandManager.TpToSpecificIsland((byte)i);
            }

            GUILayout.Label("CREATURES", Theme.Header);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Kill all", Theme.Button)) GameInfo.ToggleAllCreaturesKilled(true, false);
            if (GUILayout.Button("Reset all", Theme.Button)) GameInfo.ToggleAllCreaturesKilled(false, false);
            GUILayout.EndHorizontal();

            if (GUILayout.Button(BossManager.Boss != null ? "Kill boss" : "Kill boss  (none active)", Theme.Button))
                KillBoss();

            GUILayout.Label("CASINO", Theme.Header);
            if (GUILayout.Button(RiggedRoulette.Enabled ? "Rigged roulette: ON" : "Rigged roulette: OFF", Theme.Button))
                RiggedRoulette.Enabled = !RiggedRoulette.Enabled;
            GUILayout.Label("The wheel always lands on the colour you bet. Green pays 35x.", Theme.Small);

            GUILayout.Label("TIME", Theme.Header);
            GUILayout.Label("Game speed: " + Time.timeScale.ToString("0.00") + "x", Theme.Small);
            float next = GUILayout.HorizontalSlider(Time.timeScale, 0.1f, 3f);
            if (!Mathf.Approximately(next, Time.timeScale)) Time.timeScale = next;
            if (GUILayout.Button("Reset to 1.00x", Theme.Button)) Time.timeScale = 1f;

            GUILayout.Label("IRREVERSIBLE", Theme.Header);
            Guarded("Finish the game (roll credits)", "finish", FinishGame);
        }

        private static void KillBoss()
        {
            Creature boss = BossManager.Boss;
            if (boss == null || Player.LocalPlayer == null) return;

            boss.LocalHit(boss.transform, boss.transform.position, Vector3.up,
                          Player.LocalPlayer, 999999, false, Vector3.zero);
        }

        private static void FinishGame()
        {
            if (EndGameManager.Instance != null) EndGameManager.Instance.FinishGameInput();
        }

        private static void Guarded(string label, string key, System.Action act)
        {
            bool armed = _armed == key && Time.unscaledTime < _armedUntil;
            if (GUILayout.Button(armed ? "Click again to confirm" : label, Theme.Button))
            {
                if (armed) { _armed = null; act(); }
                else { _armed = key; _armedUntil = Time.unscaledTime + 3f; }
            }
        }
    }
}
