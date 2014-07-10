using System;
using System.Collections.Generic;
using UnityEngine;

internal class KSPScienceSettings
{
    public static KSPScienceSettings settings = new KSPScienceSettings();
    private readonly ComboBox comboBoxControl = new ComboBox();
    private readonly string[] comboBoxList;
    private readonly GUIStyle listStyle = new GUIStyle();
    private readonly Dictionary<string, bool> settingsBools;
    private readonly Dictionary<string, Color> settingsColors;
    private readonly Dictionary<string, int> settingsIntegers;
    private readonly Dictionary<string, Rect> settingsRects;
    private readonly Dictionary<string, string> settingsStrings;
    private readonly Dictionary<string, GUIStyle> settingsStylesTextColor;
    private readonly GUIStyle windowStyle;
    private static GUISkin OrbitMapSkin = AssetBase.GetGUISkin("OrbitMapSkin");

    private ConfigNode cn;
    private string filename = "GameData/KSPScienceLibrary/KSPScienceLibrarySettings.cfg";
    public bool shown = false;
    private Texture2D windowBgColorTexture2D;


    private Rect windowPosition;

    private KSPScienceSettings()
    {
        //MonoBehaviour.print("Create Science Settings");
        windowStyle = new GUIStyle(HighLogic.Skin.window);
        windowStyle.stretchHeight = true;
        windowStyle.stretchWidth = true;
        settingsStylesTextColor = new Dictionary<string, GUIStyle>();
        settingsBools = new Dictionary<string, bool>();
        settingsRects = new Dictionary<string, Rect>();
        settingsIntegers = new Dictionary<string, int>();
        settingsColors = new Dictionary<string, Color>();
        settingsStrings = new Dictionary<string, string>();
        // ComboBox Start
        /*
        KSP window 1
        KSP window 2
        KSP window 3
        KSP window 4
        KSP window 5
        KSP window 6
        KSP window 7
        OrbitMapSkin
        PlaqueDialogSkin
        ExperimentsDialogSkin
        ExpRecoveryDialogSkin
         */
        comboBoxList = new string[9];
        comboBoxList[0] = ("KSP window 1");
        comboBoxList[1] = ("KSP window 2");
        comboBoxList[2] = ("KSP window 3");
        comboBoxList[3] = ("KSP window 4");
        comboBoxList[4] = ("KSP window 5");
        comboBoxList[5] = ("KSP window 6");
        comboBoxList[6] = ("KSP window 7");
        comboBoxList[7] = ("OrbitMapSkin");
        comboBoxList[8] = ("PlaqueDialogSkin");
        //comboBoxList[9] = new GUIContent("ExperimentsDialogSkin");
        //comboBoxList[10] = new GUIContent("ExpRecoveryDialogSkin");

        listStyle.normal.textColor = Color.white;
        Texture2D tex = new Texture2D(2, 2);
        //tex.SetPixels(0, 0, 2, 2, new[] { Color.black, Color.black, Color.black, Color.black });
        listStyle.onHover.background = listStyle.hover.background = tex;
        listStyle.padding.left = listStyle.padding.right = listStyle.padding.top = listStyle.padding.bottom = 4;
        // ComboBox End

        LoadSettings();
    }

    public static string getStringSetting(string settingID)
    {
        return settings.settingsStrings[settingID];
    }

    public static Color getColorSetting(string settingID)
    {
        return settings.settingsColors[settingID];
    }

    public static GUIStyle getStyleSetting(string settingID)
    {
        return settings.settingsStylesTextColor[settingID];
    }

    public static bool getBoolSetting(string settingID)
    {
        return settings.settingsBools[settingID];
    }

    public static Rect getRectSetting(string settingID)
    {
        return settings.settingsRects[settingID];
    }

    public static void setRectSetting(string settingID, Rect setting)
    {
        settings._setRectSetting(settingID, setting);
    }

    private void _setRectSetting(string settingID, Rect setting)
    {
        if (settingsRects.ContainsKey(settingID))
            settingsRects[settingID] = setting;
        else
            settingsRects.Add(settingID, setting);
    }

    private void SaveSettings()
    {
        //MonoBehaviour.print("Saving Science Library settings...");
        //MonoBehaviour.print("styles: " + settingsStylesTextColor.Count + " bools: " + settingsBools.Count + " rects: " + settingsRects.Count);

        cn.ClearData();

        foreach (KeyValuePair<string, GUIStyle> settingsStyle in settingsStylesTextColor)
            cn.AddValue(settingsStyle.Key, ConfigNode.WriteColor(settingsStyle.Value.normal.textColor));

        foreach (KeyValuePair<string, bool> settingsBool in settingsBools)
            cn.AddValue(settingsBool.Key, settingsBool.Value.ToString());

        foreach (KeyValuePair<string, int> settingsInteger in settingsIntegers)
            cn.AddValue(settingsInteger.Key, settingsInteger.Value.ToString());

        foreach (KeyValuePair<string, Rect> settingsRect in settingsRects)
        {
            Vector4 tmpVector4 = new Vector4(settingsRect.Value.xMin, settingsRect.Value.yMin, settingsRect.Value.width, settingsRect.Value.height);
            cn.AddValue(settingsRect.Key, ConfigNode.WriteVector(tmpVector4));
        }

        foreach (KeyValuePair<string, Color> settingsColor in settingsColors)
            cn.AddValue(settingsColor.Key, ConfigNode.WriteColor(settingsColor.Value));

        foreach (KeyValuePair<string, string> settingsString in settingsStrings)
            cn.AddValue(settingsString.Key, settingsString.Value);

        cn.Save(filename);
    }

