using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace HowToFishTrainer
{
    [BepInPlugin("orqz.howtofish.menu", CFG.Name, "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        // static + public so the Harmony patch in patches.cs can read it
        public static bool MenuOpen;

        private Harmony _harmony;
        private Rect _windowRect;
        private int _tab;

        // animation state
        private float _anim;        // 0 = closed, 1 = fully open
        private float _indicator;   // slides toward _tab
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

        // ScriptEngine reloads us on every build - without this the old patches stay
        // applied and stack up, one more copy each reload.
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

            // Ease toward the target every frame. unscaledDeltaTime so it still animates
            // if the game is paused or slowed down.
            float target = MenuOpen ? 1f : 0f;
            _anim = Mathf.MoveTowards(_anim, target, Time.unscaledDeltaTime / CFG.OpenTime);
            _indicator = Mathf.Lerp(_indicator, _tab, Time.unscaledDeltaTime * CFG.TabSlideSpeed);
        }

        private void OnGUI()
        {
            Theme.Build();
            Buffs.Draw();          // always on screen, menu open or not

            if (_anim <= 0.001f) return;

            // ease-out cubic: fast at the start, settles gently
            float e = 1f - Mathf.Pow(1f - _anim, 3f);

            var oldMatrix = GUI.matrix;
            var oldColor = GUI.color;

            // scale up from 88% and fade in, pivoting on the window's centre
            float scale = Mathf.Lerp(0.88f, 1f, e);
            GUIUtility.ScaleAroundPivot(new Vector2(scale, scale), _windowRect.center);
            GUI.color = new Color(1f, 1f, 1f, e);

            // drop shadow, offset down-right behind the window
            Theme.Fill(new Rect(_windowRect.x + 7f, _windowRect.y + 9f,
                                _windowRect.width, _windowRect.height),
                       new Color(0f, 0f, 0f, 0.4f * e));

            Theme.Window.normal.textColor = Theme.Pulse;   // glowing title
            _windowRect = GUI.Window(0, _windowRect, DrawWindow, CFG.Name + "  " + CFG.Version, Theme.Window);

            GUI.color = oldColor;
            GUI.matrix = oldMatrix;
        }

        private void DrawWindow(int id)
        {
            _tab = GUILayout.Toolbar(_tab, Tabs, Theme.Tab);

            // GetLastRect gives us where the toolbar just got drawn, so the underline
            // can follow it no matter how the window is sized.
            Rect bar = GUILayoutUtility.GetLastRect();
            float tabW = bar.width / Tabs.Length;
            Theme.Fill(new Rect(bar.x + _indicator * tabW, bar.yMax - 2f, tabW, 2f), Theme.Pulse);

            GUILayout.Space(10);

            // one scroll view around every tab, so a tab can grow without
            // spilling out of the window
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
