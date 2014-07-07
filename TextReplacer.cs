public static class TextReplacer
{
    private static ConfigNode cn;
    private static string filename = "GameData/KSPScienceLibrary/TextReplacer.cfg";
    private static bool readOnce = false; //You don't want to read the config 1000 times per second. Use TRUE for release!

    public static string GetReplaceForString(string input)
    {
        if (cn == null || !readOnce)
            cn = ConfigNode.Load(filename) ?? new ConfigNode();
        if (cn.HasValue(input)) return cn.GetValue(input);
        cn.AddValue(input, input); // just in case there is something missing in the file.
        cn.Save(filename);
        return input;
    }
}