using UnityEngine;

namespace HowToFishTrainer
{
    public static class PlayerTab
    {
        private static string _armed;
        private static float _armedUntil;

        public static void Draw()
        {
            GUILayout.Label("COMBAT", Theme.Header);
            if (GUILayout.Button(PlayerManager.InGodMode ? "God mode: ON" : "God mode: OFF", Theme.Button))
                PlayerManager.ToggleGodMode();

            if (GUILayout.Button(NoRecoil.Enabled ? "No recoil: ON" : "No recoil: OFF", Theme.Button))
                NoRecoil.Enabled = !NoRecoil.Enabled;

            if (GUILayout.Button("Toggle one-shot kills", Theme.Button))
            {
                if (ServerSettings.Instance != null)
                    ServerSettings.Instance.ToggleOneShot();
            }

            GUILayout.Label("MOVEMENT", Theme.Header);
            if (GUILayout.Button(InfiniteJump.Enabled ? "Infinite jumps: ON" : "Infinite jumps: OFF", Theme.Button))
                InfiniteJump.Enabled = !InfiniteJump.Enabled;

            GUILayout.Label("CAMERA", Theme.Header);
            if (GUILayout.Button(ThirdPerson.Enabled ? "Third person: ON" : "Third person: OFF", Theme.Button))
                ThirdPerson.Enabled = !ThirdPerson.Enabled;
            GUILayout.Label("Distance: " + ThirdPerson.Distance.ToString("0.0") + "m", Theme.Small);
            ThirdPerson.Distance = GUILayout.HorizontalSlider(ThirdPerson.Distance, 1f, 10f);

            GUILayout.Label("UNLOCKS", Theme.Header);
            if (GUILayout.Button("Unlock boat", Theme.Button))
            {
                if (BoatManager.Boat != null) BoatManager.Boat.UnlockBoat();
            }
            if (GUILayout.Button("Unlock grill", Theme.Button))
                NPCManager.UnlockGrill();

            GUILayout.Label("SKINS", Theme.Header);
            if (GUILayout.Button("Unlock every skin", Theme.Button)) UnlockAllSkins();
            if (GUILayout.Button("Wipe skins", Theme.Button)) SaveManager.LockAllSkins();
            GUILayout.Label("Local save only - nothing networked, nothing on Steam.", Theme.Small);

            GUILayout.Label("IRREVERSIBLE", Theme.Header);
            Guarded("Unlock ALL achievements", "ach_unlock", () => AchievementManager.ToggleAllAchievements(true));
            Guarded("Reset all achievements + stats", "ach_reset", () => AchievementManager.ToggleAllAchievements(false));
            GUILayout.Label("Writes straight to your Steam account. Reset also wipes all Steam stats, not just achievements.", Theme.Small);
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

        private static void UnlockAllSkins()
        {
            int items = 0, skins = 0;

            for (int id = 0; id < byte.MaxValue; id++)
            {
                Item item = GameInfo.IDToItem((byte)id);
                if (item == null || !item.SkinPreset) continue;

                items++;
                for (int i = 0; i < item.SkinPreset.Skins.Count; i++)
                {
                    SaveManager.UnlockSkin(item.ID, (byte)i);
                    skins++;
                }
            }

            if (BoatManager.Boat != null && BoatManager.Boat.SkinPreset != null)
                for (int i = 0; i < BoatManager.Boat.SkinPreset.Skins.Count; i++)
                    SaveManager.UnlockSkin(byte.MaxValue, (byte)i);

            Debug.Log("[HowToFishTrainer] unlocked " + skins + " skins across " + items + " items");
        }
    }
}
