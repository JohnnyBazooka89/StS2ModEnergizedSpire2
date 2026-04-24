using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace EnergizedSpire2.EnergizedSpire2Code.Relics;

[Pool(typeof(EventRelicPool))]
public class AggressionProtocol : EnergizedSpire2Relic
{
    private const string EnergyCostIncreaseKey = "EnergyCostIncrease";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    private bool UsedThisTurn { get; set; }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new EnergyVar(EnergyCostIncreaseKey, 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != Owner ? amount : amount + DynamicVars.Energy.IntValue;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner ||
            (cardPlay.Card.Type != CardType.Skill && cardPlay.Card.Type != CardType.Power) ||
            UsedThisTurn)
        {
            return Task.CompletedTask;
        }

        UsedThisTurn = true;
        return Task.CompletedTask;
    }

    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        Decimal originalCost,
        out Decimal modifiedCost)
    {
        IEnumerable<CardModel> attacks = CardPile.GetCards(Owner, PileType.Hand).Where(c => c.Type == CardType.Attack);

        if (card.Owner != Owner ||
            (card.Type != CardType.Skill && card.Type != CardType.Power) ||
            !attacks.Any() ||
            UsedThisTurn)
        {
            modifiedCost = originalCost;
            return false;
        }

        modifiedCost = originalCost + DynamicVars[EnergyCostIncreaseKey].IntValue;
        return true;
    }

    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        ICombatState combatState)
    {
        if (side != Owner.Creature.Side)
            return Task.CompletedTask;
        UsedThisTurn = false;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        UsedThisTurn = false;
        return Task.CompletedTask;
    }
}