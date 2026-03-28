#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;

#endregion

namespace EnergizedSpire2.EnergizedSpire2Code.Relics;

[Pool(typeof(EventRelicPool))]
public class RoyalCoffers : EnergizedSpire2Relic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new GoldVar(300)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        RoyalCoffers royalCoffers = this;
        if (side != royalCoffers.Owner.Creature.Side ||
            royalCoffers.Owner.Gold < royalCoffers.DynamicVars.Gold.BaseValue)
        {
            return;
        }

        royalCoffers.Flash();
        await PlayerCmd.GainEnergy(royalCoffers.DynamicVars.Energy.BaseValue, royalCoffers.Owner);
    }
}