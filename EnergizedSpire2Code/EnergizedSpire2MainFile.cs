using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace EnergizedSpire2.EnergizedSpire2Code;

[ModInitializer(nameof(Initialize))]
public partial class EnergizedSpire2MainFile : Node
{
    public const string ModId = "EnergizedSpire2"; //Used for resource filepath

    public static Logger Logger { get; } = new(ModId, LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        harmony.PatchAll();
    }
}