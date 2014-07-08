using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[KSPAddon(KSPAddon.Startup.EveryScene, false)]
public class KSPScienceLibrary : MonoBehaviour
{
    public static KSPScienceButton toolbarButton;
    private static Rect windowPosition;
    private static GUIStyle windowStyle;
    private static bool drawWindow;
    private static KSPScienceLibrary instance;

    private static string version = "";
    private readonly List<string> pressedBiomes = new List<string>();
    private readonly List<CelestialBody> pressedCelestialBodies = new List<CelestialBody>();
    private readonly List<ScienceExperiment> pressedScienceExperiments = new List<ScienceExperiment>();
    private List<string> biomesList;
    private CelestialBody[] bodies;
    private List<LibraryExperiment> filtered;
    private bool lastdrawWindow;
    //private List<LibraryExperiment> libraryExperiments = new List<LibraryExperiment>();
    private bool resizingWindow;
    private Vector2 scrollBiomes;
    private Vector2 scrollContentPage;

    private Vector2 scrollExperiments;
    private Vector2 scrollPlanets;

    private LibraryView libraryView = new LibraryView();
    private CelestialBody sun;

    public static bool DrawWindow
    {
        get { return drawWindow; }
    }

    public void Awake()
    {
        instance = this;
        lastdrawWindow = false;

        sun = FlightGlobals.Bodies[0];
        bodies = FlightGlobals.Bodies.ToArray();
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

        RenderingManager.AddToPostDrawQueue(1, OnDraw);
    }

    public void Start()
    {
        windowStyle = new GUIStyle(HighLogic.Skin.window);
        windowStyle.stretchHeight = true;
        windowStyle.stretchWidth = true;

        drawWindow = false;

        System.Version ver = Assembly.GetAssembly(typeof (KSPScienceLibrary)).GetName().Version;
        version = "Version " + ver.Major + "." + ver.Minor + "   Build " + ver.Build + "." + ver.Revision;

        windowPosition = KSPScienceSettings.getRectSetting("LibraryRect");
    }

