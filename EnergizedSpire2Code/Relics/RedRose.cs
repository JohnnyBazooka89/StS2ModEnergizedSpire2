using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace EnergizedSpire2.EnergizedSpire2Code.Relics;

[Pool(typeof(EventRelicPool))]
public class RedRose : EnergizedSpire2Relic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ThornsPower>(1M),
        new EnergyVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this),
        HoverTipFactory.FromPower<ThornsPower>()
    ];

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != Owner ? amount : amount + DynamicVars.Energy.IntValue;
    }

    public override Task AfterCreatureAddedToCombat(Creature creature)
    {
        if (creature.Side == Owner.Creature.Side)
            return Task.CompletedTask;
        Flash();
        return PowerCmd.Apply<StrengthPower>(creature, DynamicVars["ThornsPower"].BaseValue, null, null);
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        RedRose redRode = this;
        if (room is not CombatRoom)
            return;
        IEnumerable<Creature> targets = redRode.Owner.Creature.CombatState.GetOpponentsOf(redRode.Owner.Creature)
            .Where(c => c.IsAlive);
        redRode.Flash();
        await PowerCmd.Apply<StrengthPower>(targets, redRode.DynamicVars["ThornsPower"].BaseValue, null, null);
    }
}