using System;

public struct Experiment
{
    public string FirstIdType;
    public string body;
    public double earned;
    public string fullName;
    public double remain;

    public Experiment(string eFullName, double eEarned, double eRemain, string eCelestialbody, string eFirstIdType)
    {
        fullName = eFullName;
        earned = eEarned;
        remain = eRemain;
        body = eCelestialbody;
        FirstIdType = eFirstIdType;
    }

    public override string ToString()
    {
        if (remain > 0 || earned > 0)
            return fullName + " " + Math.Round(earned, 2) + " " + Math.Round(remain, 2);
        return fullName + " " + "-" + " " + "-";
    }

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
    /// Replace for ResearchAndDevelopment.GetExperimentSubject function. Original function inserts new ScienceSubject in the database, but we do not want that.
    /// </summary>
    /// <param name="experiment"></param>
    /// <param name="situation"></param>
    /// <param name="body"></param>
    /// <param name="biome"></param>
    /// <returns></returns>
    public static ScienceSubject GetExperimentSubject(ScienceExperiment experiment, ExperimentSituations situation, CelestialBody body, string biome)
    {
        ScienceSubject scienceSubject = new ScienceSubject(experiment, situation, body, biome);
        ScienceSubject subject = ResearchAndDevelopment.GetSubjectByID(scienceSubject.id); // this will cause error in log, but it is intended behavior.
        if (subject != null)
            return subject;
        return scienceSubject;
    }
    /// <summary>
    /// Replace for ResearchAndDevelopment.GetExperimentSubject function. Original function inserts new ScienceSubject in the database, but we do not want that.
    /// </summary>
    /// <param name="experiment"></param>
    /// <param name="situation"></param>
    /// <param name="sourceUId"></param>
    /// <param name="sourceTitle"></param>
    /// <param name="body"></param>
    /// <param name="biome"></param>
    /// <returns></returns>
    public static ScienceSubject GetExperimentSubject(ScienceExperiment experiment, ExperimentSituations situation, string sourceUId, string sourceTitle, CelestialBody body, string biome)
    {
        ScienceSubject scienceSubject = new ScienceSubject(experiment, situation, sourceUId, sourceTitle, body, biome);
        ScienceSubject subject = ResearchAndDevelopment.GetSubjectByID(scienceSubject.id); // this will cause error in log, but it is intended behavior.
        if (subject != null)
            return subject;
        return scienceSubject;
    }
}