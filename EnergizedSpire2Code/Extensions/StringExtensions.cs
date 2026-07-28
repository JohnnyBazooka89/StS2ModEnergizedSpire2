namespace EnergizedSpire2.EnergizedSpire2Code.Extensions;

//Mostly utilities to get asset paths.
public static class StringExtensions
{
    public static string ImagePath(this string path)
    {
        return Path.Join(EnergizedSpire2MainFile.ResPath, "images", path);
    }

    public static string CardImagePath(this string path)
    {
        return Path.Join(EnergizedSpire2MainFile.ResPath, "images", "card_portraits", path);
    }

    public static string BetaCardImagePath(this string path)
    {
        return Path.Join(EnergizedSpire2MainFile.ResPath, "images", "card_portraits", "beta", path);
    }
    
    public static string BigCardImagePath(this string path)
    {
        return Path.Join(EnergizedSpire2MainFile.ResPath, "images", "card_portraits", "big", path);
    }

    public static string PowerImagePath(this string path)
    {
        return Path.Join(EnergizedSpire2MainFile.ResPath, "images", "powers", path);
    }

    public static string BigPowerImagePath(this string path)
    {
        return Path.Join(EnergizedSpire2MainFile.ResPath, "images", "powers", "big", path);
    }

    public static string RelicImagePath(this string path)
    {
        return Path.Join(EnergizedSpire2MainFile.ResPath, "images", "relics", path);
    }

    public static string RelicOutlineImagePath(this string path)
    {
        return Path.Join(EnergizedSpire2MainFile.ResPath, "images", "relics", "outline", path);
    }

    public static string BigRelicImagePath(this string path)
    {
        return Path.Join(EnergizedSpire2MainFile.ResPath, "images", "relics", "big", path);
    }

    public static string AncientImagePath(this string path)
    {
        return Path.Join(EnergizedSpire2MainFile.ResPath, "images", "ancients", path);
    }
}