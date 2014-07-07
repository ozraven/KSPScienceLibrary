using System;
using System.Collections.Generic;
using UnityEngine;

[KSPAddon(KSPAddon.Startup.Flight, false)]
public class KSPScienceMonitor : MonoBehaviour
{
    public static KSPScienceMonitorButton toolbarButton;
    private static Rect windowPosition;
    private static GUIStyle windowStyle;

    public static bool drawWindow = false;

    // List of science on this ship
    private readonly List<ExperimentView> OnShip = new List<ExperimentView>();
    // List of science to show on window
    private readonly List<ExperimentView> Output = new List<ExperimentView>();

    //private readonly Dictionary<string, bool[]> idsList = new Dictionary<string, bool[]>();
    // whether it should pause game at new science
    // private bool autoPauseOnNew;


    // "last" to compare with "now". For optimizations
    private string lastBiome;
    private ExperimentSituations lastExperimentSituation;
    private CelestialBody lastMainBody;

    // Optimizations
    private float lateUpdateTimer;
    private int lateUpdateTimerCounter;

    private bool resizingWindow;
    private Vector2 scrollVector2;

    public void Awake()
    {
        RenderingManager.AddToPostDrawQueue(2, OnDraw);
    }

    public void Start()
    {
        windowStyle = new GUIStyle(HighLogic.Skin.window);
        windowStyle.stretchHeight = true;
        windowStyle.stretchWidth = true;

        windowPosition = KSPScienceSettings.getRectSetting("MonitorRect");
    }

    public void Update()
    {
        if (Input.GetMouseButtonUp(0))
            resizingWindow = false;
        if (resizingWindow)
        {
            windowPosition.width = Input.mousePosition.x - windowPosition.x + 10;
            windowPosition.height = (Screen.height - Input.mousePosition.y) - windowPosition.y + 10;
        }
        // using "while" for possibility to "break". So we dont have to use GOTO. using timer to skip frames.
        while (drawWindow && (lateUpdateTimer < Time.time || lateUpdateTimerCounter > 3))
        {
            lateUpdateTimer = Time.time + 1;

            ExperimentSituations experimentSituation = ScienceUtil.GetExperimentSituation(FlightGlobals.ActiveVessel);
            CelestialBody mainbody = FlightGlobals.ActiveVessel.mainBody;
            string biome;
            if (FlightGlobals.ActiveVessel.landedAt != string.Empty)
                biome = FlightGlobals.ActiveVessel.landedAt;
            else
                biome = ScienceUtil.GetExperimentBiome(FlightGlobals.ActiveVessel.mainBody, FlightGlobals.ActiveVessel.latitude, FlightGlobals.ActiveVessel.longitude);

            if (mainbody == lastMainBody && biome == lastBiome && lastExperimentSituation == experimentSituation && lateUpdateTimerCounter <= 3)
            {
                lateUpdateTimer = 0;
                lateUpdateTimerCounter++;
                break;
            }
            lateUpdateTimerCounter = 0;
            lastBiome = biome;
            lastMainBody = mainbody;
            lastExperimentSituation = experimentSituation;

            Output.Clear();
            OnShip.Clear();


            //List<ScienceExperiment> PossibleExperiments = new List<ScienceExperiment>();

            //Search for all Science Experiment Modules on vessel, Check experiment available and add in "Output" and "OnShip"
            foreach (ModuleScienceExperiment moduleScienceExperiment in FlightGlobals.ActiveVessel.FindPartModulesImplementing<ModuleScienceExperiment>())
            {
                string firstExperimentId = moduleScienceExperiment.experimentID;
                ScienceExperiment scienceExperiment = ResearchAndDevelopment.GetExperiment(firstExperimentId);
                bool available = scienceExperiment.IsAvailableWhile(experimentSituation, mainbody);
                if (available)
                {
                    if (moduleScienceExperiment.Deployed)
                    {
                        foreach (ScienceData scienceData in moduleScienceExperiment.GetData())
                        {
                            ExperimentView experimentView = new ExperimentView(scienceData);
                            if (!Output.Contains(experimentView))
                                Output.Add(experimentView);
                            OnShip.Add(experimentView);
                        }
                    }
                    {
                        ScienceSubject scienceSubject = LibraryUtils.GetExperimentSubject(scienceExperiment, experimentSituation, mainbody, scienceExperiment.BiomeIsRelevantWhile(experimentSituation) ? biome : "");
                        ExperimentView experimentView = new ExperimentView(scienceSubject);
                        if (!Output.Contains(experimentView))
                            Output.Add(experimentView);
                    }
                }
            }

            // Check for Kerbals on board. That means we can also use "evaReport", "surfaceSample", "asteroidSample" in EVA.
            if (FlightGlobals.ActiveVessel.GetCrewCount() > 0)
            {
                {
                    string firstExperimentId = "evaReport";
                    ScienceExperiment scienceExperiment = ResearchAndDevelopment.GetExperiment(firstExperimentId);
                    bool available = scienceExperiment.IsAvailableWhile(experimentSituation, mainbody);
                    if (available)
                    {
                        ScienceSubject scienceSubject = LibraryUtils.GetExperimentSubject(scienceExperiment, experimentSituation, mainbody, scienceExperiment.BiomeIsRelevantWhile(experimentSituation) ? biome : "");
                        ExperimentView experimentView = new ExperimentView(scienceSubject);
                        if (!Output.Contains(experimentView))
                            Output.Add(experimentView);
                    }
                }
                {
                    string firstExperimentId = "surfaceSample";
                    ScienceExperiment scienceExperiment = ResearchAndDevelopment.GetExperiment(firstExperimentId);
                    bool available = scienceExperiment.IsAvailableWhile(experimentSituation, mainbody);
                    if (available)
                    {
                        ScienceSubject scienceSubject = LibraryUtils.GetExperimentSubject(scienceExperiment, experimentSituation, mainbody, scienceExperiment.BiomeIsRelevantWhile(experimentSituation) ? biome : "");
                        ExperimentView experimentView = new ExperimentView(scienceSubject);
                        if (!Output.Contains(experimentView))
                            Output.Add(experimentView);
                    }
                }

                // Find asteroid that could be used for science. (I hope it will not be too many asteroids, because of linear complexity of this code)
                ModuleAsteroid[] asteroids = FindObjectsOfType<ModuleAsteroid>();
                foreach (ModuleAsteroid asteroid in asteroids)
                {
                    Vector3 destination3 = asteroid.gameObject.transform.position - FlightGlobals.ActiveVessel.gameObject.transform.position;
                    float unfocusedRange = asteroid.Events["TakeSampleEVAEvent"].unfocusedRange;
                    unfocusedRange *= unfocusedRange;
                    if (destination3.sqrMagnitude < unfocusedRange)
                    {
                        string firstExperimentId = "asteroidSample";
                        ScienceExperiment scienceExperiment = ResearchAndDevelopment.GetExperiment(firstExperimentId);
                        bool available = scienceExperiment.IsAvailableWhile(experimentSituation, mainbody);
                        if (available)
                        {
                            string asteroidname = asteroid.part.partInfo.name + asteroid.part.flightID;
                            ScienceSubject scienceSubject = LibraryUtils.GetExperimentSubject(scienceExperiment, experimentSituation, asteroidname, "", mainbody, scienceExperiment.BiomeIsRelevantWhile(experimentSituation) ? biome : "");
                            ExperimentView experimentView = new ExperimentView(scienceSubject);
                            if (!Output.Contains(experimentView))
                                Output.Add(experimentView);
                        }
                    }
                }
            }
            //Search for all Science Containers on vessel and write all found experiments into OnShip list
            foreach (ModuleScienceContainer moduleScienceContainer in FlightGlobals.ActiveVessel.FindPartModulesImplementing<ModuleScienceContainer>())
            {
                foreach (ScienceData scienceData in moduleScienceContainer.GetData())
                {
                    ExperimentView experimentView = new ExperimentView(scienceData);
                    //if (!Output.Contains(experimentView))
                    Output.Add(experimentView);
                    OnShip.Add(experimentView);
                }
            }

            Output.Sort();
            break;
        }
    }


