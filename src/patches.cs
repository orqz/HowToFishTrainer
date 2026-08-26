using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace HowToFishTrainer
{
    // The game asks Player.BlockInputs before it lets you move, punch, swing, use the
    // inventory or drive. It's a get-only property computed from dying/paused/typing,
    // all of which we can't set. So instead of setting them, we let the property run
    // normally and then change its answer on the way out.
    [HarmonyPatch(typeof(Player), nameof(Player.BlockInputs), MethodType.Getter)]
    public static class BlockInputsWhileMenuOpen
    {
        // __result is Harmony's name for the value the original returned.
        // `ref` means we can overwrite it.
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            if (Plugin.MenuOpen) __result = true;
        }
    }

    // Weapon.cs calls PlayerCamera.Recoil to kick your view up on every shot.
    // A prefix returning false means the original never runs at all, so the
    // kick is never applied and your aim stays where you put it.
    [HarmonyPatch(typeof(PlayerCamera), nameof(PlayerCamera.Recoil))]
    public static class NoRecoil
    {
        public static bool Enabled;

        [HarmonyPrefix]
        public static bool Prefix()
        {
            return !Enabled;   // false = skip the original method
        }
    }

    // JumpInput normally checks you're grounded (or in coyote time, swimming, on a
    // boat...) before calling Jump(). We skip that whole condition and jump anyway.
    [HarmonyPatch(typeof(PlayerMovement), "JumpInput")]
    public static class InfiniteJump
    {
        public static bool Enabled;

        [HarmonyPrefix]
        public static bool Prefix(PlayerMovement __instance)
        {
            if (!Enabled) return true;

            // still respect BlockInputs, or you'd jump while typing in the menu
            if (Player.LocalPlayer != null && Player.LocalPlayer.BlockInputs) return false;

            __instance.Jump();
            return false;
        }
    }

    // LocalCasino spins the wheel, reads where the ball landed, and hands that colour
    // to ServerRouletteResult, which compares it against the colour you bet on.
    // We rewrite the ball's answer to whatever you bet, so it always matches.
    // Bet Green and it pays 35x.
    [HarmonyPatch(typeof(CasinoManager), nameof(CasinoManager.ServerRouletteResult))]
    public static class RiggedRoulette
    {
        public static bool Enabled;

        // _curBetColor is a private static field, so we reach it by name once and cache it
        private static readonly FieldInfo CurBetColor =
            AccessTools.Field(typeof(CasinoManager), "_curBetColor");

        [HarmonyPrefix]
        public static void Prefix(ref BetColor winColor)
        {
            if (!Enabled || CurBetColor == null) return;
            winColor = (BetColor)CurBetColor.GetValue(null);
        }
    }

    // PlayerCamera.SetCamPosRot puts the camera at your head every frame. We let it
    // do that, then shove the camera backwards along its own forward axis.
    [HarmonyPatch(typeof(PlayerCamera), "SetCamPosRot")]
    public static class ThirdPerson
    {
        public static bool Enabled;
        public static float Distance = 3.5f;

        [HarmonyPostfix]
        public static void Postfix(PlayerCamera __instance)
        {
            if (!Enabled) return;

            Transform cam = __instance.CamTransform;
            if (cam == null) return;

            Vector3 head = cam.position;
            Vector3 back = -cam.forward;
            float dist = Distance;

            // stop the camera pushing through walls - if something's behind you,
            // sit just in front of it instead
            RaycastHit hit;
            if (Physics.Raycast(head, back, out hit, Distance + 0.4f, GameInfo.LevelLayer))
                dist = Mathf.Max(0f, hit.distance - 0.4f);

            cam.position = head + back * dist;
        }
    }
}
