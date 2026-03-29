using System.Collections.Generic;
using System.Threading.Tasks;
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
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace EnergizedSpire2.EnergizedSpire2Code.Relics;

[Pool(typeof(EventRelicPool))]
public class HighHeels : EnergizedSpire2Relic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new PowerVar<DexterityPower>(1M),
        new CardsVar(1),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this),
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.FromCard<Clumsy>(),
    ];

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != Owner ? amount : amount + DynamicVars.Energy.IntValue;
    }

    public override Task AfterCreatureAddedToCombat(Creature creature)
    {
        if (creature.Side == Owner.Creature.Side)
            return Task.CompletedTask;
        Flash();
        return PowerCmd.Apply<StrengthPower>(creature, -DynamicVars["ThornsPower"].BaseValue, null, null);
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
        {
            return;
        }

        Flash();
        await PowerCmd.Apply<DexterityPower>(Owner.Creature, -DynamicVars.Dexterity.BaseValue,
            Owner.Creature, null);
    }

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        CombatState combatState)
    {
        if (player != Owner || combatState.RoundNumber != 1)
        {
            return;
        }

        Flash();
        List<CardModel> cards = new List<CardModel>();
        for (int index = 0; index < DynamicVars.Cards.IntValue; index++)
        {
            cards.Add(combatState.CreateCard<Clumsy>(Owner));
        }

        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(cards,
            PileType.Draw, true, CardPilePosition.Random));
        await Cmd.Wait(3f);
    }
}