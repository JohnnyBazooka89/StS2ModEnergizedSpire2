using BaseLib.Abstracts;
using BaseLib.Utils;
using EnergizedSpire2.EnergizedSpire2Code.Ancients;
using HarmonyLib;

namespace EnergizedSpire2.EnergizedSpire2Code.Patches;

[HarmonyPatch(typeof(CustomAncientModel), nameof(CustomAncientModel.OptionPools), MethodType.Getter)]
public static class CustomAncientModel_OptionPools_PrefixPatch
{
    static bool Prefix(CustomAncientModel __instance, ref OptionPools __result)
    {
        if (__instance is EnergizedSpire2Ancient myModel)
        {
            // Compute dynamically every time, no caching
            __result = myModel.GetDynamicOptionPools();
            return false; // skip original getter
        }

        return true; // run original getter for all other classes
    }
}