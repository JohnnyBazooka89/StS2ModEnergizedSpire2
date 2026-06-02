using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace EnergizedSpire2.EnergizedSpire2Code.Relics;

[Pool(typeof(EventRelicPool))]
public class OgreHead : EnergizedSpire2Relic
{
    private const string MisdirectionPercentKey = "MisdirectionPercent";
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new(MisdirectionPercentKey, 50M),
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != Owner ? amount : amount + DynamicVars.Energy.IntValue;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        IEnumerable<Creature> targets = Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature)
            .Where(c => c.IsAlive);
        if (cardPlay.Card.Owner != Owner || cardPlay.Target == null || targets.Count() < 2)
        {
            return Task.CompletedTask;
        }

        int random = Owner.RunState.Rng.CombatTargets.NextInt(0, 100);
        if (random >= DynamicVars[MisdirectionPercentKey].BaseValue)
        {
            return Task.CompletedTask;
        }

        Flash();
        List<Creature> otherTargets = targets.Where(t => t != cardPlay.Target).ToList();
        Owner.RunState.Rng.CombatTargets.Shuffle(otherTargets);

        Traverse.Create(cardPlay)
            .Property("Target")
            .SetValue(otherTargets[0]);

        return Task.CompletedTask;
    }
}