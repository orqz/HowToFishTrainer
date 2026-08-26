using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace HowToFishTrainer
{
    [BepInPlugin("orqz.howtofish.menu", CFG.Name, "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static bool MenuOpen;

        private Harmony _harmony;
        private Rect _windowRect;
        private int _tab;

        private float _anim;
        private float _indicator;
        private Vector2 _scroll;

        private static readonly string[] Tabs = { "Money", "Player", "Spawn", "World" };

        private void Awake()
        {
            Logger.LogInfo(CFG.Name + " loaded, version " + CFG.Version);

            _harmony = new Harmony("orqz.howtofish.menu");
            _harmony.PatchAll();

            _windowRect = new Rect(
                Screen.width / 2 - CFG.WindowWidth / 2,
                Screen.height / 2 - CFG.WindowHeight / 2,
                CFG.WindowWidth,
                CFG.WindowHeight);
        }

        private void OnDestroy()
        {
            MenuOpen = false;
            _harmony?.UnpatchSelf();
        }

        private void Update()
        {
            if (Input.GetKeyDown(CFG.MenuKey))
            {
                MenuOpen = !MenuOpen;
                PlayerCamera.ToggleMouse(MenuOpen);
                Logger.LogInfo("menu open: " + MenuOpen);
            }

            float target = MenuOpen ? 1f : 0f;
            _anim = Mathf.MoveTowards(_anim, target, Time.unscaledDeltaTime / CFG.OpenTime);
            _indicator = Mathf.Lerp(_indicator, _tab, Time.unscaledDeltaTime * CFG.TabSlideSpeed);
        }

        private void OnGUI()
        {
            Theme.Build();
            Buffs.Draw();

            if (_anim <= 0.001f) return;

            float e = 1f - Mathf.Pow(1f - _anim, 3f);

            var oldMatrix = GUI.matrix;
            var oldColor = GUI.color;

            float scale = Mathf.Lerp(0.88f, 1f, e);
            GUIUtility.ScaleAroundPivot(new Vector2(scale, scale), _windowRect.center);
            GUI.color = new Color(1f, 1f, 1f, e);

            Theme.Fill(new Rect(_windowRect.x + 7f, _windowRect.y + 9f,
                                _windowRect.width, _windowRect.height),
                       new Color(0f, 0f, 0f, 0.4f * e));

            Theme.Window.normal.textColor = Theme.Pulse;
            _windowRect = GUI.Window(0, _windowRect, DrawWindow, CFG.Name + "  " + CFG.Version, Theme.Window);

            GUI.color = oldColor;
            GUI.matrix = oldMatrix;
        }

        private void DrawWindow(int id)
        {
            _tab = GUILayout.Toolbar(_tab, Tabs, Theme.Tab);

            Rect bar = GUILayoutUtility.GetLastRect();
            float tabW = bar.width / Tabs.Length;
            Theme.Fill(new Rect(bar.x + _indicator * tabW, bar.yMax - 2f, tabW, 2f), Theme.Pulse);

            GUILayout.Space(10);

            _scroll = GUILayout.BeginScrollView(_scroll);
            if (_tab == 0) MoneyTab.Draw();
            else if (_tab == 1) PlayerTab.Draw();
            else if (_tab == 2) SpawnTab.Draw();
            else if (_tab == 3) WorldTab.Draw();
            GUILayout.EndScrollView();
            GUILayout.Label("press " + CFG.MenuKey + " to close", Theme.Small);

            GUI.DragWindow();
        }
    }
}
