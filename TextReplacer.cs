public static class TextReplacer
{
    private static ConfigNode cn;
    private static string filename = "GameData/KSPScienceLibrary/TextReplacer.cfg";
    private static bool readOnce = false;

    public static string GetReplaceForString(string input)
    {
        if (cn == null || !readOnce)
            cn = ConfigNode.Load(filename) ?? new ConfigNode();
        return cn.HasValue(input) ? cn.GetValue(input) : input;
    }
}