using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace HowToFishTrainer
{
    [HarmonyPatch(typeof(Player), nameof(Player.BlockInputs), MethodType.Getter)]
    public static class BlockInputsWhileMenuOpen
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            if (Plugin.MenuOpen) __result = true;
        }
    }

    [HarmonyPatch(typeof(PlayerCamera), nameof(PlayerCamera.Recoil))]
    public static class NoRecoil
    {
        public static bool Enabled;

        [HarmonyPrefix]
        public static bool Prefix()
        {
            return !Enabled;
        }
    }

    [HarmonyPatch(typeof(PlayerMovement), "JumpInput")]
    public static class InfiniteJump
    {
        public static bool Enabled;

        [HarmonyPrefix]
        public static bool Prefix(PlayerMovement __instance)
        {
            if (!Enabled) return true;

            if (Player.LocalPlayer != null && Player.LocalPlayer.BlockInputs) return false;

            __instance.Jump();
            return false;
        }
    }

    [HarmonyPatch(typeof(CasinoManager), nameof(CasinoManager.ServerRouletteResult))]
    public static class RiggedRoulette
    {
        public static bool Enabled;

        private static readonly FieldInfo CurBetColor =
            AccessTools.Field(typeof(CasinoManager), "_curBetColor");

        [HarmonyPrefix]
        public static void Prefix(ref BetColor winColor)
        {
            if (!Enabled || CurBetColor == null) return;
            winColor = (BetColor)CurBetColor.GetValue(null);
        }
    }

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

            RaycastHit hit;
            if (Physics.Raycast(head, back, out hit, Distance + 0.4f, GameInfo.LevelLayer))
                dist = Mathf.Max(0f, hit.distance - 0.4f);

            cam.position = head + back * dist;
        }
    }
}
