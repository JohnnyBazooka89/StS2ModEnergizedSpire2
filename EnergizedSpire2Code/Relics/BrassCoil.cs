using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace EnergizedSpire2.EnergizedSpire2Code.Relics;

[Pool(typeof(EventRelicPool))]
public class BrassCoil : EnergizedSpire2Relic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new PowerVar<ArtifactPower>(1M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this),
        HoverTipFactory.FromPower<ArtifactPower>()
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
        return PowerCmd.Apply<ArtifactPower>(new ThrowingPlayerChoiceContext(), creature, DynamicVars["ArtifactPower"].BaseValue, null, null);
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return;
        IEnumerable<Creature> targets = Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature)
            .Where(c => c.IsAlive);
        Flash();
        decimal factor = targets.Count() == 1 ? 2M : 1M;
        await PowerCmd.Apply<ArtifactPower>(new ThrowingPlayerChoiceContext(), targets, DynamicVars["ArtifactPower"].BaseValue * factor, null, null);
    }
}