using BaseLib.Utils;
using EnergizedSpire2.EnergizedSpire2Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace EnergizedSpire2.EnergizedSpire2Code.Cards;

[Pool(typeof(CurseCardPool))]
public class ReceptionProblems() : EnergizedSpire2Card(-1, CardType.Curse, CardRarity.Curse, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new PowerVar<FocusPower>(2M)
    ];

    public override int MaxUpgradeLevel => 0;
    public override bool CanBeGeneratedByModifiers => false;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Unplayable,
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FocusPower>()
    ];

    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw)
    {
        if (card != this || Pile.Type != PileType.Hand)
            return;

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<ReceptionProblemsPower>(Owner.Creature,
            DynamicVars["FocusPower"].BaseValue, Owner.Creature, this);
    }
}