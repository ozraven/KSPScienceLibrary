using System.Collections.Generic;
using UnityEngine;

internal class KSPScienceSettings
{
    public static KSPScienceSettings settings = new KSPScienceSettings();
    private readonly Dictionary<string, bool> settingsBools;
    private readonly Dictionary<string, Rect> settingsRects;
    private readonly Dictionary<string, GUIStyle> settingsStyles;
    private readonly GUIStyle windowStyle;


    private ConfigNode cn;
    private string filename = "GameData/KSPScienceLibrary/KSPScienceLibrarySettings.cfg";
    public bool shown = false;


    private Rect windowPosition;

    private KSPScienceSettings()
    {
        //MonoBehaviour.print("Create Science Settings");
        windowStyle = new GUIStyle(HighLogic.Skin.window);
        windowStyle.stretchHeight = true;
        windowStyle.stretchWidth = true;
        settingsStyles = new Dictionary<string, GUIStyle>();
        settingsBools = new Dictionary<string, bool>();
        settingsRects = new Dictionary<string, Rect>();


        LoadSettings();
    }

    public static GUIStyle getStyleSetting(string settingID)
    {
//         if (null == settings)
//             settings = new KSPScienceSettings();
        return settings.settingsStyles[settingID];
    }

    public static bool getBoolSetting(string settingID)
    {
//         if (null == settings)
//             settings = new KSPScienceSettings();
        return settings.settingsBools[settingID];
    }

    public static Rect getRectSetting(string settingID)
    {
//         if (null == settings)
//             settings = new KSPScienceSettings();
        //MonoBehaviour.print("getRectSetting " + settingID + " " + settings.settingsRects[settingID]);
        return settings.settingsRects[settingID];
    }

    public static void setRectSetting(string settingID, Rect setting)
    {
        //MonoBehaviour.print("setRectSetting " + settingID + " " + setting);
//         if (null == settings)
//             settings = new KSPScienceSettings();
        settings._setRectSetting(settingID, setting);
    }

    private void _setRectSetting(string settingID, Rect setting)
    {
        //MonoBehaviour.print("_setRectSetting " + settingID + " " + setting);

        if (settingsRects.ContainsKey(settingID))
            settingsRects[settingID] = setting;
        else
            settingsRects.Add(settingID, setting);
    }

    private void SaveSettings()
    {
        //MonoBehaviour.print("Saving Science Library settings...");
        //MonoBehaviour.print("styles: " + settingsStyles.Count + " bools: " + settingsBools.Count + " rects: " + settingsRects.Count);

        cn.ClearData();

        foreach (KeyValuePair<string, GUIStyle> settingsStyle in settingsStyles)
            cn.AddValue(settingsStyle.Key, ConfigNode.WriteColor(settingsStyle.Value.normal.textColor));

        foreach (KeyValuePair<string, bool> settingsBool in settingsBools)
            cn.AddValue(settingsBool.Key, settingsBool.Value.ToString());

        foreach (KeyValuePair<string, Rect> settingsRect in settingsRects)
        {
            Vector4 tmpVector4 = new Vector4(settingsRect.Value.yMin, settingsRect.Value.width, settingsRect.Value.height, settingsRect.Value.xMin);
            cn.AddValue(settingsRect.Key, ConfigNode.WriteVector(tmpVector4));
        }
        cn.Save(filename);
    }

    private void LoadSettings()
    {
        settingsStyles.Clear();
        settingsBools.Clear();
        settingsRects.Clear();

        GUIStyle style;

        style = new GUIStyle();
        style.normal.textColor = Color.white;
        settingsStyles.Add("LibraryDoneExperiments", style);
        style = new GUIStyle();
        style.normal.textColor = Color.red;
        settingsStyles.Add("LibraryNewExperiments", style);
        style = new GUIStyle();
        style.normal.textColor = Color.red;
        settingsStyles.Add("MonitorKSCExperiments", style);
        style = new GUIStyle();
        style.normal.textColor = Color.green;
        settingsStyles.Add("MonitorNewExperiments", style);
        style = new GUIStyle();
        style.normal.textColor = Color.yellow;
        settingsStyles.Add("MonitorOnShipExperiments", style);

        settingsBools.Add("ShowDeployButton", false);
        settingsBools.Add("ShowOnlyKnownBiomes", false);
        settingsBools.Add("ShowOnlyKnownExperiments", false);


        cn = ConfigNode.Load(filename) ?? new ConfigNode();

        ReadTextColorFromConfig("LibraryDoneExperiments", settingsStyles["LibraryDoneExperiments"]);
        ReadTextColorFromConfig("LibraryNewExperiments", settingsStyles["LibraryNewExperiments"]);
        ReadTextColorFromConfig("MonitorKSCExperiments", settingsStyles["MonitorKSCExperiments"]);
        ReadTextColorFromConfig("MonitorNewExperiments", settingsStyles["MonitorNewExperiments"]);
        ReadTextColorFromConfig("MonitorOnShipExperiments", settingsStyles["MonitorOnShipExperiments"]);

        settingsBools["ShowDeployButton"] = ReadBoolFromConfig("ShowDeployButton", settingsBools["ShowDeployButton"]);
        settingsBools["ShowOnlyKnownBiomes"] = ReadBoolFromConfig("ShowOnlyKnownBiomes", settingsBools["ShowOnlyKnownBiomes"]);
        settingsBools["ShowOnlyKnownExperiments"] = ReadBoolFromConfig("ShowOnlyKnownExperiments", settingsBools["ShowOnlyKnownExperiments"]);

        _setRectSetting("LibraryRect", ReadRectFromConfig("LibraryRect", new Rect(5, 20, Screen.width - 10, Screen.height - 80)));
        _setRectSetting("MonitorRect", ReadRectFromConfig("MonitorRect", new Rect((Screen.width - 600)/2, (Screen.height - 200)/2, 600, 200)));
        _setRectSetting("SettingsRect", ReadRectFromConfig("SettingsRect", new Rect((Screen.width - 400)/2, (Screen.height - 400)/2, 400, 400)));

        //MonoBehaviour.print("after load settings. styles: " + settingsStyles.Count + " bools: " + settingsBools.Count + " rects: " + settingsRects.Count);
    }

