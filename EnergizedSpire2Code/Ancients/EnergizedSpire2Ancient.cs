using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using EnergizedSpire2.EnergizedSpire2Code.Relics;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;

namespace EnergizedSpire2.EnergizedSpire2Code.Ancients;

[Pool(typeof(AncientEventModel))]
public class EnergizedSpire2Ancient : CustomAncientModel
{
    public override string CustomScenePath => "res://EnergizedSpire2/images/ancients/darv.tscn";
    public override string CustomMapIconPath => "res://EnergizedSpire2/images/card_portraits/big/card.png";
    public override string CustomMapIconOutlinePath => "res://EnergizedSpire2/images/card_portraits/big/card.png";
    public override string CustomRunHistoryIconPath => "res://EnergizedSpire2/images/card_portraits/big/card.png";

    public override string CustomRunHistoryIconOutlinePath => CustomRunHistoryIconPath;

    protected override OptionPools MakeOptionPools
    {
        get
        {
            List<AncientOption> listOfAncientOptions = [];

            // If Owner is null -> return ALL options
            if (Owner == null)
            {
                AddIroncladOptions(listOfAncientOptions);
                AddSilentOptions(listOfAncientOptions);
                AddDefectOptions(listOfAncientOptions);
                AddRegentOptions(listOfAncientOptions);
                AddNecrobinderOptions(listOfAncientOptions, null);

                AddGlobalOptions(listOfAncientOptions, null, null);

                return new OptionPools(MakePool(listOfAncientOptions.ToArray()));
            }

            var characterId = Owner.Character.Id;
            bool isSinglePlayer = Owner.RunState.CardMultiplayerConstraint ==
                                  CardMultiplayerConstraint.SingleplayerOnly;
            int actNumber = Owner.RunState.Act.ActNumber();

            if (characterId == ModelDb.GetId<Ironclad>())
            {
                AddIroncladOptions(listOfAncientOptions);
            }

            if (characterId == ModelDb.GetId<Silent>())
            {
                AddSilentOptions(listOfAncientOptions);
            }

            if (characterId == ModelDb.GetId<Defect>())
            {
                AddDefectOptions(listOfAncientOptions);
            }

            if (characterId == ModelDb.GetId<Regent>())
            {
                AddRegentOptions(listOfAncientOptions);
            }

            if (characterId == ModelDb.GetId<Necrobinder>())
            {
                AddNecrobinderOptions(listOfAncientOptions, isSinglePlayer);
            }

            AddGlobalOptions(listOfAncientOptions, isSinglePlayer, actNumber);

            EnergizedSpire2MainFile.Logger.Warn("characterId: " + characterId);
            EnergizedSpire2MainFile.Logger.Warn("There are " + listOfAncientOptions.Count + " options");

            return new OptionPools(MakePool(listOfAncientOptions.ToArray()));
        }
    }

    public OptionPools GetDynamicOptionPools()
    {
        return MakeOptionPools;
    }

    private void AddIroncladOptions(List<AncientOption> options)
    {
        options.Add(AncientOption<RottingSkull>());
        options.Add(AncientOption<TabascoSauce>());
    }

    private void AddSilentOptions(List<AncientOption> options)
    {
        options.Add(AncientOption<HighHeels>());
        options.Add(AncientOption<PogoStick>());
    }

    private void AddDefectOptions(List<AncientOption> options)
    {
        options.Add(AncientOption<OldTV>());
        options.Add(AncientOption<AggressionProtocol>());
    }

    private void AddRegentOptions(List<AncientOption> options)
    {
        options.Add(AncientOption<FadingConstellation>());
        options.Add(AncientOption<RoyalCoffers>());
    }

    private void AddNecrobinderOptions(List<AncientOption> options, bool? isSinglePlayer)
    {
        options.Add(AncientOption<NecroticScythe>());
        if (isSinglePlayer is null or true)
        {
            options.Add(AncientOption<BrassCoil>());
        }
    }

    private void AddGlobalOptions(List<AncientOption> options, bool? isSinglePlayer, int? actNumber)
    {
        if (actNumber is null or 2)
        {
            options.Add(AncientOption<CursedPentagram>());
        }

        options.Add(AncientOption<DeadBattery>());
        options.Add(AncientOption<OgreHead>());
        options.Add(AncientOption<SpiderWeb>());
        options.Add(AncientOption<StickyHand>());

        if (isSinglePlayer is null or true)
        {
            options.Add(AncientOption<MagnifyingGlass>());
            options.Add(AncientOption<NeverEndingSparkler>());
            options.Add(AncientOption<RedRose>());
        }
    }
}