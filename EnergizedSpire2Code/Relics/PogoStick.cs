using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace EnergizedSpire2.EnergizedSpire2Code.Relics;

[Pool(typeof(EventRelicPool))]
public class PogoStick : EnergizedSpire2Relic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new CardsVar(1),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this),
        HoverTipFactory.FromKeyword(CardKeyword.Sly),
        HoverTipFactory.FromCard<Dazed>()
    ];

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != Owner ? amount : amount + DynamicVars.Energy.IntValue;
    }

    public override async Task BeforeCardAutoPlayed(CardModel card, Creature? target, AutoPlayType type)
    {
        if (type != AutoPlayType.SlyDiscard || card.Owner != Owner ||
            Owner.Creature.Side != Owner.Creature.CombatState.CurrentSide)
            return;
        Flash();

        List<CardModel> cards = new List<CardModel>();
        for (int index = 0; index < DynamicVars.Cards.IntValue; index++)
        {
            cards.Add(Owner.Creature.CombatState.CreateCard<Dazed>(Owner));
        }

        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(cards,
            PileType.Draw, true, CardPilePosition.Random));
    }
}