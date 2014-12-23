using System;

namespace KSPScienceLibrary
{
    internal static class LibraryUtils
    {
        /// <summary>
        ///     Replace for ResearchAndDevelopment.GetExperimentSubject function. Original function inserts new ScienceSubject in
        ///     the database, but we do not want that.
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
            if(subject != null)
                return subject;
            return scienceSubject;
        }

        /// <summary>
        ///     Replace for ResearchAndDevelopment.GetExperimentSubject function. Original function inserts new ScienceSubject in
        ///     the database, but we do not want that.
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
            if(subject != null)
                return subject;
            return scienceSubject;
        }

        public static double ScienceDataValue(CelestialBody body, ExperimentSituations situation)
        {
            switch(situation)
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

        public static string FindExperimentBody(string experimentSecondText)
        {
            int firstUpperCase = 0;
            for(int i = 1; i < experimentSecondText.Length; i++)
            {
                if(Char.IsUpper(experimentSecondText[i]) && firstUpperCase == 0)
                {
                    firstUpperCase = i;
                }
            }
            return experimentSecondText.Substring(0, firstUpperCase);
        }

        public static string FindBiome(string experimentSecondText, ExperimentSituations situation)
        {
            string situationStr = situation.ToString();
            int offset = experimentSecondText.IndexOf(situationStr) + situationStr.Length;
            if(offset == experimentSecondText.Length)
                return "";
            return experimentSecondText.Substring(offset);
        }
    }
}