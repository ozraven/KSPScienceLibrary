using System;
using System.Collections.Generic;

internal class LibraryUtils
{
    private static Dictionary<string, List<string>> _experimentToPartRelation;

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
}