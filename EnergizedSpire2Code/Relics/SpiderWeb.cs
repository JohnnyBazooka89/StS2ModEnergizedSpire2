using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace EnergizedSpire2.EnergizedSpire2Code.Relics;

[Pool(typeof(EventRelicPool))]
public class SpiderWeb : EnergizedSpire2Relic
{
    private const string EnergyCostIncreaseKey = "EnergyCostIncrease";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new EnergyVar(EnergyCostIncreaseKey, 1)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this),
        ..HoverTipFactory.FromAffliction<Entangled>()
    ];

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != Owner ? amount : amount + DynamicVars.Energy.IntValue;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner)
        {
            return;
        }

        List<CardModel> attacks = PileType.Hand.GetPile(Owner).Cards.Where(c => c is { Type: CardType.Attack, Affliction: null }).ToList();
        if (attacks.Count == 0)
        {
            return;
        }

        Flash();
        Owner.RunState.Rng.Shuffle.Shuffle(attacks);
        attacks[0].EnergyCost.AddThisTurn(DynamicVars[EnergyCostIncreaseKey].IntValue);
        await CardCmd.Afflict<Entangled>(attacks[0], 1M);
    }
    
    
    public override Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner.Creature))
            return Task.CompletedTask;

        foreach (CardModel card in Owner.PlayerCombatState?.AllCards.Where(c => c.Affliction is Entangled) ?? [])
        {
            CardCmd.ClearAffliction(card);
        }

        return Task.CompletedTask;
    }
}