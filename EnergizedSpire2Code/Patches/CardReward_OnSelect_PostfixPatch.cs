using EnergizedSpire2.EnergizedSpire2Code.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace EnergizedSpire2.EnergizedSpire2Code.Patches;

[HarmonyPatch(typeof(CardReward), "OnSelect")]
public static class CardReward_OnSelect_PostfixPatch
{
    static async Task<bool> Postfix(Task<bool> __result, CardReward __instance)
    {
        bool originalResult = await __result;
        if (originalResult)
        {
            await GrantExtraRewards(__instance);
        }

        return originalResult;
    }

    private static async Task GrantExtraRewards(CardReward cardReward)
    {
        if (cardReward == null)
            return;

        var cards = Traverse.Create(cardReward)
            .Field("_cards")
            .GetValue<List<CardCreationResult>>();

        if (cards == null || cards.Count == 0)
            return;


        Player? player = LocalContext.GetMe(cards[0].Card.RunState);
        StickyHand? stickyHand = player?.GetRelic<StickyHand>();
        if (stickyHand == null)
        {
            return;
        }

        var extras = cards.ToList();

        foreach (var extra in extras)
        {
            CardPileAddResult addResult = await CardPileCmd.Add(extra.Card, PileType.Deck);
            if (!addResult.success)
                continue;

            CardModel addedCard = addResult.cardAdded;

            cards.RemoveAll(c => c.Card == addedCard);

            MainFile.Logger.Info($"Obtained {addedCard.Id} from card reward (extra card from Sticky Hand)");
            RunManager.Instance.RewardSynchronizer.SyncLocalObtainedCard(addedCard);
        }
    }
}