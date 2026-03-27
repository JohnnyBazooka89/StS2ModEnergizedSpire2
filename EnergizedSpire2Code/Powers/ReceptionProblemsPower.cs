using EnergizedSpire2.EnergizedSpire2Code.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace EnergizedSpire2.EnergizedSpire2Code.Powers;

public class ReceptionProblemsPower : EnergizedSpire2Power
{
    private bool _shouldIgnoreNextInstance;

    public override PowerType Type => !IsPositive ? PowerType.Debuff : PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private AbstractModel OriginModel => ModelDb.Card<ReceptionProblems>();

    private bool IsPositive => false;

    private int Sign => !IsPositive ? -1 : 1;

    public override LocString Title
    {
        get
        {
            switch (OriginModel)
            {
                case CardModel cardModel:
                    return cardModel.TitleLocString;
                case PotionModel potionModel:
                    return potionModel.Title;
                case RelicModel relicModel:
                    return relicModel.Title;
                default:
                    throw new InvalidOperationException();
            }
        }
    }

    public override LocString Description =>
        new("powers",
            IsPositive ? "TEMPORARY_FOCUS_POWER.description" : "TEMPORARY_FOCUS_DOWN.description");

    protected override string SmartDescriptionLocKey =>
        !IsPositive
            ? "TEMPORARY_FOCUS_DOWN.smartDescription"
            : "TEMPORARY_FOCUS_POWER.smartDescription";

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            List<IHoverTip> items = new List<IHoverTip>();
            List<IHoverTip> hoverTipList = items;
            IEnumerable<IHoverTip> collection;
            switch (OriginModel)
            {
                case CardModel card:
                    collection =
                    [
                        HoverTipFactory.FromCard(card)
                    ];
                    break;
                case PotionModel model:
                    collection =
                    [
                        HoverTipFactory.FromPotion(model)
                    ];
                    break;
                case RelicModel relic:
                    collection = HoverTipFactory.FromRelic(relic);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            hoverTipList.AddRange(collection);
            items.Add(HoverTipFactory.FromPower<FocusPower>());
            return items;
        }
    }

    public override async Task BeforeApplied(
        Creature target,
        Decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (_shouldIgnoreNextInstance)
        {
            _shouldIgnoreNextInstance = false;
        }
        else
        {
            await PowerCmd.Apply<FocusPower>(target, Sign * amount, applier, cardSource, true);
        }
    }

    public override async Task AfterPowerAmountChanged(
        PowerModel power,
        Decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        ReceptionProblemsPower receptionProblemsPower = this;
        if (amount == receptionProblemsPower.Amount || power != receptionProblemsPower)
            return;
        if (receptionProblemsPower._shouldIgnoreNextInstance)
        {
            receptionProblemsPower._shouldIgnoreNextInstance = false;
        }
        else
        {
            await PowerCmd.Apply<FocusPower>(receptionProblemsPower.Owner,
                receptionProblemsPower.Sign * amount, applier, cardSource, true);
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        ReceptionProblemsPower power = this;
        if (side != power.Owner.Side)
            return;
        power.Flash();
        await PowerCmd.Remove(power);
        await PowerCmd.Apply<FocusPower>(power.Owner, (-power.Sign * power.Amount),
            power.Owner, null);
    }
}