using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Relics;

namespace EnergizedSpire2.EnergizedSpire2Code.Utils;

public class RelicUtils
{
    public static void ReloadRelicIcon(ModelId relicId)
    {
        foreach (NRelicInventoryHolder nRelicInventoryHolder in NRun.Instance?.GlobalUi.RelicInventory.RelicNodes)
        {
            var relic = nRelicInventoryHolder.Relic;
            if (relic.Model.Id == relicId)
            {
                relic.Reload();
            }
        }
    }
}