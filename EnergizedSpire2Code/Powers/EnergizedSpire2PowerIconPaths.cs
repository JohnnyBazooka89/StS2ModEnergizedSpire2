using BaseLib.Extensions;
using EnergizedSpire2.EnergizedSpire2Code.Extensions;
using Godot;

namespace EnergizedSpire2.EnergizedSpire2Code.Powers;

public static class EnergizedSpire2PowerIconPaths
{
    public static string PowerImagePath(string entry)
    {
        var path = $"{entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
        return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
    }

    public static string BigPowerImagePath(string entry)
    {
        var path = $"{entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
        return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
    }
}