using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using UnityEngine;
using UnityEngine.Rendering;

using SXLMod;
using GameManagement;

using SkaterXL.Core;
using SkaterXL.TrickDetection;

namespace SXL.Main
{
    #region Player Patches
    [HarmonyPatch(typeof(PlayerState_Impact), "Update")]
    internal class SXLPopDelay_Patch
    {
        private static void PostFix(ref float ___maxImpactTime)
        {
            if ((SXLMod.Customization.SXLSettings.delayPop ? 0 : (SXLRuntime.enabled ? 1 : 0)) == 0)
                return;
            ___maxImpactTime = 0.05f;
        }
    }

    [HarmonyPatch(typeof(PlayerState_Manualling), "FixedUpdate")]
    internal class SXLManualPopDelay_Patch
    {
        private static void PostFix(ref float ___impactTimer)
        {
            if ((SXLMod.Customization.SXLSettings.delayPop ? 0 : (SXLRuntime.enabled ? 1 : 0)) == 0)
                return;
            ___impactTimer = 2f;
        }
    }

    [HarmonyPatch(typeof(PlayerState_Released), "Update")]
    internal class SXLPlayerState_Released_Patch
    {
        private static bool Prefix()
        {
            Debug.Log("Playerstate_Released.Update");
            bool realisticMode = SXLMod.Customization.SXLSettings.realisticMode;
            if ((!realisticMode || !SXLRuntime.enabled ? 0 : realisticMode ? 1 : 0) != 0) 
            {
                PlayerController.Instance.ToggleFlipColliders(false);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(StickInput), "OnStickPressed", new Type[] { typeof(bool)})]
    internal class SXLStickInput_OnStickPressed_Patch
    {
        private static void Prefix()
        {
            if ((!SXLRuntime.enabled ? 0 : (SXLMod.Customization.SXLSettings.realisticMode ? 1 : 0)) == 0)
                return;

            Debug.Log("StickInput.OnStickPressed");
            float boardZ = PlayerController.Instance.boardController.boardRigidbody.transform.localEulerAngles.z;
            float skaterZ = PlayerController.Instance.skaterController.skaterRigidbody.transform.localEulerAngles.z;
            double num2 = skaterZ;

            if (Mathf.Abs(Mathf.DeltaAngle(boardZ, skaterZ)) <= 100.0f)
                return;

            PlayerController.Instance.ForceBail();
            PlayerController.Instance.CrossFadeAnimation("Fall", 2.1f);
        }
    }


    #endregion

    #region Trick Name Fix
    [HarmonyPatch(typeof(Grind.GrindSegment), MethodType.Constructor, new Type[] { typeof(GrindType), typeof(bool), typeof(float) })]
    internal class SXLGrind_GrindSegment_Patch
    {
        static void Postfix(ref Grind.GrindSegment __instance)
        {
            Debug.Log("I'm in the constructor");
            __instance.isSwitch = PlayerController.Instance.IsSwitch;
        }
    }

    [HarmonyPatch(typeof(Grind), nameof(Grind.ToString))]
    internal class SXLTrickDetection_ToString_Patch
    {
        private static readonly Dictionary<GrindType, GrindType> m_switchTrickDictionary = new Dictionary<GrindType, GrindType>()
        {
            {GrindType.FsNoseSlide, GrindType.FsTailSlide}, {GrindType.BsNoseSlide, GrindType.BsTailSlide},
            {GrindType.FsTailSlide, GrindType.FsNoseSlide}, {GrindType.BsTailSlide, GrindType.BsNoseSlide},
            {GrindType.FsNoseBluntSlide, GrindType.FsBluntSlide}, {GrindType.BsNoseBluntSlide, GrindType.BsBluntSlide},
            {GrindType.FsBluntSlide, GrindType.FsNoseBluntSlide}, {GrindType.BsBluntSlide, GrindType.BsNoseBluntSlide},
            {GrindType.FsCrook, GrindType.FsSalad}, {GrindType.BsCrook, GrindType.BsSalad},
            {GrindType.FsOverCrook, GrindType.FsSuski}, {GrindType.BsOverCrook, GrindType.BsSuski},
            {GrindType.FsSalad, GrindType.FsCrook}, {GrindType.BsSalad, GrindType.BsCrook},
            {GrindType.FsSuski, GrindType.FsOverCrook}, {GrindType.BsSuski, GrindType.BsOverCrook},
            {GrindType.FsFeeble, GrindType.FsLosi}, {GrindType.BsFeeble, GrindType.BsLosi},
            {GrindType.FsWilly, GrindType.FsFeeble}, {GrindType.BsWilly, GrindType.BsFeeble},
            {GrindType.FsSmith, GrindType.FsWilly}, {GrindType.BsSmith, GrindType.BsWilly},
            {GrindType.FsLosi, GrindType.FsSmith}, {GrindType.BsLosi, GrindType.BsSmith},
            {GrindType.FsNoseGrind, GrindType.FsFiveO}, {GrindType.BsNoseGrind, GrindType.BsFiveO},
            {GrindType.FsFiveO, GrindType.FsNoseGrind}, {GrindType.BsFiveO, GrindType.BsNoseGrind}
        };

        private static string GetGrindName(GrindType grindType, bool isSwitch)
        {
            if (isSwitch)
            {
                GrindType switchType;

                if (m_switchTrickDictionary.TryGetValue(grindType, out switchType))
                    grindType = switchType;

            }

            return Grind.GetGrindName(grindType);
        }
        
        static bool Prefix(Grind __instance, ref string __result)
        {
            IEnumerable<Grind.GrindSegment> source = __instance.grindSegments.Where<Grind.GrindSegment>((Func<Grind.GrindSegment, bool>)(gt => gt.duration > Mathf.Min(0.4f, 0.5f * __instance.duration)));
            __result = string.Join(" to ", source.Count<Grind.GrindSegment>() == 0 ? __instance.grindSegments.Select<Grind.GrindSegment, string>((Func<Grind.GrindSegment, string>)(gt => GetGrindName(gt.grindType, gt.isSwitch))) : source.Select<Grind.GrindSegment, string>((Func<Grind.GrindSegment, string>)(gt => GetGrindName(gt.grindType, gt.isSwitch))));
            return false;
        }
    }
    #endregion
}
