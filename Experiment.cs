using System;

namespace KSPScienceLibrary
{
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
            if(remain > 0 || earned > 0)
            {
                return string.Format("{0} {1} {2}", fullName, Math.Round(earned, 1), Math.Round(remain, 1));
            }
            else
            {
                return string.Format("{0} - -", fullName);
            }
        }
    }
}