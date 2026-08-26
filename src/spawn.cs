using System.Collections.Generic;
using UnityEngine;

namespace HowToFishTrainer
{
    public static class SpawnTab
    {
        private static List<Item> _all;
        private static List<Item> _shown = new List<Item>();
        private static string _search = "";
        private static string _lastSearch = null;
        private static bool _dead, _drip;

        public static void Draw()
        {
            if (Server.Instance == null)
            {
                GUILayout.Label("SPAWN", Theme.Header);
                GUILayout.Label("Spawning is server-side. Load a world as host first.", Theme.Small);
                return;
            }

            BuildList();

            GUILayout.Label("SEARCH  (" + _all.Count + " items)", Theme.Header);
            _search = GUILayout.TextField(_search ?? "", 24, Theme.TextField);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(_dead ? "Dead: ON" : "Dead: OFF", Theme.Button)) _dead = !_dead;
            if (GUILayout.Button(_drip ? "Drip: ON" : "Drip: OFF", Theme.Button)) _drip = !_drip;
            GUILayout.EndHorizontal();

            // Only refilter during Layout - changing the control count between the
            // Layout and Repaint passes makes IMGUI throw.
            if (Event.current.type == EventType.Layout && _search != _lastSearch)
            {
                _lastSearch = _search;
                Filter();
            }

            for (int i = 0; i < _shown.Count; i++)
            {
                Item item = _shown[i];
                if (item == null) continue;
                if (GUILayout.Button(Pretty(item.name), Theme.Button))
                    Spawn(item);
            }
            if (_shown.Count == 0) GUILayout.Label("nothing matches", Theme.Small);

            GUILayout.Label("Spawns 2m in front of your camera.", Theme.Small);
        }

        private static void BuildList()
        {
            if (_all != null && _all.Count > 0) return;

            _all = new List<Item>();
            // ids are bytes, and 255 is reserved as the boat's stand-in id
            for (int id = 0; id < byte.MaxValue; id++)
            {
                Item item = GameInfo.GetSpawnable((byte)id);
                if (item != null) _all.Add(item);
            }
            _all.Sort((a, b) => string.Compare(a.name, b.name, System.StringComparison.OrdinalIgnoreCase));
            _shown = new List<Item>(_all);
        }

        private static void Filter()
        {
            _shown.Clear();
            for (int i = 0; i < _all.Count; i++)
            {
                if (string.IsNullOrEmpty(_search) ||
                    _all[i].name.IndexOf(_search, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    _shown.Add(_all[i]);
            }
        }

        private static string Pretty(string n)
        {
            int i = n.IndexOf("(Clone)", System.StringComparison.Ordinal);
            return i > 0 ? n.Substring(0, i) : n;
        }

        private static void Spawn(Item prefab)
        {
            Camera cam = GameInfo.CurCamera != null ? GameInfo.CurCamera : Camera.main;
            if (cam == null || Server.Instance == null) return;

            Vector3 pos = cam.transform.position + cam.transform.forward * 2f;
            Item spawned = Object.Instantiate(prefab, pos, Quaternion.identity);

            if (spawned.Creature != null)
            {
                if (_drip) spawned.Creature.SetDrip();
                if (_dead) spawned.Creature.ServerKillOnSpawn();
            }

            Server.Instance.Spawn(spawned.gameObject);
        }
    }
}
