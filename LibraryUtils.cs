using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

internal class LibraryUtils
{
    private static Dictionary<string, string> _experimentToPartRelation;
    private static readonly Dictionary<CelestialBody, Dictionary<string, Dictionary<string, Dictionary<ExperimentSituations, LibraryExperiment>>>> _databaseDictionary = new Dictionary<CelestialBody, Dictionary<string, Dictionary<string, Dictionary<ExperimentSituations, LibraryExperiment>>>>();
    //private static readonly List<LibraryExperiment> libraryExperiments = new List<LibraryExperiment>();


//     /// <summary>
//     ///     use GetAllPossibleLibraryExperiments to fill with data.
//     /// </summary>
//     public static List<LibraryExperiment> LibraryExperiments
//     {
//         get { return libraryExperiments; }
//     }

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
    /// <param name="foundSubject">
    ///     True if the Subject was found in the Game RAD database. False if we have just created a new
    ///     Subject
    /// </param>
    /// <returns></returns>
    public static ScienceSubject GetExperimentSubject(ScienceExperiment experiment, ExperimentSituations situation, CelestialBody body, string biome, out bool foundSubject)
    {
        foundSubject = true;
        ScienceSubject scienceSubject = new ScienceSubject(experiment, situation, body, biome);
        ScienceSubject subject = ResearchAndDevelopment.GetSubjectByID(scienceSubject.id);
        // this will cause error in log, but it is intended behavior. Well, not "intended", but there is no other way to do it.
        if (subject != null)
            return subject;
        foundSubject = false;
        return scienceSubject;
    }

