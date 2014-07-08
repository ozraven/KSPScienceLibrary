using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class LibraryUtils
{
    private static Dictionary<string, List<string>> _experimentToPartRelation;
    private static readonly Dictionary<CelestialBody, Dictionary<string, Dictionary<string, Dictionary<ExperimentSituations, LibraryExperiment>>>> _databaseDictionary = new Dictionary<CelestialBody, Dictionary<string, Dictionary<string, Dictionary<ExperimentSituations, LibraryExperiment>>>>();
    private static readonly List<LibraryExperiment> libraryExperiments = new List<LibraryExperiment>();


    /// <summary>
    ///     use GetAllPossibleLibraryExperiments to fill with data.
    /// </summary>
    public static List<LibraryExperiment> LibraryExperiments
    {
        get { return libraryExperiments; }
    }

    /// <summary>
    ///     use GetAllPossibleLibraryExperiments to fill with data.
    /// </summary>
    public static Dictionary<CelestialBody, Dictionary<string, Dictionary<string, Dictionary<ExperimentSituations, LibraryExperiment>>>> DatabaseDictionary
    {
        get { return _databaseDictionary; }
    }

    /// <summary>
    ///     Replace for ResearchAndDevelopment.GetExperimentSubject function. Original function inserts new ScienceSubject in
    ///     the database, but we do not want that.
    ///     This function is for all experiments, but asteroids.
    ///     If there is no such experiment in the database the game will write "error" in log. Just ignore it.
    /// </summary>
    /// <param name="experiment"></param>
    /// <param name="situation"></param>
    /// <param name="body"></param>
    /// <param name="biome"></param>
    /// <returns></returns>
    public static ScienceSubject GetExperimentSubject(ScienceExperiment experiment, ExperimentSituations situation, CelestialBody body, string biome)
    {
        ScienceSubject scienceSubject = new ScienceSubject(experiment, situation, body, biome);
        ScienceSubject subject = ResearchAndDevelopment.GetSubjectByID(scienceSubject.id);
        // this will cause error in log, but it is intended behavior. Well, not "intended", but there is no other way to do it.
        if (subject != null)
            return subject;
        return scienceSubject;
    }

    /// <summary>
    ///     Replace for ResearchAndDevelopment.GetExperimentSubject function. Original function inserts new ScienceSubject in
    ///     the database, but we do not want that.
    ///     This function is to use with asteroids.
    ///     If there is no such experiment in the database the game will write "error" in log. Just ignore it.
    /// </summary>
    /// <param name="experiment"></param>
    /// <param name="situation"></param>
    /// <param name="sourceUId">Asteroid uid</param>
    /// <param name="sourceTitle">Asteroid title</param>
    /// <param name="body"></param>
    /// <param name="biome"></param>
    /// <returns></returns>
    public static ScienceSubject GetExperimentSubject(ScienceExperiment experiment, ExperimentSituations situation, string sourceUId, string sourceTitle, CelestialBody body, string biome)
    {
        ScienceSubject scienceSubject = new ScienceSubject(experiment, situation, sourceUId, sourceTitle, body, biome);
        ScienceSubject subject = ResearchAndDevelopment.GetSubjectByID(scienceSubject.id);
        // this will cause error in log, but it is intended behavior. Well, not "intended", but there is no other way to do it.
        if (subject != null)
            return subject;
        return scienceSubject;
    }

    /// <summary>
    ///     ScienceDataValue for body and situation.
    /// </summary>
    /// <param name="body"></param>
    /// <param name="situation"></param>
    /// <returns></returns>
    public static double ScienceDataValue(CelestialBody body, ExperimentSituations situation)
    {
        switch (situation)
        {
            case ExperimentSituations.SrfLanded:
                return body.scienceValues.LandedDataValue;
            case ExperimentSituations.SrfSplashed:
                return body.scienceValues.SplashedDataValue;
            case ExperimentSituations.FlyingLow:
                return body.scienceValues.FlyingLowDataValue;
            case ExperimentSituations.FlyingHigh:
                return body.scienceValues.FlyingHighDataValue;
            case ExperimentSituations.InSpaceLow:
                return body.scienceValues.InSpaceLowDataValue;
            case ExperimentSituations.InSpaceHigh:
                return body.scienceValues.InSpaceHighDataValue;
            default:
                throw new ArgumentOutOfRangeException("situation");
        }
    }

    /// <summary>
    ///     Get Body name from Second ExperimentID
    /// </summary>
    /// <param name="experimentSecondText">SecondExperimentId = that comes AFTER @ character!</param>
    /// <returns></returns>
    public static string FindExperimentBody(string experimentSecondText)
    {
        int firstUpperCase = 0;
        for (int i = 1; i < experimentSecondText.Length; i++)
        {
            if (Char.IsUpper(experimentSecondText[i]) && firstUpperCase == 0)
            {
                firstUpperCase = i;
            }
        }
        return experimentSecondText.Substring(0, firstUpperCase);
    }

    /// <summary>
    ///     Gets Biome name from Second ExperimentID. Don't use it for asteroids!
    /// </summary>
    /// <param name="experimentSecondText">SecondExperimentId = that comes AFTER @ character!</param>
    /// <param name="situation">This should be known</param>
    /// <returns></returns>
    public static string FindBiome(string experimentSecondText, ExperimentSituations situation)
    {
        string situationStr = situation.ToString();
        int offset = experimentSecondText.IndexOf(situationStr) + situationStr.Length;
        if (offset == experimentSecondText.Length)
            return "";
        return experimentSecondText.Substring(offset);
    }

    /// <summary>
    ///     Find Experiment Situation from Second ExperimentID
    /// </summary>
    /// <param name="experimentSecondText">SecondExperimentId = that comes AFTER @ character!</param>
    /// <returns></returns>
    public static ExperimentSituations FindExperimentSituation(string experimentSecondText)
    {
        foreach (ExperimentSituations experimentSituation in Enum.GetValues(typeof (ExperimentSituations)))
            if (experimentSecondText.Contains(experimentSituation.ToString()))
                return experimentSituation;
        throw new Exception("Error in FindExperimentSituation: Can't find situation in '" + experimentSecondText + "'");
        //return null;
    }

    /// <summary>
    ///     Get relationship of experimentFirstId and Part titles those can be used for this experiment.
    ///     It runs only once, but there is no way to add new Science Parts in runtime, is it?
    /// </summary>
    /// <returns>Dictionary with KEY=ExperimentFirstId, Value=List of Parts those can deploy this experiment</returns>
    public static Dictionary<string, List<string>> GetExperimentToPartRelation()
    {
        if (_experimentToPartRelation != null) return _experimentToPartRelation;
        _experimentToPartRelation = new Dictionary<string, List<string>>();
        foreach (AvailablePart availablePart in PartLoader.LoadedPartsList)
        {
            string partName = availablePart.title;
            Part part = availablePart.partPrefab;
            PartModuleList modules = part.Modules;
            if (modules == null) continue;
            List<ModuleScienceExperiment> modules2 = part.FindModulesImplementing<ModuleScienceExperiment>();
            foreach (ModuleScienceExperiment moduleScienceExperiment in modules2)
            {
                if (!_experimentToPartRelation.ContainsKey(moduleScienceExperiment.experimentID))
                    _experimentToPartRelation.Add(moduleScienceExperiment.experimentID, new List<string>());
                _experimentToPartRelation[moduleScienceExperiment.experimentID].Add(partName);
            }
        }
        return _experimentToPartRelation;
    }

    /// <summary>
    ///     Reads all configs and creates all possible experiments.
    ///     Not all of them can be created in game. For example impossible "landed on Jool".
    ///     But from the config layer it is still possible.
    ///     Because config does not know about "special conditions" of Jool surface.
    ///     Don't use this function too often!
    /// </summary>
    /// <returns></returns>
    public static void GetAllPossibleLibraryExperiments()
    {
        MonoBehaviour.print("GetAllPossibleLibraryExperiments");
        libraryExperiments.Clear();
        List<string> exIds = ResearchAndDevelopment.GetExperimentIDs();
        foreach (string firstId in exIds)
        {
            foreach (ExperimentSituations experimentSituation in Enum.GetValues(typeof (ExperimentSituations)))
            {
                foreach (CelestialBody body in FlightGlobals.Bodies)
                {
                    bool ocean = body.ocean;
                    if (ExperimentSituations.SrfSplashed == experimentSituation && !ocean)
                        continue;
                    if ((ExperimentSituations.FlyingHigh == experimentSituation || ExperimentSituations.FlyingLow == experimentSituation) && !body.atmosphere)
                        continue;
                    ScienceExperiment experiment = ResearchAndDevelopment.GetExperiment(firstId);
                    bool available = experiment.IsAvailableWhile(experimentSituation, body);
                    if (available)
                    {
                        bool shouldHaveBiome = experiment.BiomeIsRelevantWhile(experimentSituation);
                        if (shouldHaveBiome && body.BiomeMap.Attributes.Length > 0)
                            foreach (string biome in ResearchAndDevelopment.GetBiomeTags(body))
                            {
                                LibraryExperiment libraryExperiment = new LibraryExperiment(firstId, experimentSituation, body, experiment, shouldHaveBiome, biome);
                                libraryExperiments.Add(libraryExperiment);
                                addToDatabase(libraryExperiment);
                            }
                        else
                        {
                            LibraryExperiment libraryExperiment = new LibraryExperiment(firstId, experimentSituation, body, experiment, shouldHaveBiome, "");
                            libraryExperiments.Add(libraryExperiment);
                            addToDatabase(libraryExperiment);
                        }
                    }
                }
            }
        }
    }

    private static void addToDatabase(LibraryExperiment experiment)
    {
        if (!DatabaseDictionary.ContainsKey(experiment.CelestialBody))
            DatabaseDictionary.Add(experiment.CelestialBody, new Dictionary<string, Dictionary<string, Dictionary<ExperimentSituations, LibraryExperiment>>>());
        if (!DatabaseDictionary[experiment.CelestialBody].ContainsKey(experiment.Biome))
            DatabaseDictionary[experiment.CelestialBody].Add(experiment.Biome, new Dictionary<string, Dictionary<ExperimentSituations, LibraryExperiment>>());
        if (!DatabaseDictionary[experiment.CelestialBody][experiment.Biome].ContainsKey(experiment.FirstId))
            DatabaseDictionary[experiment.CelestialBody][experiment.Biome].Add(experiment.FirstId, new Dictionary<ExperimentSituations, LibraryExperiment>());
        if (!DatabaseDictionary[experiment.CelestialBody][experiment.Biome][experiment.FirstId].ContainsKey(experiment.ExperimentSituation))
            DatabaseDictionary[experiment.CelestialBody][experiment.Biome][experiment.FirstId].Add(experiment.ExperimentSituation, experiment);
    }

    /// <summary>
    ///     Filter Experiments and return only Experiments those have CelestialBodys & ExperimentIDs & Biomes
    ///     ( (body1 || ... || bodyN) && (Experiment1 || ... || ExperimentN) && (Biome1 || ... || BiomeN) )
    ///     Don't use too often!
    /// </summary>
    /// <param name="libraryExperiments">Input LibraryExperiment List</param>
    /// <param name="filterBy_body_experiment_biome">
    ///     can be of type Celestial body = filter body, ScienceExperiment = filter ExperimentId,
    ///     String = filter Biomes
    /// </param>
    /// <returns>Result List</returns>
    public static List<LibraryExperiment> FilterLibraryExperiments(List<LibraryExperiment> libraryExperiments, List<CelestialBody> filterByBody, List<ScienceExperiment> filterByExperiment, List<string> filterByBiome)
    {
        MonoBehaviour.print("FilterLibraryExperiments");
        List<LibraryExperiment> result = new List<LibraryExperiment>();
        foreach (LibraryExperiment libraryExperiment in libraryExperiments)
        {
            bool bodyFilter = false;
            bool experimentFilter = false;
            bool biomeFilter = false;

            if (filterByBody.Count == 0) bodyFilter = true;
            else
                foreach (CelestialBody celestialBody in filterByBody)
                    if (celestialBody == libraryExperiment.CelestialBody)
                    {
                        bodyFilter = true;
                        break;
                    }
            if (filterByExperiment.Count == 0) experimentFilter = true;
            else
                foreach (ScienceExperiment experiment in filterByExperiment)
                    if (experiment.id == libraryExperiment.ScienceExperiment.id)
                    {
                        experimentFilter = true;
                        break;
                    }
            if (filterByBiome.Count == 0) biomeFilter = true;
            else
                foreach (string biome in filterByBiome)
                    if (biome == libraryExperiment.Biome)
                    {
                        biomeFilter = true;
                        break;
                    }
            if (bodyFilter && experimentFilter && biomeFilter)
                result.Add(libraryExperiment);
        }
        return result;
    }

    /// <summary>
    ///     Go through list of planets and write down all biomes.
    ///     Don't use too often!
    /// </summary>
    /// <param name="planets">input list</param>
    /// <returns>Found biomes</returns>
    public static List<string> GetBiomesForPlanets(IEnumerable<CelestialBody> planets)
    {
        MonoBehaviour.print("GetBiomesForPlanets");
        List<string> result = new List<string>();
        foreach (CelestialBody planet in planets)
            foreach (string biome in ResearchAndDevelopment.GetBiomeTags(planet))
                result.Add(biome);
        return result;
    }


    public static LibraryView GetLibraryView(List<CelestialBody> pressedCelestialBodies, List<string> pressedBiomes, List<ScienceExperiment> pressedScienceExperiments)
    {
        MonoBehaviour.print("GetLibraryView");
        LibraryView libraryView = new LibraryView();

        foreach (CelestialBody body in DatabaseDictionary.Keys)
        {
            if (pressedCelestialBodies.Count == 0 || pressedCelestialBodies.Contains(body))
            {
                foreach (string biome in DatabaseDictionary[body].Keys)
                {
                    if (pressedBiomes.Count == 0 || pressedBiomes.Contains(biome))
                    {
                        //GUILayout.BeginHorizontal();
                        LibraryRow row = new LibraryRow(new GUIStyle());

                        libraryView.rows.Add(row);
                        GUIStyle style = new GUIStyle(HighLogic.Skin.label);

                        style.padding = new RectOffset(5, 5, 30, 2);
                        row.cells.Add(new LibraryCell(body.name + " - " + biome, style));
                        //GUILayout.Label(body.name + " - " + biome, GUILayout.MaxWidth(width), GUILayout.MinWidth(width));

                        foreach (string situationStr in Enum.GetNames(typeof (ExperimentSituations)))
                        {
                            //GUILayout.Label(situationStr, GUILayout.MaxWidth(width), GUILayout.MinWidth(width));
                            row.cells.Add(new LibraryCell(situationStr, style));
                        }
                        //GUILayout.EndHorizontal();

                        foreach (string firstID in DatabaseDictionary[body][biome].Keys)
                        {
                            if (pressedScienceExperiments.Count == 0 || pressedScienceExperiments.Any(experiment => experiment.id == firstID))
                            {
                                //GUILayout.BeginHorizontal();
                                //GUILayout.Label(firstID, GUILayout.MaxWidth(width), GUILayout.MinWidth(width));
                                LibraryRow row2 = new LibraryRow(new GUIStyle());
                                libraryView.rows.Add(row2);
                                GUIStyle style2 = new GUIStyle(HighLogic.Skin.label);
                                style2.padding = new RectOffset(5, 5, 1, 1);
                                row2.cells.Add(new LibraryCell(TextReplacer.GetReplaceForString(firstID), style2));
                                foreach (ExperimentSituations situation in Enum.GetValues(typeof (ExperimentSituations)))
                                {
                                    if(situation == ExperimentSituations.SrfSplashed && !body.ocean)
                                        continue;
                                    if((situation == ExperimentSituations.FlyingHigh || situation == ExperimentSituations.FlyingLow) && !body.atmosphere)
                                        continue;
                                    if (DatabaseDictionary[body][biome][firstID].ContainsKey(situation))
                                    {
                                        LibraryExperiment lib = DatabaseDictionary[body][biome][firstID][situation];
                                        if (lib != null)
                                        {
                                            //GUILayout.Label(lib.ScienceCapacity.ToString(), GUILayout.MaxWidth(width), GUILayout.MinWidth(width));
                                            row2.cells.Add(new LibraryCell(lib.Earned+" / "+lib.ScienceCapacity+" ("+lib.EarnedPercent+"%)" , new GUIStyle(HighLogic.Skin.label)));
                                        } else
                                        {
                                            //GUILayout.Label("---", GUILayout.MaxWidth(width), GUILayout.MinWidth(width));
                                            row2.cells.Add(new LibraryCell("---", new GUIStyle(HighLogic.Skin.label)));
                                        }
                                    } else
                                    {
                                        //GUILayout.Label("---", GUILayout.MaxWidth(width), GUILayout.MinWidth(width));
                                        row2.cells.Add(new LibraryCell("---", new GUIStyle(HighLogic.Skin.label)));
                                    }
                                }
                                //GUILayout.EndHorizontal();
                            }
                        }
                        //GUILayout.Space(20);
                    }
                }
            }
        }
        return libraryView;
    }
}