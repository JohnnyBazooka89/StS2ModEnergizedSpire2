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
    private const string _hpThresholdKey = "HpThreshold";
    private bool _strengthApplied;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(_hpThresholdKey, 50M),
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
        RottingSkull rottingSkull = this;
        Creature creature = rottingSkull.Owner.Creature;
        bool flag = creature.CurrentHp >
                    creature.MaxHp * (rottingSkull.DynamicVars[_hpThresholdKey].BaseValue / 100M);
        rottingSkull.Status = flag ? RelicStatus.Normal : RelicStatus.Active;
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        RottingSkull rottingSkull = this;
        if (side != rottingSkull.Owner.Creature.Side || rottingSkull.Status != RelicStatus.Active)
            return;
        rottingSkull.Flash();
        await PlayerCmd.GainEnergy(rottingSkull.DynamicVars.Energy.BaseValue, rottingSkull.Owner);
    }

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        return player != Owner || Status != RelicStatus.Active ? count : count + DynamicVars.Cards.BaseValue;
    }
}