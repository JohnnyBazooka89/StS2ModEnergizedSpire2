using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
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
public class RottingSkull : EnergizedSpire2Relic
{
    private const string HpThresholdKey = "HpThreshold";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(HpThresholdKey, 50M),
        new EnergyVar(1),
        new CardsVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return;
        await SetActiveIfNecessary();
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override async Task AfterCurrentHpChanged(Creature creature, decimal _)
    {
        if (!CombatManager.Instance.IsInProgress)
            return;
        await SetActiveIfNecessary();
    }

    private async Task SetActiveIfNecessary()
    {
        Creature creature = Owner.Creature;
        bool flag = creature.CurrentHp >
                    creature.MaxHp * (DynamicVars[HpThresholdKey].BaseValue / 100M);
        Status = flag ? RelicStatus.Normal : RelicStatus.Active;
    }

    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side != Owner.Creature.Side || Status != RelicStatus.Active)
            return;
        Flash();
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        return player != Owner || Status != RelicStatus.Active ? count : count + DynamicVars.Cards.BaseValue;
    }
}