using BaseLib.Hooks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace EnergizedSpire2.EnergizedSpire2Code.Relics;

[Pool(typeof(EventRelicPool))]
public class TabascoSauce : EnergizedSpire2Relic, IHealAmountModifier
{
    private const string HpReductionPercentKey = "HpReductionPercent";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new(HpReductionPercentKey, 50M),
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];

    public Decimal ModifyHealMultiplicative(Creature creature, Decimal amount)
    {
        if (creature.Player != Owner)
        {
            return 1;
        }

        Flash();
        return 1 - DynamicVars[HpReductionPercentKey].BaseValue / 100M;
    }

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != Owner ? amount : amount + DynamicVars.Energy.IntValue;
    }
}