using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace EnergizedSpire2.EnergizedSpire2Code.Relics;

[Pool(typeof(EventRelicPool))]
public class CursedPentagram : EnergizedSpire2Relic
{
    private int _cardsAdded;
    private bool _isActivating;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new CardsVar(5)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];

    public override bool ShowCounter => true;

    public override int DisplayAmount
    {
        get => !IsActivating ? CardsAddedSinceLastTrigger : DynamicVars.Cards.IntValue;
    }

    [SavedProperty]
    private int CardsAdded
    {
        get => _cardsAdded;
        set
        {
            _cardsAdded = value;
            InvokeDisplayAmountChanged();
        }
    }

    private int CardsAddedSinceLastTrigger => CardsAdded % DynamicVars.Cards.IntValue;

    private bool IsActivating
    {
        get => _isActivating;
        set
        {
            _isActivating = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != Owner ? amount : amount + DynamicVars.Energy.IntValue;
    }

    public override async Task AfterCardChangedPiles(
        CardModel card,
        PileType oldPileType,
        AbstractModel? source)
    {
        if (Owner.Creature.IsDead || card.Owner != Owner)
            return;
        CardPile? pile = card.Pile;
        if (pile is not { Type: PileType.Deck } ||
            card.Rarity == CardRarity.Curse ||
            card.Type == CardType.Curse
           )
        {
            return;
        }

        CardsAdded++;
        if (CardsAddedSinceLastTrigger != 0)
        {
            return;
        }

        _ = TaskHelper.RunSafely(DoActivateVisuals());

        HashSet<CardModel> availableCurses = ModelDb.CardPool<CurseCardPool>()
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.CanBeGeneratedByModifiers).ToHashSet();

        List<CardPileAddResult> curseAddResult = new List<CardPileAddResult>();
        CardModel? curseToAdd = Owner.RunState.Rng.Niche.NextItem(availableCurses);

        if (curseToAdd == null)
        {
            return;
        }

        curseAddResult.Add(await CardPileCmd.Add(Owner.RunState.CreateCard(curseToAdd, Owner),
            PileType.Deck));
        CardCmd.PreviewCardPileAdd(curseAddResult, 2f);
        await Cmd.Wait(0.75f);
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }
}