    private bool ReadTextColorFromConfig(string id, GUIStyle style)
    {
        if (!cn.HasValue(id)) return false;
        style.normal.textColor = ConfigNode.ParseColor(cn.GetValue(id));
        return true;
    }

    private Rect ReadRectFromConfig(string id, Rect default_value)
    {
        if (!cn.HasValue(id)) return default_value;
        Vector4 tmpVector4 = ConfigNode.ParseVector4(cn.GetValue(id));
        return new Rect(tmpVector4.w, tmpVector4.x, tmpVector4.y, tmpVector4.z);
    }

    private bool ReadBoolFromConfig(string id, bool default_value)
    {
        if (!cn.HasValue(id)) return default_value;
        return bool.Parse(cn.GetValue(id));
    }

    public static void Show()
    {
//         if (null == settings)
//             settings = new KSPScienceSettings();
        settings.shown = true;
        settings.windowPosition = getRectSetting("SettingsRect");
        RenderingManager.AddToPostDrawQueue(3, settings.OnDraw);
    }

    public static void Hide()
    {
//         if (null == settings)
//             settings = new KSPScienceSettings();
        settings.shown = false;
        settings.SaveSettings();
        RenderingManager.RemoveFromPostDrawQueue(3, settings.OnDraw);
    }

    public static void Toggle()
    {
        //if (null == settings || !settings.shown)
        if (!settings.shown)
            Show();
        else
            Hide();
    }

    ~KSPScienceSettings()
    {
        OnDestroy();
    }

    public void OnDestroy()
    {
        //MonoBehaviour.print("Destroy Science Settings");
        SaveSettings();

        RenderingManager.RemoveFromPostDrawQueue(3, OnDraw);
    }

    private void OnDraw()
    {
        //MonoBehaviour.print("Settings_OnDraw");
        if (!settings.shown) return;
        windowPosition = GUI.Window(5, windowPosition, OnWindow, "Science Settings", windowStyle);
        setRectSetting("SettingsRect", windowPosition);
    }

    private void OnWindow(int id)
    {
        //cn.ClearData();
        //MonoBehaviour.print("Settings_OnWindow");
        GUILayout.BeginVertical();
        GUILayout.Label("Monitor:");
        AddHorizontalColorSlider("MonitorKSCExperiments", "Monitor KSC Experiments:");
        AddHorizontalColorSlider("MonitorNewExperiments", "Monitor New Experiments:");
        AddHorizontalColorSlider("MonitorOnShipExperiments", "Monitor OnShip Experiments:");
        AddToggle("ShowDeployButton", "Show \"Deploy\" Button:");
        GUILayout.Label("Library:");
        AddHorizontalColorSlider("LibraryDoneExperiments", "Library Done Experiments:");
        AddHorizontalColorSlider("LibraryNewExperiments", "Library New Experiments:");
        AddToggle("ShowOnlyKnownBiomes", "Show only known biomes:");
        AddToggle("ShowOnlyKnownExperiments", "Show only known experimentts:");
/*
        if (GUILayout.Button("Save to File"))
            settings.SaveSettings();
        if (GUILayout.Button("Reload from File"))
            LoadSettings();
*/
        GUILayout.EndVertical();

        if (GUI.Button(new Rect(windowPosition.width - 21, 0, 21, 21), "X"))
            Hide();
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }

    private void AddToggle(string id, string name)
    {
        GUILayout.BeginHorizontal();
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;

        GUILayout.Label(name, new[] {GUILayout.MaxWidth(210), GUILayout.MinWidth(210)});

        settingsBools[id] = GUILayout.Toggle(settingsBools[id], settingsBools[id].ToString());

        GUILayout.EndHorizontal();

        //cn.SetValue(id, settingsBools[id].ToString());
    }

    private void AddHorizontalColorSlider(string id, string name)
    {
        GUILayout.BeginHorizontal();
        GUIStyle style = settingsStyles[id];

        GUILayout.Label(name, new[] {GUILayout.MaxWidth(210), GUILayout.MinWidth(210)});
        float r = style.normal.textColor.r*255;
        float g = style.normal.textColor.g*255;
        float b = style.normal.textColor.b*255;
        r = GUILayout.HorizontalSlider(r, 0, 255);
        g = GUILayout.HorizontalSlider(g, 0, 255);
        b = GUILayout.HorizontalSlider(b, 0, 255);
        style.normal.textColor = new Color(r/255, g/255, b/255);
        GUILayout.Box("text", style);
        GUILayout.EndHorizontal();

        //cn.SetValue(id, ConfigNode.WriteColor(style.normal.textColor));
    }
}