    public void OnDestroy()
    {
        RenderingManager.RemoveFromPostDrawQueue(1, OnDraw);
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
        if (HighLogic.LoadedScene == GameScenes.EDITOR && drawWindow)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.y = (Screen.height - mousePos.y);
            if (windowPosition.Contains(mousePos))
                EditorLogic.fetch.Lock(true, true, true, "library");
            else
                EditorLogic.fetch.Unlock("library");
        }
    }

    public static void Hide()
    {
        drawWindow = false;
        KSPScienceSettings.Hide();
        if (HighLogic.LoadedScene == GameScenes.EDITOR)
            EditorLogic.fetch.Unlock("library");
    }

    public static void Show()
    {
        LibraryUtils.GetAllPossibleLibraryExperiments();
        drawWindow = true;
    }

    private void OnDraw()
    {
        if (toolbarButton != null)
            toolbarButton.UpdateIcon(drawWindow);

        if (!lastdrawWindow && drawWindow)
        {
        }

        if (drawWindow)
        {
            windowPosition = GUI.Window(2345, windowPosition, OnWindow, "Science Library " + version, windowStyle);
            KSPScienceSettings.setRectSetting("LibraryRect", windowPosition);
        }

        lastdrawWindow = drawWindow;
    }

    private void OnWindow(int windowID)
    {
        GUI.skin = KSPScienceSettings.getSkin();
        bool filterChanged = false;
        GUILayout.BeginArea(new Rect(1, 21, windowPosition.width - 22, windowPosition.height - 42));

        GUILayout.BeginHorizontal();
        // start planets
        GUILayout.BeginVertical(GUILayout.MaxWidth(105), GUILayout.MinWidth(105));
        scrollPlanets = GUILayout.BeginScrollView(scrollPlanets);


        if (GUILayout.Toggle(pressedCelestialBodies.Count == 0, TextReplacer.GetReplaceForString("All Planets"), "Button") && pressedCelestialBodies.Count != 0)
        {
            pressedCelestialBodies.Clear();
            filterChanged = true;
        }
        GUILayout.Space(20);
        foreach (CelestialBody body in bodies)
        {
            bool pressedP;
            bool pressedPLast = pressedCelestialBodies.Contains(body);
            if (body.referenceBody != sun)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
            }
            pressedP = GUILayout.Toggle(pressedPLast, body.name, "Button");
            if (pressedP && !pressedPLast)
            {
                pressedCelestialBodies.Add(body);
                filterChanged = true;
            }
            if (!pressedP && pressedPLast)
            {
                pressedCelestialBodies.Remove(body);
                filterChanged = true;
            }
            if (body.referenceBody != sun)
                GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        // end planets

        // start experiments
        GUILayout.BeginVertical(GUILayout.MaxWidth(145), GUILayout.MinWidth(145));
        scrollExperiments = GUILayout.BeginScrollView(scrollExperiments);

        if (GUILayout.Toggle(pressedScienceExperiments.Count == 0, TextReplacer.GetReplaceForString("All Experiments"), "Button") && pressedScienceExperiments.Count != 0)
        {
            pressedScienceExperiments.Clear();
            filterChanged = true;
        }
        GUILayout.Space(20);

        foreach (string eid in ResearchAndDevelopment.GetExperimentIDs())
        {
            ScienceExperiment experiment = ResearchAndDevelopment.GetExperiment(eid);
            bool pressedP;
            bool pressedPLast = pressedScienceExperiments.Contains(experiment);

            pressedP = GUILayout.Toggle(pressedPLast, TextReplacer.GetReplaceForString(experiment.id), "Button");
            if (pressedP && !pressedPLast)
            {
                pressedScienceExperiments.Add(experiment);
                filterChanged = true;
            }
            if (!pressedP && pressedPLast)
            {
                pressedScienceExperiments.Remove(experiment);
                filterChanged = true;
            }
        }


        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        // end experiments
        GUILayout.BeginVertical(GUILayout.MaxWidth(windowPosition.width - 280), GUILayout.MinWidth(windowPosition.width - 280));
        // start biomes
        GUILayout.BeginHorizontal(GUILayout.MaxHeight(45), GUILayout.MinHeight(45));
        scrollBiomes = GUILayout.BeginScrollView(scrollBiomes);
        GUILayout.BeginHorizontal();
        if (GUILayout.Toggle(pressedBiomes.Count == 0, TextReplacer.GetReplaceForString("All Biomes"), "Button") && pressedBiomes.Count != 0)
        {
            pressedBiomes.Clear();
            filterChanged = true;
        }


        if (filterChanged || biomesList == null)
            biomesList = LibraryUtils.GetBiomesForPlanets(pressedCelestialBodies.Count == 0 ? (IEnumerable<CelestialBody>) bodies : pressedCelestialBodies);
        foreach (string biome in biomesList)
        {
            bool pressedP;
            bool pressedPLast = pressedBiomes.Contains(biome);

            pressedP = GUILayout.Toggle(pressedPLast, biome, "Button");
            if (pressedP && !pressedPLast)
            {
                pressedBiomes.Add(biome);
                filterChanged = true;
            }
            if (!pressedP && pressedPLast)
            {
                pressedBiomes.Remove(biome);
                filterChanged = true;
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();
        // end biomes
        // start content page
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        scrollContentPage = GUILayout.BeginScrollView(scrollContentPage);
        if (filterChanged || filtered == null)
            filtered = LibraryUtils.FilterLibraryExperiments(LibraryUtils.LibraryExperiments, pressedCelestialBodies, pressedScienceExperiments, pressedBiomes);

        //IEnumerable<CelestialBody> bodies = pressedCelestialBodies.Count == 0 ? (IEnumerable<CelestialBody>) bodies : pressedCelestialBodies;
        //Get biomes for body
        //getSituations
        //Experiments from body, biome, situation????????????

        //or should I use SQL???

        float width = (windowPosition.width - 280)/7;
        foreach (CelestialBody body in LibraryUtils.DatabaseDictionary.Keys)
        {
            if (pressedCelestialBodies.Count == 0 || pressedCelestialBodies.Contains(body))
            {
                foreach (string biome in LibraryUtils.DatabaseDictionary[body].Keys)
                {
                    if (pressedBiomes.Count == 0 || pressedBiomes.Contains(biome))
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(body.name + " - " + biome, GUILayout.MaxWidth(width), GUILayout.MinWidth(width));
                        foreach (string situationStr in Enum.GetNames(typeof (ExperimentSituations)))
                        {
                            GUILayout.Label(situationStr, GUILayout.MaxWidth(width), GUILayout.MinWidth(width));
                        }
                        GUILayout.EndHorizontal();

                        foreach (string firstID in LibraryUtils.DatabaseDictionary[body][biome].Keys)
                        {
                            if (pressedScienceExperiments.Count == 0 || pressedScienceExperiments.Any(experiment => experiment.id == firstID))
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Label(firstID, GUILayout.MaxWidth(width), GUILayout.MinWidth(width));
                                foreach (ExperimentSituations situation in Enum.GetValues(typeof (ExperimentSituations)))
                                {
                                    if (LibraryUtils.DatabaseDictionary[body][biome][firstID].ContainsKey(situation))
                                    {
                                        LibraryExperiment lib = LibraryUtils.DatabaseDictionary[body][biome][firstID][situation];
                                        if (lib != null)
                                            GUILayout.Label(lib.ScienceCapacity.ToString(), GUILayout.MaxWidth(width), GUILayout.MinWidth(width));
                                        else
                                            GUILayout.Label("---", GUILayout.MaxWidth(width), GUILayout.MinWidth(width));
                                    } else
                                    {
                                        GUILayout.Label("---", GUILayout.MaxWidth(width), GUILayout.MinWidth(width));
                                    }
                                }
                                GUILayout.EndHorizontal();
                            }
                        }
                        GUILayout.Space(20);
                    }
                }
            }
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        // stop content page
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
        if (GUI.Button(new Rect(windowPosition.width - 42, 0, 21, 21), "S"))
            KSPScienceSettings.Toggle();
        if (GUI.Button(new Rect(windowPosition.width - 21, 0, 21, 21), "X"))
            Hide();
        if (GUI.RepeatButton(new Rect(windowPosition.width - 21, windowPosition.height - 21, 21, 21), "\u21d8"))
            resizingWindow = true;
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }
}