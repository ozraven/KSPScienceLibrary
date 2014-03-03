using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct Experiment
{
    public string fullName;
    public double earned;
    public double remain;
    public string body;
    public string type;

    public Experiment(string eFullName, double eEarned, double eRemain, string eCelestialbody, string eFirstIdType)
    {
        fullName = eFullName;
        earned = eEarned;
        remain = eRemain;
        body = eCelestialbody;
        type = eFirstIdType;
    }

    public override string ToString()
    {
        if (remain > 0 || earned > 0)
            return fullName + " " + Math.Round(earned, 2) + " " + Math.Round(remain, 2);
        return fullName + " " + "-" + " " + "-";
    }
}