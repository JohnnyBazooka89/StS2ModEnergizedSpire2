using EnergizedSpire2.EnergizedSpire2Code.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace EnergizedSpire2.EnergizedSpire2Code.Powers;

public class ReceptionProblemsPower : EnergizedSpire2TemporaryPower<ReceptionProblems, FocusPower>
{
    protected override Func<PlayerChoiceContext, Creature, decimal, Creature?, CardModel?, bool, Task> ApplyPowerFunc
        => (playerChoiceContext, creature, amount, applier, cardSource, _)
            => PowerCmd.Apply<FocusPower>(playerChoiceContext, creature,
                amount, applier, cardSource);

    public override PowerType Type => PowerType.Debuff;

    protected override bool InvertInternalPowerAmount => true;

    public override LocString Description => new("powers", "TEMPORARY_FOCUS_DOWN.description");

    protected override string SmartDescriptionLocKey => "TEMPORARY_FOCUS_DOWN.smartDescription";
}