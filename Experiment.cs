using System;

public struct Experiment
{
    public string FirstIdType;
    public string body;
    public double earned;
    public string fullName;
    public bool onShip;
    public double remain;

    public Experiment(string eFullName, double eEarned, double eRemain, string eCelestialbody, string eFirstIdType, bool eOnShip = false)
    {
        fullName = eFullName;
        earned = eEarned;
        remain = eRemain;
        body = eCelestialbody;
        FirstIdType = eFirstIdType;
        onShip = eOnShip;
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
}