    /// <summary>
    ///     Replace for ResearchAndDevelopment.GetExperimentSubject function. Original function inserts new ScienceSubject in
    ///     the database, but we do not want that.
    ///     This function is to use with asteroids only.
    ///     If there is no such experiment in the database the game will write "error" in log. Just ignore it.
    /// </summary>
    /// <param name="experiment"></param>
    /// <param name="situation"></param>
    /// <param name="sourceUId">Asteroid uid</param>
    /// <param name="sourceTitle">Asteroid title</param>
    /// <param name="body"></param>
    /// <param name="biome"></param>
    /// <param name="foundSubject">
    ///     True if the Subject was found in the Game RAD database. False if we have just created a new
    ///     Subject
    /// </param>
    /// <returns></returns>
    public static ScienceSubject GetExperimentSubject(ScienceExperiment experiment, ExperimentSituations situation, string sourceUId, string sourceTitle, CelestialBody body, string biome, out bool foundSubject)
    {
        foundSubject = true;
        ScienceSubject scienceSubject = new ScienceSubject(experiment, situation, sourceUId, sourceTitle, body, biome);
        ScienceSubject subject = ResearchAndDevelopment.GetSubjectByID(scienceSubject.id);
        // this will cause error in log, but it is intended behavior. Well, not "intended", but there is no other way to do it.
        if (subject != null)
            return subject;
        foundSubject = false;
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
    public static Dictionary<string, string> GetExperimentToPartRelation()
    {
        if (_experimentToPartRelation != null) return _experimentToPartRelation;
        _experimentToPartRelation = new Dictionary<string, string>();

        Dictionary<string, List<string>> tmp = new Dictionary<string, List<string>>();

        foreach (AvailablePart availablePart in PartLoader.LoadedPartsList)
        {
            string partName = availablePart.title;
            Part part = availablePart.partPrefab;
            PartModuleList modules = part.Modules;
            if (modules == null) continue;
            List<ModuleScienceExperiment> modules2 = part.FindModulesImplementing<ModuleScienceExperiment>();
            foreach (ModuleScienceExperiment moduleScienceExperiment in modules2)
            {
                if (!tmp.ContainsKey(moduleScienceExperiment.experimentID))
                    tmp.Add(moduleScienceExperiment.experimentID, new List<string>());
                tmp[moduleScienceExperiment.experimentID].Add(partName + Environment.NewLine);
            }
        }
        foreach (KeyValuePair<string, List<string>> keyValuePair in tmp)
        {
            _experimentToPartRelation.Add(keyValuePair.Key, String.Concat(keyValuePair.Value.ToArray()));
        }

        if (_experimentToPartRelation.ContainsKey("asteroidSample"))
            _experimentToPartRelation["asteroidSample"] = "EVA" + Environment.NewLine + _experimentToPartRelation["asteroidSample"];
        else
            _experimentToPartRelation.Add("asteroidSample", "EVA" + Environment.NewLine);
        if (_experimentToPartRelation.ContainsKey("evaReport"))
            _experimentToPartRelation["evaReport"] = "EVA" + Environment.NewLine + _experimentToPartRelation["evaReport"];
        else
            _experimentToPartRelation.Add("evaReport", "EVA" + Environment.NewLine);
        if (_experimentToPartRelation.ContainsKey("surfaceSample"))
            _experimentToPartRelation["surfaceSample"] = "EVA" + Environment.NewLine + _experimentToPartRelation["surfaceSample"];
        else
            _experimentToPartRelation.Add("surfaceSample", "EVA" + Environment.NewLine);

        return _experimentToPartRelation;
    }

    /// <summary>
    ///     Reads all configs and creates all possible experiments.
    ///     Some of them are in blacklist, like impossible "landed on Jool".
    ///     Don't use this function too often!
    /// </summary>
    /// <returns></returns>
    public static void GetAllPossibleLibraryExperiments()
    {
        MonoBehaviour.print("GetAllPossibleLibraryExperiments");
        List<ScienceSubject> gameSubjects = ResearchAndDevelopment.GetSubjects();
        //libraryExperiments.Clear();
        List<string> exIds = ResearchAndDevelopment.GetExperimentIDs();
        foreach (string firstId in exIds)
        {
            foreach (ExperimentSituations experimentSituation in Enum.GetValues(typeof (ExperimentSituations)))
            {
                foreach (CelestialBody body in FlightGlobals.Bodies)
                {
                    if ((experimentSituation == ExperimentSituations.SrfSplashed || experimentSituation == ExperimentSituations.SrfLanded) && (body.name == "Jool" || body.name == "Sun"))
                        continue;
                    if (!body.ocean && ExperimentSituations.SrfSplashed == experimentSituation)
                        continue;
                    if (!body.atmosphere && (ExperimentSituations.FlyingHigh == experimentSituation || ExperimentSituations.FlyingLow == experimentSituation))
                        continue;
                    ScienceExperiment experiment = ResearchAndDevelopment.GetExperiment(firstId);
                    bool available = experiment.IsAvailableWhile(experimentSituation, body);
                    if (available)
                    {
                        bool shouldHaveBiome = experiment.BiomeIsRelevantWhile(experimentSituation);
                        if (shouldHaveBiome && body.BiomeMap.Attributes.Length > 0)
                            foreach (string biome in ResearchAndDevelopment.GetBiomeTags(body))
                            {
                                if (experimentSituation != ExperimentSituations.SrfLanded && (biome == "KSC" || biome == "Runway" || biome == "LaunchPad")) continue;
                                LibraryExperiment libraryExperiment = new LibraryExperiment(firstId, experimentSituation, body, experiment, shouldHaveBiome, biome);
                                //libraryExperiments.Add(libraryExperiment);
                                addToDatabase(libraryExperiment);
                                gameSubjects.Remove(libraryExperiment.Subject);
                            }
                        else
                        {
                            LibraryExperiment libraryExperiment = new LibraryExperiment(firstId, experimentSituation, body, experiment, shouldHaveBiome, "");
                            //libraryExperiments.Add(libraryExperiment);
                            addToDatabase(libraryExperiment);
                            gameSubjects.Remove(libraryExperiment.Subject);
                        }
                    }
                }
            }
        }
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("BUG! Still not found science:");
        foreach (ScienceSubject gameSubject in gameSubjects)
            stringBuilder.AppendLine(gameSubject.id);
        MonoBehaviour.print(stringBuilder);
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
        result.Add("");
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
                        LibraryRow row = new LibraryRow(new GUIStyle());
                        bool rowHeader = true;
                        libraryView.rows.AddLast(row);
                        GUIStyle style = new GUIStyle(HighLogic.Skin.label);
                        style.padding = new RectOffset(5, 5, 30, 2);
                        row.cells.Add(new LibraryCell(body.name + " - " + biome, style));
                        foreach (string situationStr in Enum.GetNames(typeof (ExperimentSituations)))
                        {
                            row.cells.Add(new LibraryCell(situationStr, style));
                        }
                        foreach (string firstID in DatabaseDictionary[body][biome].Keys)
                        {
                            if (pressedScienceExperiments.Count == 0 || pressedScienceExperiments.Any(experiment => experiment.id == firstID))
                            {
                                rowHeader = false;
                                LibraryRow row2 = new LibraryRow(new GUIStyle());
                                libraryView.rows.AddLast(row2);
                                GUIStyle style2 = new GUIStyle(HighLogic.Skin.label);
                                style2.padding = new RectOffset(5, 5, 1, 1);
                                row2.cells.Add(new LibraryCell(TextReplacer.GetReplaceForString(firstID), style2));
                                foreach (ExperimentSituations situation in Enum.GetValues(typeof (ExperimentSituations)))
                                {
                                    if (DatabaseDictionary[body][biome][firstID].ContainsKey(situation))
                                    {
                                        LibraryExperiment lib = DatabaseDictionary[body][biome][firstID][situation];
                                        if (lib != null)
                                        {
                                            string tooltip = Environment.NewLine + "Earned:        " + lib.Earned + Environment.NewLine + "Total:          " + lib.ScienceCapacity + Environment.NewLine + "Earned percent: " + lib.EarnedPercent + "%" + Environment.NewLine;
                                            row2.cells.Add(new LibraryCell(lib.Earned + " / " + lib.ScienceCapacity + " (" + lib.EarnedPercent + "%)", tooltip, new GUIStyle(HighLogic.Skin.label)));
                                        } else
                                        {
                                            row2.cells.Add(new LibraryCell("---", new GUIStyle(HighLogic.Skin.label)));
                                        }
                                    } else
                                    {
                                        row2.cells.Add(new LibraryCell("---", new GUIStyle(HighLogic.Skin.label)));
                                    }
                                }
                            }
                        }
                        if (rowHeader)
                            libraryView.rows.RemoveLast();
                    }
                }
            }
        }
        return libraryView;
    }
}