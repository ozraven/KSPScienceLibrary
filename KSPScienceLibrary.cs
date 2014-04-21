using System;
using System.Collections.Generic;
using UnityEngine;

[KSPAddon(KSPAddon.Startup.EveryScene, false)]
public class KSPScienceLibrary : MonoBehaviour
{
    public static KSPScienceButton toolbarButton;
    private static Rect windowPosition = new Rect(5, 20, Screen.width - 10, Screen.height - 80);
    private static GUIStyle windowStyle;
    public static bool drawWindow = false;
    private List<string> allExperimentTypes = new List<string>();
    //private bool autoDeploy = false;
    //private CelestialBody[] bodies = null;
    //private Dictionary<string, CelestialBody> bodiesNames = null;
    private List<Experiment> dataOutputList;
    private bool lastdrawWindow;

    private int lastsorted;
    private int pressedFilterId = -1;
    private int pressedPlanet = -1;
    private bool resizingWindow;

    private Vector2 scrollPlanet = Vector2.zero;
    private Vector2 scrollPosition = Vector2.zero;
    private string selectedBody;
    private List<Experiment> selectedExperiments = new List<Experiment>();
    private List<Experiment> selectedExperiments2 = new List<Experiment>();

    public void Awake()
    {
        RenderingManager.AddToPostDrawQueue(0, OnDraw);
    }

    public void Start()
    {
        windowStyle = new GUIStyle(HighLogic.Skin.window);
        windowStyle.stretchHeight = true;
        windowStyle.stretchWidth = true;

        drawWindow = false;
        selectedBody = "All";
        selectedExperiments = dataOutputList;
    }

    public void OnDestroy()
    {
        print("Destroy Science Window");
        RenderingManager.RemoveFromPostDrawQueue(0, OnDraw);
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

        if (!lastdrawWindow && drawWindow)
        {
            GetSciData();
        }

        if (drawWindow)
            windowPosition = GUI.Window(2345, windowPosition, OnWindow, "Science Library", windowStyle);


        lastdrawWindow = drawWindow;
    }

    private void OnWindow(int windowID)
    {
        GUILayout.BeginArea(new Rect(10, 10, windowPosition.width - 10, windowPosition.height - 10));
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.MaxWidth(240));
        GUILayout.Label("Experiment Filters");
        scrollPlanet = GUILayout.BeginScrollView(scrollPlanet);
        int pressedCounter = 0;
        bool pressedlast = (pressedCounter == pressedFilterId);
        bool pressed;
        pressed = GUILayout.Toggle(pressedlast, "All Experiments", "Button");
        if (pressed && !pressedlast)
        {
            pressedFilterId = pressedCounter;
            selectedBody = "All";
            selectedExperiments = dataOutputList;
            selectedExperiments2 = selectedExperiments;
            pressedPlanet = -1;
        }

        foreach (string experimentType in allExperimentTypes)
        {
            pressedCounter++;
            pressedlast = (pressedCounter == pressedFilterId);
            pressed = GUILayout.Toggle(pressedlast, experimentType, "Button");
            if (pressed && !pressedlast)
            {
                pressedFilterId = pressedCounter;
                selectedBody = experimentType;
                selectedExperiments = GetSelectedExperimentsByType(experimentType);
                selectedExperiments2 = selectedExperiments;
                pressedPlanet = -1;
            }
        }
        CelestialBody sun = FlightGlobals.Bodies[0];


        CelestialBody[] bodies = FlightGlobals.Bodies.ToArray();
        Array.Sort(bodies, delegate(CelestialBody body1, CelestialBody body2)
        {
            if (body2.referenceBody == body1 && body1 != sun)
                return -1;
            if (body1.referenceBody == body2 && body2 != sun)
                return 1;
            if (body1.referenceBody == body2.referenceBody && body2.referenceBody != sun)
                return 0;
            return (sun.transform.position - body1.transform.position).magnitude.CompareTo((sun.transform.position - body2.transform.position).magnitude);
        });
        pressedCounter = 0;

        GUILayout.Space(20);
        foreach (CelestialBody body in bodies)
        {
            bool pressedlastPlanet = (pressedCounter == pressedPlanet);
            bool pressedP;
            if (body.referenceBody != sun)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(60);
            }
            pressedP = GUILayout.Toggle(pressedlastPlanet, body.name, "Button");
            if (pressedP && !pressedlastPlanet)
            {
                selectedBody = body.name;
                selectedExperiments2 = GetSelectedExperiments(selectedBody, selectedExperiments);
                pressedPlanet = pressedCounter;
            }
            if (body.referenceBody != sun)
            {
                GUILayout.EndHorizontal();
            }


