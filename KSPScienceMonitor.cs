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
    private readonly List<String> OnShip = new List<string>();
    private List<Experiment> ExperimentsNow = new List<Experiment>();
    //
    private bool autoPauseOnNew;
    private Dictionary<string, Tupel<int, int>> idsList = new Dictionary<string, Tupel<int, int>>();
    //
    private float lateUpdateTimer;
    private List<ModuleScienceExperiment> modules = new List<ModuleScienceExperiment>();
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
        //         GUILayout.BeginHorizontal();
        //         GUILayout.Label("ABC-");
        //         GUILayout.Label("123");
        //         GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        autoPauseOnNew = GUILayout.Toggle(autoPauseOnNew, "Auto Pause on new Science: " + autoPauseOnNew);
        GUILayout.EndHorizontal();

        //         GUILayout.BeginHorizontal();
        //         drawWindow = GUILayout.Toggle(drawWindow, "Window State: " + drawWindow);
        //         GUILayout.EndHorizontal();
        Vessel.Situations situation = FlightGlobals.ActiveVessel.situation;
        //         GUILayout.Label(situation.ToString());
        //         Vessel.State state = FlightGlobals.ActiveVessel.state;
        //         GUILayout.Label(state.ToString());
        ExperimentSituations experimentSituation = ExperimentSituationFromVesselSituation(situation, FlightGlobals.ActiveVessel);
        //GUILayout.Label(experimentSituation.ToString());
        //         FlightCtrlState ctrlState = FlightGlobals.ActiveVessel.ctrlState;
        //         GUILayout.Label(ctrlState.ToString());
        CelestialBody mainbody = FlightGlobals.ActiveVessel.mainBody;
        //GUILayout.Label(mainbody.name);
        scrollVector2 = GUILayout.BeginScrollView(scrollVector2);
        //GUILayout.BeginHorizontal();


        if (lateUpdateTimer < Time.time)
        {
            idsList = new Dictionary<string, Tupel<int, int>>();
            OnShip.Clear();
            //modules = new List<ModuleScienceExperiment>();
            lateUpdateTimer = Time.time + 1;

            //Search for all Science Experiment Modules on vessel
            foreach (ModuleScienceExperiment moduleScienceExperiment in FlightGlobals.ActiveVessel.FindPartModulesImplementing<ModuleScienceExperiment>())
            {
                if (!idsList.ContainsKey(moduleScienceExperiment.experimentID))
                    idsList.Add(moduleScienceExperiment.experimentID,
                        new Tupel<int, int>(moduleScienceExperiment.Deployed ? 1 : 0, moduleScienceExperiment.Deployed ? 0 : 1));
                else
                    idsList[moduleScienceExperiment.experimentID] =
                        new Tupel<int, int>(
                            moduleScienceExperiment.Deployed ? idsList[moduleScienceExperiment.experimentID].t1 + 1 : idsList[moduleScienceExperiment.experimentID].t1 + 0,
                            moduleScienceExperiment.Deployed ? idsList[moduleScienceExperiment.experimentID].t2 + 0 : idsList[moduleScienceExperiment.experimentID].t2 + 1);
                //modules.Add(moduleScienceExperiment);

                //Write all found deployed experiments into OnShip list
                if (moduleScienceExperiment.Deployed)
                {
                    foreach (ScienceData scienceData in moduleScienceExperiment.GetData())
                    {
                        if (!OnShip.Contains(scienceData.subjectID))
                        {
                            OnShip.Add(scienceData.subjectID);
                        }
                    }
                }
            }

            //Search for all Science Containers on vessel and write all found experiments into OnShip list
            foreach (ModuleScienceContainer moduleScienceContainer in FlightGlobals.ActiveVessel.FindPartModulesImplementing<ModuleScienceContainer>())
            {
                foreach (ScienceData scienceData in moduleScienceContainer.GetData())
                {
                    if (!OnShip.Contains(scienceData.subjectID))
                    {
                        OnShip.Add(scienceData.subjectID);
                    }
                }
            }


            if (FlightGlobals.ActiveVessel.GetCrewCount() > 0)
            {
                if (!idsList.ContainsKey("evaReport"))
                    idsList.Add("evaReport", new Tupel<int, int>(0, 0));


                ModuleAsteroid[] asteroids = FindObjectsOfType<ModuleAsteroid>();
                foreach (ModuleAsteroid asteroid in asteroids)
                {
                    Vector3 destination3 = asteroid.gameObject.transform.position - FlightGlobals.ActiveVessel.gameObject.transform.position;
                    float unfocusedRange = asteroid.Events["TakeSampleEVAEvent"].unfocusedRange;
                    unfocusedRange *= unfocusedRange;
                    if (destination3.sqrMagnitude < unfocusedRange)
                    {
                        if (!idsList.ContainsKey("asteroidSample"))
                            idsList.Add("asteroidSample", new Tupel<int, int>(0, 0));
                    }
                }
            }
            ExperimentsNow = new List<Experiment>();


            string biome = "";
            if (FlightGlobals.ActiveVessel.landedAt != string.Empty)
                biome += FlightGlobals.ActiveVessel.landedAt;
            else
                biome +=
                    FlightGlobals.ActiveVessel.mainBody.BiomeMap.GetAtt(FlightGlobals.ActiveVessel.latitude*Math.PI/180d,
                        FlightGlobals.ActiveVessel.longitude*Math.PI/180d).name;

            //Get allready done experiments from space center
            List<ScienceSubject> subjectslist = ResearchAndDevelopment.GetSubjects();

            //Get Possible firstIDs from Vessel
            foreach (string firstExperimentId in idsList.Keys)
            {
                CelestialBody thisBody = FlightGlobals.ActiveVessel.mainBody;
                ScienceExperiment experiment = ResearchAndDevelopment.GetExperiment(firstExperimentId);
                bool biomeNeeded = experiment.BiomeIsRelevantWhile(experimentSituation);
                bool available = experiment.IsAvailableWhile(experimentSituation, thisBody);
                if (available)
                {
                    string experimentFullName = firstExperimentId + "@" + mainbody.name + experimentSituation;
                    if (biomeNeeded)
                        experimentFullName += biome.Replace(" ", string.Empty);
                    bool foundAtSpaceCenter = false;
                    ScienceSubject foundScienceSubject = null;

                    //Test if this experiment allready made, then set foundAtSpaceCenter=true
                    foreach (ScienceSubject scienceSubject in subjectslist)
                        if (scienceSubject.id == experimentFullName)
                        {
                            foundAtSpaceCenter = true;
                            foundScienceSubject = scienceSubject;
                            break;
                        }
                    var style = new GUIStyle();


                    bool onship = OnShip.Contains(experimentFullName);


                    if (foundAtSpaceCenter)
                    {
                        ExperimentsNow.Add(new Experiment(experimentFullName, foundScienceSubject.science, foundScienceSubject.scienceCap - foundScienceSubject.science,
                            thisBody.name,
                            firstExperimentId, onship));
                    } else
                    {
                        ExperimentsNow.Add(new Experiment(experimentFullName, 0, experiment.baseValue*experiment.dataScale, thisBody.name, firstExperimentId, onship));


                        if (autoPauseOnNew && !onship)
                        {
                            //activate pause
                            TimeWarp.SetRate(0, true);
                            FlightDriver.SetPause(true);
                        }
                    }
                }
            }
        }

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
                    GUILayout.Label("Remaining");
                    break;
                case 4:
                    GUILayout.Label("Depl");
                    break;
                case 5:
                    GUILayout.Label("Unde");
                    break;
            }
            foreach (Experiment experimentNow in ExperimentsNow)
            {
                var style = new GUIStyle();
                if (experimentNow.earned > 0)
                    style.normal.textColor = Color.red;
                else if (experimentNow.onShip)
                    style.normal.textColor = Color.yellow;
                else
                    style.normal.textColor = Color.green;
                switch (i)
                {
                    case 0:
                        GUILayout.Label(experimentNow.fullName, style);
                        break;
                    case 1:
                        GUILayout.Label(Math.Round(experimentNow.earned, 2).ToString(), style);
                        break;
                    case 2:
                        GUILayout.Label(experimentNow.onShip ? "\u221a" : " ", style);
                        break;
                    case 3:
                        GUILayout.Label(Math.Round(experimentNow.remain, 2).ToString(), style);
                        break;
                    case 4:
                        GUILayout.Label(idsList[experimentNow.FirstIdType].t1.ToString(), style);
                        break;
                    case 5:
                        GUILayout.Label(idsList[experimentNow.FirstIdType].t2.ToString(), style);
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

    public static ExperimentSituations ExperimentSituationFromVesselSituation(Vessel.Situations situation, Vessel thisVessel)
    {
        switch (situation)
        {
            case Vessel.Situations.SPLASHED:
                return ExperimentSituations.SrfSplashed;
            case Vessel.Situations.LANDED:
            case Vessel.Situations.PRELAUNCH:
                return ExperimentSituations.SrfLanded;
            case Vessel.Situations.FLYING:
                if (thisVessel.altitude < thisVessel.mainBody.scienceValues.flyingAltitudeThreshold)
                    return ExperimentSituations.FlyingLow;
                return ExperimentSituations.FlyingHigh;
            case Vessel.Situations.SUB_ORBITAL:
            case Vessel.Situations.ORBITING:
            case Vessel.Situations.ESCAPING:
            case Vessel.Situations.DOCKED:
                if (thisVessel.altitude < thisVessel.mainBody.scienceValues.spaceAltitudeThreshold)
                    return ExperimentSituations.InSpaceLow;
                return ExperimentSituations.InSpaceHigh;
        }
        return ExperimentSituations.SrfSplashed;
    }

    public void OnDestroy()
    {
        print("Destroy Science Monitor");
        RenderingManager.RemoveFromPostDrawQueue(0, OnDraw);
    }
}