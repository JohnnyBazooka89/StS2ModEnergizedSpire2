using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Random;

namespace EnergizedSpire2.EnergizedSpire2Code.Relics;

[Pool(typeof(EventRelicPool))]
public class NeverEndingSparkler : EnergizedSpire2Relic
{
    private const string ChancePercentKey = "ChancePercent";

    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new(ChancePercentKey, 50M),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this)
    ];

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != Owner ? amount : amount + DynamicVars.Energy.IntValue;
    }

    public override Task AfterObtained()
    {
        ChangeQuestionMarkRoomsIntoEliteRooms();
        return Task.CompletedTask;
    }

    public override Task AfterActEntered()
    {
        ChangeQuestionMarkRoomsIntoEliteRooms();
        return Task.CompletedTask;
    }

    private void ChangeQuestionMarkRoomsIntoEliteRooms()
    {
        ActMap map = Owner.RunState.Map;
        Rng rng = new Rng(Owner.RunState.Rng.Seed, 100 * Owner.RunState.ActFloor);
        List<MapPoint> questionRooms = map.GetAllMapPoints().Where(p => p.PointType == MapPointType.Unknown)
            .ToList();

        foreach (MapPoint questionRoom in questionRooms)
        {
            if (rng.NextInt(100) < DynamicVars[ChancePercentKey].BaseValue)
            {
                questionRoom.PointType = MapPointType.Elite;
            }
        }

        NMapScreen.Instance?.RefreshAllPointVisuals();
    }
}