            pressedCounter++;
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label(selectedBody + " Experiments List");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Sort by Name"))
        {
            if (lastsorted == 1)
            {
                selectedExperiments2.Sort(SortByNameD);
                lastsorted = 5;
            } else
            {
                selectedExperiments2.Sort(SortByName);
                lastsorted = 1;
            }
        }
        if (GUILayout.Button("Sort by Earned"))
        {
            if (lastsorted == 2)
            {
                selectedExperiments2.Sort(SortByEarnedD);
                lastsorted = 6;
            } else
            {
                selectedExperiments2.Sort(SortByEarned);
                lastsorted = 2;
            }
        }
        if (GUILayout.Button("Sort by Remaining"))
        {
            if (lastsorted == 3)
            {
                selectedExperiments2.Sort(SortByRemainD);
                lastsorted = 7;
            } else
            {
                selectedExperiments2.Sort(SortByRemain);
                lastsorted = 3;
            }
        }
        if (GUILayout.Button("Sort by Type"))
        {
            if (lastsorted == 4)
            {
                selectedExperiments2.Sort(SortByTypeD);
                lastsorted = 8;
            } else
            {
                selectedExperiments2.Sort(SortByType);
                lastsorted = 4;
            }
        }

        GUILayout.EndHorizontal();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUILayout.BeginVertical();
        GUILayout.Label("TYPE");
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;
        foreach (Experiment experiment in selectedExperiments2)
        {
            if (experiment.earned == 0)
                GUILayout.Label(experiment.FirstIdType, style);
            else
                GUILayout.Label(experiment.FirstIdType);
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label("EARNED");
        foreach (Experiment experiment in selectedExperiments2)
        {
            if (experiment.earned == 0)
                GUILayout.Label(experiment.earned.ToString(), style);
            else
                GUILayout.Label(experiment.earned.ToString());
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label("REMAINING");
        foreach (Experiment experiment in selectedExperiments2)
        {
            if (experiment.earned == 0)
                GUILayout.Label(experiment.remain.ToString(), style);
            else
                GUILayout.Label(experiment.remain.ToString());
        }
        GUILayout.EndVertical();
        GUILayout.Space(20);
        GUILayout.BeginVertical();
        GUILayout.Label("Library");
        foreach (Experiment experiment in selectedExperiments2)
        {
            if (experiment.earned == 0)
                GUILayout.Label(experiment.fullName, style);
            else
                GUILayout.Label(experiment.fullName);
        }
        GUILayout.EndVertical();
        GUILayout.Space(20);

        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
        GUILayout.Space(20);
        GUILayout.EndVertical();
        GUILayout.Space(20);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();


        if (GUI.Button(new Rect(windowPosition.width - 20, 0, 20, 20), "X"))
            drawWindow = false;
        if (GUI.RepeatButton(new Rect(windowPosition.width - 20, windowPosition.height - 20, 20, 20), "\u21d8"))
            resizingWindow = true;
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }

    public void GetSciData()
    {
        if (ResearchAndDevelopment.Instance == null)
            return;

        dataOutputList = new List<Experiment>();
        List<ScienceSubject> newExperiments = new List<ScienceSubject>();
        List<string> exIds = ResearchAndDevelopment.GetExperimentIDs();

        foreach (string id in exIds)
        {
            foreach (ExperimentSituations experimentSituation in Enum.GetValues(typeof (ExperimentSituations)))
            {
                foreach (CelestialBody body in FlightGlobals.Bodies)
                {
                    bool ocean = body.ocean;
                    if (ExperimentSituations.SrfSplashed == experimentSituation && !ocean)
                        continue;
                    ScienceExperiment experiment = ResearchAndDevelopment.GetExperiment(id);
                    bool available = experiment.IsAvailableWhile(experimentSituation, body);
                    if (available)
                    {
                        bool shouldHaveBiome = experiment.BiomeIsRelevantWhile(experimentSituation);
                        if (shouldHaveBiome)
                        {
                            foreach (string biome in ResearchAndDevelopment.GetBiomeTags(body))
                            {
                                ScienceExperiment scienceExperiment = ResearchAndDevelopment.GetExperiment(id);
                                newExperiments.Add(new ScienceSubject(scienceExperiment, experimentSituation, body, biome));
                            }
                            if (body.BiomeMap.Attributes.Length == 0)
                            {
                                ScienceExperiment scienceExperiment = ResearchAndDevelopment.GetExperiment(id);
                                newExperiments.Add(new ScienceSubject(scienceExperiment, experimentSituation, body, ""));
                            }
                        } else
                        {
                            ScienceExperiment scienceExperiment = ResearchAndDevelopment.GetExperiment(id);
                            newExperiments.Add(new ScienceSubject(scienceExperiment, experimentSituation, body, ""));
                        }
                    }
                }
            }
        }

        List<ScienceSubject> subjectslist = ResearchAndDevelopment.GetSubjects();
        foreach (ScienceSubject scienceSubject in subjectslist)
        {
            newExperiments.RemoveAll(subject => subject.id == scienceSubject.id);
            string title = scienceSubject.id;
            double earned = Math.Round(scienceSubject.science, 1);
            double remain = Math.Round(scienceSubject.scienceCap - scienceSubject.science, 1);
            string body = FindExperimentBody(scienceSubject.id.Split('@')[1]);
            string type = scienceSubject.id.Split('@')[0];
            Experiment experiment = new Experiment(title, earned, remain, body, type);
            dataOutputList.Add(experiment);
        }

        foreach (ScienceSubject newExperiment in newExperiments)
        {
            CelestialBody thisBody = FlightGlobals.Bodies.Find(celestialBody => newExperiment.id.Split('@')[1].StartsWith(celestialBody.name));
            Experiment ex = new Experiment(newExperiment.id, 0, Math.Round(newExperiment.scienceCap, 1), thisBody.name, newExperiment.id.Split('@')[0]);
            dataOutputList.Add(ex);
        }
        dataOutputList.Sort(SortByName);
        allExperimentTypes = GetAllDiscoveredExperimentTypes();
    }

    public List<Experiment> GetSelectedExperiments(string body, List<Experiment> input = null)
    {
        if (input == null)
            input = dataOutputList;
        List<Experiment> selectedExperimentsTemp = new List<Experiment>();
        foreach (Experiment experiment in input)
        {
            if (experiment.body == body)
            {
                selectedExperimentsTemp.Add(experiment);
            }
        }
        return selectedExperimentsTemp;
    }

    public List<Experiment> GetSelectedExperimentsByType(string type, List<Experiment> input = null)
    {
        if (input == null)
            input = dataOutputList;
        List<Experiment> selectedExperimentsTemp = new List<Experiment>();
        foreach (Experiment experiment in input)
        {
            if (experiment.FirstIdType == type)
                selectedExperimentsTemp.Add(experiment);
        }
        return selectedExperimentsTemp;
    }

    public List<string> GetAllDiscoveredExperimentTypes()
    {
        List<string> selectedExperimentTypesTemp = new List<string>();
        foreach (Experiment experiment in dataOutputList)
        {
            if (!selectedExperimentTypesTemp.Contains(experiment.FirstIdType))
                selectedExperimentTypesTemp.Add(experiment.FirstIdType);
        }
        selectedExperimentTypesTemp.Sort();
        return selectedExperimentTypesTemp;
    }

    private int SortByNameD(Experiment o1, Experiment o2)
    {
        return SortByName(o2, o1);
    }

    private int SortByName(Experiment o1, Experiment o2)
    {
        if (o1.earned == 0 && o2.earned == 0)
            return o1.fullName.CompareTo(o2.fullName);
        if (o1.earned == 0 || o2.earned == 0)
            return o2.earned.CompareTo(o1.earned);
        return o1.fullName.CompareTo(o2.fullName);
    }

    private int SortByEarnedD(Experiment o1, Experiment o2)
    {
        return SortByEarned(o2, o1);
    }

    private int SortByEarned(Experiment o1, Experiment o2)
    {
        return o2.earned.CompareTo(o1.earned);
    }

    private int SortByRemainD(Experiment o1, Experiment o2)
    {
        return SortByRemain(o2, o1);
    }

    private int SortByRemain(Experiment o1, Experiment o2)
    {
        if (o1.earned == 0 && o2.earned == 0)
            return o2.remain.CompareTo(o1.remain);
        if (o1.earned == 0 || o2.earned == 0)
            return o2.earned.CompareTo(o1.earned);
        return o2.remain.CompareTo(o1.remain);
    }

    private int SortByTypeD(Experiment o1, Experiment o2)
    {
        return SortByType(o2, o1);
    }

    private int SortByType(Experiment o1, Experiment o2)
    {
        if (o1.earned == 0 && o2.earned == 0)
            return o1.FirstIdType.CompareTo(o2.FirstIdType);
        if (o1.earned == 0 || o2.earned == 0)
            return o2.earned.CompareTo(o1.earned);
        return o1.FirstIdType.CompareTo(o2.FirstIdType);
    }

    public static string FindBiome(string experimentSecondText, ExperimentSituations situation)
    {
        string situationStr = situation.ToString();
        int offset = experimentSecondText.IndexOf(situationStr) + situationStr.Length;
        if (offset == experimentSecondText.Length)
            return "";
        return experimentSecondText.Substring(offset);
    }

    public static ExperimentSituations FindExperimentSituation(string experimentSecondText)
    {
        foreach (ExperimentSituations experimentSituation in Enum.GetValues(typeof (ExperimentSituations)))
            if (experimentSecondText.Contains(experimentSituation.ToString()))
                return experimentSituation;
        throw new Exception("Error in FindExperimentSituation: Can't find situation in '" + experimentSecondText + "'");
        //return null;
    }

    public static string FindExperimentBody(string experimentSecondText)
    {
        int firstUpperCase = 0;
        for (int i = 1; i < experimentSecondText.Length; i++)
        {
            if (char.IsUpper(experimentSecondText[i]) && firstUpperCase == 0)
            {
                firstUpperCase = i;
            }
        }
        return experimentSecondText.Substring(0, firstUpperCase);
    }
}