    private void LoadSettings()
    {
        settingsStylesTextColor.Clear();
        settingsBools.Clear();
        settingsRects.Clear();
        settingsColors.Clear();

        GUIStyle style;

        style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.wordWrap = false;
        settingsStylesTextColor.Add("LibraryDoneExperiments", style);
        style = new GUIStyle();
        style.normal.textColor = Color.red;
        style.wordWrap = false;
        settingsStylesTextColor.Add("LibraryNewExperiments", style);
        style = new GUIStyle();
        style.normal.textColor = Color.red;
        style.wordWrap = false;
        settingsStylesTextColor.Add("MonitorKSCExperiments", style);
        style = new GUIStyle();
        style.normal.textColor = Color.green;
        style.wordWrap = false;
        settingsStylesTextColor.Add("MonitorNewExperiments", style);
        style = new GUIStyle();
        style.normal.textColor = Color.yellow;
        style.wordWrap = false;
        settingsStylesTextColor.Add("MonitorOnShipExperiments", style);

        settingsBools.Add("ShowDeployButton", false);
        settingsBools.Add("ShowOnlyKnownBiomes", false);
        settingsBools.Add("ShowOnlyKnownExperiments", false);

        //settingsIntegers.Add("Skin", 0);

        settingsColors.Add("WindowBGColor", new Color());

        settingsStrings.Add("Skin", "OrbitMapSkin");

        cn = ConfigNode.Load(filename) ?? new ConfigNode();

        ReadTextColorFromConfig("LibraryDoneExperiments", settingsStylesTextColor["LibraryDoneExperiments"]);
        ReadTextColorFromConfig("LibraryNewExperiments", settingsStylesTextColor["LibraryNewExperiments"]);
        ReadTextColorFromConfig("MonitorKSCExperiments", settingsStylesTextColor["MonitorKSCExperiments"]);
        ReadTextColorFromConfig("MonitorNewExperiments", settingsStylesTextColor["MonitorNewExperiments"]);
        ReadTextColorFromConfig("MonitorOnShipExperiments", settingsStylesTextColor["MonitorOnShipExperiments"]);

        settingsBools["ShowDeployButton"] = ReadBoolFromConfig("ShowDeployButton", settingsBools["ShowDeployButton"]);
        settingsBools["ShowOnlyKnownBiomes"] = ReadBoolFromConfig("ShowOnlyKnownBiomes", settingsBools["ShowOnlyKnownBiomes"]);
        settingsBools["ShowOnlyKnownExperiments"] = ReadBoolFromConfig("ShowOnlyKnownExperiments", settingsBools["ShowOnlyKnownExperiments"]);

        //settingsIntegers["Skin"] = ReadIntegerFromConfig("Skin", settingsIntegers["Skin"]);
        settingsStrings["Skin"] = ReadStringFromConfig("Skin", settingsStrings["Skin"]);

        settingsColors["WindowBGColor"] = ReadColorFromConfig("WindowBGColor", settingsColors["WindowBGColor"]);

        _setRectSetting("LibraryRect", ReadRectFromConfig("LibraryRect", new Rect(5, 20, Screen.width - 10, Screen.height - 80)));
        _setRectSetting("MonitorRect", ReadRectFromConfig("MonitorRect", new Rect((Screen.width - 600)/2, (Screen.height - 200)/2, 600, 200)));
        //_setRectSetting("SettingsRect", ReadRectFromConfig("SettingsRect", new Rect((Screen.width - 400)/2, (Screen.height - 440)/2, 400, 440)));


        //MonoBehaviour.print("after load settings. styles: " + settingsStylesTextColor.Count + " bools: " + settingsBools.Count + " rects: " + settingsRects.Count);
    }

    public static Texture2D GetBGTexture(bool force = false)
    {
        if (settings.windowBgColorTexture2D != null && !force) return settings.windowBgColorTexture2D;
        settings.windowBgColorTexture2D = new Texture2D(1, 1);
        Color color = getColorSetting("WindowBGColor");
        settings.windowBgColorTexture2D.SetPixel(0, 0, color);
        settings.windowBgColorTexture2D.Apply();
        return settings.windowBgColorTexture2D;
    }

