using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Relics;

namespace EnergizedSpire2.EnergizedSpire2Code.Utils;

public class RelicUtils
{
    public static void ReloadAllIcons()
    {
        foreach (NRelicInventoryHolder nRelicInventoryHolder in NRun.Instance?.GlobalUi.RelicInventory.RelicNodes)
        {
            var relic = nRelicInventoryHolder.Relic;
            Traverse.Create(relic).Method("Reload").GetValue();
        }
    }
}