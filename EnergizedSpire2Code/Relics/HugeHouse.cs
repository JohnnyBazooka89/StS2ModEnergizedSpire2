using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace EnergizedSpire2.EnergizedSpire2Code.Relics;

[Pool(typeof(EventRelicPool))]
public class HugeHouse : EnergizedSpire2Relic
{
    private const string PotionsToLoseKey = "PotionsToLose";
    private const string StrikesToAddKey = "StrikesToAdd";
    private const string CardsToDowngradeKey = "CardsToDowngrade";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new(PotionsToLoseKey, 1M),
        new GoldVar(50),
        new HpLossVar(5M),
        new(StrikesToAddKey, 1),
        new(CardsToDowngradeKey, 1)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            CardModel characterStrike = getCharacterStrike();
            return
            [
                HoverTipFactory.ForEnergy(this),
                HoverTipFactory.FromCard(characterStrike)
            ];
        }
    }

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != Owner ? amount : amount + DynamicVars.Energy.IntValue;
    }

    public override async Task AfterObtained()
    {
        await LoseRandomPotion();

        await PlayerCmd.LoseGold(DynamicVars.Gold.IntValue, Owner);

        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars.HpLoss.BaseValue,
            false);

        await AddCharacterStrike();

        await DowngradeCard();
    }

    private async Task LoseRandomPotion()
    {
        List<PotionModel> potions = Owner.Potions.ToList();
        if (!potions.Any())
        {
            return;
        }

        PotionModel potion = Owner.RunState.Rng.Niche.NextItem(potions);
        await PotionCmd.Discard(potion);
    }

    private async Task AddCharacterStrike()
    {
        CardModel characterStrike = getCharacterStrike();
        List<CardPileAddResult> addCardResult = new List<CardPileAddResult>();
        addCardResult.Add(await CardPileCmd.Add(Owner.RunState.CreateCard(characterStrike, Owner), PileType.Deck));
        CardCmd.PreviewCardPileAdd(addCardResult, 2f);
        await Cmd.Wait(2.25f);
    }

    private async Task DowngradeCard()
    {
        List<CardModel> upgradedCards = Owner.Deck.Cards.Where(c => c.IsUpgraded).ToList();
        if (!upgradedCards.Any())
        {
            return;
        }

        CardModel card = Owner.RunState.Rng.Niche.NextItem(upgradedCards);
        upgradedCards.Remove(card);
        CardCmd.Downgrade(card);
        CardCmd.Preview(card, style: CardPreviewStyle.MessyLayout);
        await Cmd.Wait(0.75f);
    }

    private CardModel getCharacterStrike()
    {
        CardModel strike = IsMutable
            ? Owner?.Character.CardPool.AllCards.Where(c =>
                c.Rarity == CardRarity.Basic && c.Tags.Contains(CardTag.Strike)).FirstOrDefault()
            : null;

        return strike ?? ModelDb.Card<StrikeIronclad>();
    }
}