    private Color ReadColorFromConfig(string id, Color defaultColor)
    {
        if (!cn.HasValue(id)) return defaultColor;
        return ConfigNode.ParseColor(cn.GetValue(id));
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
        return new Rect(tmpVector4.x, tmpVector4.y, tmpVector4.z, tmpVector4.w);
    }

    private bool ReadBoolFromConfig(string id, bool default_value)
    {
        if (!cn.HasValue(id)) return default_value;
        return bool.Parse(cn.GetValue(id));
    }

    private string ReadStringFromConfig(string id, string default_value)
    {
        if (!cn.HasValue(id)) return default_value;
        return cn.GetValue(id);
    }

    private int ReadIntegerFromConfig(string id, int default_value)
    {
        if (!cn.HasValue(id)) return default_value;
        return int.Parse(cn.GetValue(id));
    }

    public static void Show()
    {
//         if (null == settings)
//             settings = new KSPScienceSettings();
        settings.shown = true;
        settings.windowPosition = new Rect((Screen.width - 400)/2, (Screen.height - 460)/2, 400, 460); 
        //getRectSetting("SettingsRect");
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
        windowPosition = GUI.Window(5, windowPosition, OnWindow, TextReplacer.GetReplaceForString("Science Settings"), windowStyle);
        //setRectSetting("SettingsRect", windowPosition);
    }

    private void OnWindow(int id)
    {
        GUI.skin = AssetBase.GetGUISkin("KSP window 1");
        //cn.ClearData();
        //MonoBehaviour.print("Settings_OnWindow");
        GUILayout.BeginVertical();
        GUILayout.Label(TextReplacer.GetReplaceForString("Monitor:"));
        AddHorizontalColorSlider("MonitorKSCExperiments", TextReplacer.GetReplaceForString("Monitor KSC Experiments:"));
        AddHorizontalColorSlider("MonitorNewExperiments", TextReplacer.GetReplaceForString("Monitor New Experiments:"));
        AddHorizontalColorSlider("MonitorOnShipExperiments", TextReplacer.GetReplaceForString("Monitor OnShip Experiments:"));
        AddToggle("ShowDeployButton", TextReplacer.GetReplaceForString("Show \"Deploy\" Button:"));
        GUILayout.Label(TextReplacer.GetReplaceForString("Library:"));
        AddHorizontalColorSlider("LibraryDoneExperiments", TextReplacer.GetReplaceForString("Library Done Experiments:"));
        AddHorizontalColorSlider("LibraryNewExperiments", TextReplacer.GetReplaceForString("Library New Experiments:"));
        AddToggle("ShowOnlyKnownBiomes", TextReplacer.GetReplaceForString("Show only known biomes:"));
        AddToggle("ShowOnlyKnownExperiments", TextReplacer.GetReplaceForString("Show only known experimentts:"));
        GUILayout.Label(TextReplacer.GetReplaceForString("Skin:"));

        int selectedItemIndex = comboBoxControl.GetSelectedItemIndex();


        selectedItemIndex = comboBoxControl.List(comboBoxList[selectedItemIndex], comboBoxList, listStyle, Array.IndexOf(comboBoxList, settings.settingsStrings["Skin"]));
        settings.settingsStrings["Skin"] = comboBoxList[selectedItemIndex];

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
        GUIStyle style = settingsStylesTextColor[id];

        GUILayout.Label(name, new[] {GUILayout.MaxWidth(210), GUILayout.MinWidth(210)});
        float r = style.normal.textColor.r*255;
        float g = style.normal.textColor.g*255;
        float b = style.normal.textColor.b*255;
        r = GUILayout.HorizontalSlider(r, 0, 255);
        g = GUILayout.HorizontalSlider(g, 0, 255);
        b = GUILayout.HorizontalSlider(b, 0, 255);
        style.normal.textColor = new Color(r/255, g/255, b/255);
        GUILayout.Box(TextReplacer.GetReplaceForString("text"), style);
        GUILayout.EndHorizontal();

        //cn.SetValue(id, ConfigNode.WriteColor(style.normal.textColor));
    }

    public static GUISkin getSkin()
    {
        int sel;
        sel = Array.IndexOf(settings.comboBoxList, settings.settingsStrings["Skin"]);
        if (sel < 0) sel = 0;
        return AssetBase.GetGUISkin(settings.comboBoxList[sel]);
    }

    public static void ChangeSkin(GUISkin skin)
    {
        skin.label.margin = OrbitMapSkin.label.margin;
        skin.label.padding = OrbitMapSkin.label.padding;

        skin.button.margin = OrbitMapSkin.button.margin;
        skin.button.padding = OrbitMapSkin.button.padding;

        skin.toggle.margin = OrbitMapSkin.toggle.margin;
        skin.toggle.padding = OrbitMapSkin.toggle.padding;

        skin.window.margin = OrbitMapSkin.window.margin;
        skin.window.padding = OrbitMapSkin.window.padding;

        skin.scrollView.margin = OrbitMapSkin.scrollView.margin;
        skin.scrollView.padding = OrbitMapSkin.scrollView.padding;
    }
}