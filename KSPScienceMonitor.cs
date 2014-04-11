using System;
using System.Collections.Generic;
using UnityEngine;

[KSPAddon(KSPAddon.Startup.Flight, false)]
public class KSPScienceMonitor : MonoBehaviour
{
    public static KSPScienceMonitorButton toolbarButton;
    private static Rect windowPosition = new Rect(0, 0, 600, 200);
    private static GUIStyle windowStyle;

    public static bool drawWindow = false;

    // List of science on this ship
    //private readonly List<ExperimentView> OnShip = new List<ExperimentView>();
    //List of science to show on window
    private readonly List<ExperimentView> Output = new List<ExperimentView>();

    // List of possible science at this moment and it is the list to draw on window
    //private List<Experiment> ExperimentsNow = new List<Experiment>();

    // whether it should pause game at new science
    private bool autoPauseOnNew;


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
        RenderingManager.AddToPostDrawQueue(0, OnDraw);
    }

    public void Start()
    {
        windowStyle = new GUIStyle(HighLogic.Skin.window);
        windowStyle.stretchHeight = true;
        windowStyle.stretchWidth = true;
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

        // using "while" for possibility to "break". So we dont have to use GOTO
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
            //OnShip.Clear();


            //List<ScienceExperiment> PossibleExperiments = new List<ScienceExperiment>();

            //Search for all Science Experiment Modules on vessel
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
                            var experimentView = new ExperimentView(scienceData);
                            if (!Output.Contains(experimentView))
                                Output.Add(experimentView);
                        }
                    }
                    {
                        ScienceSubject scienceSubject = ResearchAndDevelopment.GetExperimentSubject(scienceExperiment, experimentSituation, mainbody,
                            scienceExperiment.BiomeIsRelevantWhile(experimentSituation) ? biome : "");
                        var experimentView = new ExperimentView(scienceSubject);
                        if (!Output.Contains(experimentView))
                            Output.Add(experimentView);
                    }
                }
            }

            if (FlightGlobals.ActiveVessel.GetCrewCount() > 0)
            {
                {
                    string firstExperimentId = "evaReport";
                    ScienceExperiment scienceExperiment = ResearchAndDevelopment.GetExperiment(firstExperimentId);
                    bool available = scienceExperiment.IsAvailableWhile(experimentSituation, mainbody);
                    if (available)
                    {
                        ScienceSubject scienceSubject = ResearchAndDevelopment.GetExperimentSubject(scienceExperiment, experimentSituation, mainbody,
                            scienceExperiment.BiomeIsRelevantWhile(experimentSituation) ? biome : "");
                        var experimentView = new ExperimentView(scienceSubject);
                        if (!Output.Contains(experimentView))
                            Output.Add(experimentView);
                    }
                }


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
                            ScienceSubject scienceSubject = ResearchAndDevelopment.GetExperimentSubject(scienceExperiment, experimentSituation, asteroidname, "", mainbody,
                                scienceExperiment.BiomeIsRelevantWhile(experimentSituation) ? biome : "");
                            var experimentView = new ExperimentView(scienceSubject);
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
                    var experimentView = new ExperimentView(scienceData);
                    //if (!Output.Contains(experimentView))
                    {
                        Output.Add(experimentView);
                    }
                }
            }


            //             if (autoPauseOnNew && !onship)
            //             {
            //                 //activate pause
            //                 TimeWarp.SetRate(0, true);
            //                 FlightDriver.SetPause(true);
            //             }


            break;
        }
    }

    private void OnDraw()
    {
        if (toolbarButton != null)
            toolbarButton.UpdateIcon(drawWindow);
        if (drawWindow)
            windowPosition = GUI.Window(1234, windowPosition, OnWindow, "Science Monitor", windowStyle);
    }

    private void OnWindow(int windowID)
    {
        GUILayout.BeginHorizontal();
        autoPauseOnNew = GUILayout.Toggle(autoPauseOnNew, "Auto Pause on new Science: " + autoPauseOnNew);
        GUILayout.EndHorizontal();
        scrollVector2 = GUILayout.BeginScrollView(scrollVector2);


        GUILayout.BeginHorizontal();
        for (int i = 0; i < 6; i++)
        {
            GUILayout.BeginVertical();
            switch (i)
            {
                case 0:
                    GUILayout.Label("ID");
                    break;
                case 1:
                    GUILayout.Label("Earned");
                    break;
                case 2:
                    GUILayout.Label("OnShip");
                    break;
                case 3:
                    GUILayout.Label("Max");
                    break;
                case 4:
                    GUILayout.Label("Depl");
                    break;
                case 5:
                    GUILayout.Label("Unde");
                    break;
            }
            foreach (ExperimentView experimentNow in Output)
            {
                var style = new GUIStyle();
                if (experimentNow.EarnedScience > 0)
                    style.normal.textColor = Color.red;
                else if (experimentNow.OnShip)
                    style.normal.textColor = Color.yellow;
                else
                    style.normal.textColor = Color.green;
                switch (i)
                {
                    case 0:
                        GUILayout.Label(experimentNow.FullExperimentId, style);
                        break;
                    case 1:
                        GUILayout.Label(Math.Round(experimentNow.EarnedScience, 2).ToString(), style);
                        break;
                    case 2:
                        GUILayout.Label(experimentNow.OnShip ? "\u221a" : " ", style);
                        break;
                    case 3:
                        GUILayout.Label(Math.Round(experimentNow.FullScience, 2).ToString(), style);
                        break;
                    case 4:
                        GUILayout.Label("-", style);
                        break;
                    case 5:
                        GUILayout.Label("-", style);
                        break;
                }
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();
        //GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUILayout.EndHorizontal();
        if (GUI.Button(new Rect(windowPosition.width - 20, 0, 20, 20), "X"))
            drawWindow = false;
        if (GUI.RepeatButton(new Rect(windowPosition.width - 20, windowPosition.height - 20, 20, 20), "\u21d8"))
            resizingWindow = true;
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }

    public void OnDestroy()
    {
        print("Destroy Science Monitor");
        RenderingManager.RemoveFromPostDrawQueue(0, OnDraw);
    }
}