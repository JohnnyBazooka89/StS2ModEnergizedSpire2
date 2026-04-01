using System.Reflection;
using BaseLib;
using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace EnergizedSpire2.EnergizedSpire2Code.Patches;

[HarmonyPatch]
public static class RunHistoryIconOutlinePath
{
    static MethodBase TargetMethod()
    {
        return AccessTools.PropertyGetter(typeof(AncientEventModel), "RunHistoryIconOutlinePath");
    }

    static bool Prefix(AncientEventModel __instance, ref string __result)
    {
        if (__instance is ICustomModel customModel)
        {
            switch (customModel)
            {
                case CustomAncientModel ancient:
                    BaseLibMain.Logger.Info("Using custom ancient run history icon outline path");
                    __result = ancient.CustomRunHistoryIconOutlinePath;
                    return __result == null;
            }
        }

        return true;
    }
}