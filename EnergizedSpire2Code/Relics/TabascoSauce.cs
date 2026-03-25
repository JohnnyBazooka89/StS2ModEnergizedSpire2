using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace EnergizedSpire2.EnergizedSpire2Code.Relics;

[Pool(typeof(EventRelicPool))]
public class TabascoSauce : EnergizedSpire2Relic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != Owner ? amount : amount + DynamicVars.Energy.IntValue;
    }

    // Rest Site calls ModifyHealAmount twice for some reason. We need to multiply by 2 to counteract this.
    public override decimal ModifyRestSiteHealAmount(Creature creature, decimal amount)
    {
        return amount * 2;
    }

    public override decimal ModifyHealAmount(Creature creature, decimal amount)
    {
        bool shouldTrigger = creature.Player == Owner;
        if (shouldTrigger)
        {
            Flash();
        }

        return shouldTrigger
            ? amount * 0.5M
            : amount;
    }
}