using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace EnergizedSpire2.EnergizedSpire2Code.Relics;

[Pool(typeof(EventRelicPool))]
public class MagnifyingGlass : EnergizedSpire2Relic
{
    private const string MoreHpPercentKey = "MoreHpPercent";
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new(MoreHpPercentKey, 15M),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != Owner ? amount : amount + DynamicVars.Energy.IntValue;
    }

    public override async Task AfterCreatureAddedToCombat(Creature creature)
    {
        if (creature.Side == Owner.Creature.Side)
            return;
        Flash();
        await IncreaseMaxHpForCreature(creature);
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return;
        IEnumerable<Creature> targets = Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature)
            .Where(c => c.IsAlive);
        Flash();
        foreach (Creature creature in targets)
        {
            await IncreaseMaxHpForCreature(creature);
        }
    }

    private Task IncreaseMaxHpForCreature(Creature creature)
    {
        return CreatureCmd.SetMaxAndCurrentHp(creature,
            creature.MaxHp * (1 + DynamicVars[MoreHpPercentKey].BaseValue / 100M));
    }
}