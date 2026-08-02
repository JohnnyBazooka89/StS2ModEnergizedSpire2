using BaseLib.Utils;
using EnergizedSpire2.EnergizedSpire2Code.Extensions;
using EnergizedSpire2.EnergizedSpire2Code.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace EnergizedSpire2.EnergizedSpire2Code.Relics;

[Pool(typeof(EventRelicPool))]
public class EnergizedCoin : EnergizedSpire2Relic
{
    private readonly LocString _ectoplasmCurrentEffectLoc =
        LocString.GetIfExists(_locTable, $"{ModelDb.GetId<EnergizedCoin>().Entry}.Ectoplasm.currentEffect")!;

    private readonly LocString _ectoplasmDescriptionLoc =
        LocString.GetIfExists(_locTable, $"{ModelDb.GetId<EnergizedCoin>().Entry}.Ectoplasm.description")!;

    private readonly LocString _ectoplasmTitleLoc =
        LocString.GetIfExists(_locTable, $"{ModelDb.GetId<EnergizedCoin>().Entry}.Ectoplasm.title")!;

    private readonly LocString _sozuCurrentEffectLoc =
        LocString.GetIfExists(_locTable, $"{ModelDb.GetId<EnergizedCoin>().Entry}.Sozu.currentEffect")!;

    private readonly LocString _sozuDescriptionLoc =
        LocString.GetIfExists(_locTable, $"{ModelDb.GetId<EnergizedCoin>().Entry}.Sozu.description")!;

    private readonly LocString _sozuTitleLoc =
        LocString.GetIfExists(_locTable, $"{ModelDb.GetId<EnergizedCoin>().Entry}.Sozu.title")!;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override string PackedIconPath
    {
        get
        {
            String path = $"{GetIconFileName()}.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "relic.png".RelicImagePath();
        }
    }

    public override string BigIconPath
    {
        get
        {
            String path = $"{GetIconFileName()}.png".BigRelicImagePath();
            return ResourceLoader.Exists(path) ? path : "relic.png".BigRelicImagePath();
        }
    }
    
    private string GetIconFileName()
    {
        return CurrentEffect switch
        {
            EnergizedCoinEffect.Ectoplasm => "energized_coin_ectoplasm",
            _ => "energized_coin_sozu"
        };
    }

    private EnergizedCoinEffect? CurrentEffect { get; set; }

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            List<IHoverTip> hoverTips =
            [
                HoverTipFactory.ForEnergy(this),
            ];

            switch (CurrentEffect)
            {
                case EnergizedCoinEffect.Sozu:
                    hoverTips.Add(new HoverTip(_sozuCurrentEffectLoc, _sozuDescriptionLoc));
                    break;
                case EnergizedCoinEffect.Ectoplasm:
                    hoverTips.Add(new HoverTip(_ectoplasmCurrentEffectLoc, _ectoplasmDescriptionLoc));
                    break;
                default:
                    hoverTips.Add(new HoverTip(_sozuTitleLoc, _sozuDescriptionLoc));
                    hoverTips.Add(new HoverTip(_ectoplasmTitleLoc, _ectoplasmDescriptionLoc));
                    break;
            }

            return hoverTips;
        }
    }

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != Owner ? amount : amount + DynamicVars.Energy.IntValue;
    }

    public override bool ShouldProcurePotion(PotionModel potion, Player player)
    {
        return player != Owner || CurrentEffect != EnergizedCoinEffect.Sozu;
    }

    public override Decimal ModifyGoldGained(Player player, Decimal amount)
    {
        return player != Owner || CurrentEffect != EnergizedCoinEffect.Ectoplasm ? amount : 0M;
    }

    public override Task AfterModifyingGoldGained(Player player, Decimal amount)
    {
        Flash();
        return Task.CompletedTask;
    }
    
    public override Task AfterRoomEntered(AbstractRoom room)
    {
        EnergizedCoinEffect[] values = Enum.GetValues<EnergizedCoinEffect>();
        EnergizedCoinEffect randomEffect = values[Owner.RunState.Rng.Niche.NextInt(0, values.Length)];
        CurrentEffect = randomEffect;
        RelicIconChanged();
        RelicUtils.ReloadRelicIcon(Id);
        Flash();
        return Task.CompletedTask;
    }

    public override Task AfterObtained()
    {
        CurrentEffect = Enum.GetValues<EnergizedCoinEffect>().First();
        return Task.CompletedTask;
    }

    private enum EnergizedCoinEffect
    {
        Sozu,
        Ectoplasm
    }
}