    private void OnDraw()
    {
        if (toolbarButton != null)
            toolbarButton.UpdateIcon(drawWindow);
        if (drawWindow)
        {
            windowPosition = GUI.Window(1234, windowPosition, OnWindow, "Science Monitor", windowStyle);
            KSPScienceSettings.setRectSetting("MonitorRect", windowPosition);
        }
    }

    private void OnWindow(int windowID)
    {
        GUI.skin = KSPScienceSettings.getSkin();
        //         GUILayout.BeginHorizontal();
        //         autoPauseOnNew = GUILayout.Toggle(autoPauseOnNew, "Auto Pause on new Science: " + autoPauseOnNew);
        //         GUILayout.EndHorizontal();
        scrollVector2 = GUILayout.BeginScrollView(scrollVector2);


        GUILayout.BeginHorizontal();
        for (int i = 0; i <= 6; i++)
        {
            GUILayout.BeginVertical();
            switch (i)
            {
                case 0:
                    GUILayout.Label(TextReplacer.GetReplaceForString("ID"));
                    break;
                case 1:
                    GUILayout.Label(TextReplacer.GetReplaceForString("Earned"));
                    break;
                case 2:
                    GUILayout.Label(TextReplacer.GetReplaceForString("Max"));
                    break;
                case 3:
                    GUILayout.Label(TextReplacer.GetReplaceForString("Remains"));
                    break;
                case 4:
                    GUILayout.Label(TextReplacer.GetReplaceForString("OnShip"));
                    break;
                case 5:
                    GUILayout.Label(TextReplacer.GetReplaceForString("NextExp"));
                    break;
                case 6:
                    if (KSPScienceSettings.getBoolSetting("ShowDeployButton"))
                        GUILayout.Label(TextReplacer.GetReplaceForString("Deploy"));
                    break;
            }
            foreach (ExperimentView experimentView in Output)
            {
                GUIStyle style = new GUIStyle();
                if (experimentView.OnShip)
                    style = KSPScienceSettings.getStyleSetting("MonitorOnShipExperiments");
                else if (experimentView.EarnedScience > 0)
                    style = KSPScienceSettings.getStyleSetting("MonitorKSCExperiments");
                else
                {
                    style = KSPScienceSettings.getStyleSetting("MonitorNewExperiments");
                    if (OnShip.Exists(view => view.FullExperimentId == experimentView.FullExperimentId))
                        continue;
                }
                switch (i)
                {
                    case 0:
                        GUILayout.Label(experimentView.FullExperimentId, style);
                        break;
                    case 1:
                    {
                        string strout = Math.Round(experimentView.EarnedScience, 1).ToString();
                        if (strout == "0") strout = "-";
                        GUILayout.Label(strout, style);
                    }
                        break;
                    case 2:
                    {
                        string strout = Math.Round(experimentView.FullScience, 1).ToString();
                        if (strout == "0") strout = "-";
                        GUILayout.Label(strout, style);
                    }
                        break;
                    case 3:
                        double percent = (experimentView.FullScience - experimentView.EarnedScience)/experimentView.FullScience*100;
                        if (experimentView.FullScience == 0)
                        {
                            GUILayout.Label("-", style);
                        } else if (percent >= 30)
                        {
                            GUIStyle tmpstyle = style;
                            style = KSPScienceSettings.getStyleSetting("MonitorNewExperiments");
                            GUILayout.Label(Math.Round(percent) + "%", style);
                            style = tmpstyle;
                        } else
                        {
                            GUILayout.Label(Math.Round(percent) + "%", style);
                        }
                        break;
                    case 4:
                        GUILayout.Label(experimentView.OnShip ? "\u221a" : " ", style);
                        break;
                    case 5:
                    {
                        string strout = Math.Round(experimentView.NextExperimentScience, 1).ToString();
                        if (strout == "0") strout = "-";
                        GUILayout.Label(strout, style);
                    }
                        break;
                    case 6:
                    {
                        if (KSPScienceSettings.getBoolSetting("ShowDeployButton"))
                        {
                            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                            buttonStyle.margin = new RectOffset(0, 0, 0, 0);
                            buttonStyle.normal.textColor = style.normal.textColor;
                            if (experimentView.FullExperimentId.Split('@')[0] == "asteroidSample")
                            {
                                //ModuleAsteroid[] asteroids = FindObjectsOfType<ModuleAsteroid>();
                                //ModuleScienceContainer collector = FlightGlobals.ActiveVessel.FindPartModulesImplementing<ModuleScienceContainer>().First<ModuleScienceContainer>();
                                //will be added in next version.
                                GUILayout.Button("------", buttonStyle, GUILayout.Height(15));
                            } else
                            {
                                bool foundFreeSpaceForExperiment = false;
                                ModuleScienceExperiment moduleScienceExperiment = null;
                                foreach (ModuleScienceExperiment _moduleScienceExperiment in FlightGlobals.ActiveVessel.FindPartModulesImplementing<ModuleScienceExperiment>())
                                {
                                    if (_moduleScienceExperiment.experimentID == experimentView.FullExperimentId.Split('@')[0])
                                    {
                                        if (!_moduleScienceExperiment.Deployed && !_moduleScienceExperiment.Inoperable && _moduleScienceExperiment.GetScienceCount() == 0)
                                        {
                                            foundFreeSpaceForExperiment = true;
                                            moduleScienceExperiment = _moduleScienceExperiment;
                                            break;
                                        }
                                    }
                                }
                                if (foundFreeSpaceForExperiment)
                                {
                                    if (GUILayout.Button(TextReplacer.GetReplaceForString("deploy"), buttonStyle, GUILayout.Height(15)))
                                        moduleScienceExperiment.DeployExperiment();
                                } else
                                    GUILayout.Button("------", buttonStyle, GUILayout.Height(15));
                            }
                        }
                    }
                        break;
                }
                //                 if (autoPauseOnNew && style.normal.textColor == Color.green)
                //                 {
                //                     //activate pause
                //                     TimeWarp.SetRate(0, true);
                //                     FlightDriver.SetPause(true);
                //                 }
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();
        //GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        if (GUI.Button(new Rect(windowPosition.width - 42, 0, 21, 21), "S"))
            KSPScienceSettings.Toggle();
        if (GUI.Button(new Rect(windowPosition.width - 21, 0, 21, 21), "X"))
            Hide();
        if (GUI.RepeatButton(new Rect(windowPosition.width - 21, windowPosition.height - 21, 21, 21), "\u21d8"))
            resizingWindow = true;
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }

    public void OnDestroy()
    {
        //print("Destroy Science Monitor");
        RenderingManager.RemoveFromPostDrawQueue(2, OnDraw);
    }

    public static void Hide()
    {
        drawWindow = false;
        KSPScienceSettings.Hide();
    }

    public static void Show()
    {
        drawWindow = true;
    }
}