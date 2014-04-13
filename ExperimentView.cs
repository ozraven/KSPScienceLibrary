using System;
using UnityEngine;

internal class ExperimentView : IEquatable<ExperimentView>, IComparable<ExperimentView>
{
    private readonly float _earnedScience;
    private readonly string _fullExperimentId;
    private readonly float _fullScience;
    private readonly bool _onShip;

    //     public ExperimentView(string fullExperimentId, bool onShip, float earnedScience, float fullScience)
    //     {
    //         _fullExperimentId = fullExperimentId;
    //         _onShip = onShip;
    //         _earnedScience = earnedScience;
    //         _fullScience = fullScience;
    //     }

    public ExperimentView(ScienceSubject scienceSubject, bool onShip = false)
    {
        _fullExperimentId = scienceSubject.id;
        _onShip = onShip;
        _earnedScience = scienceSubject.science;
        _fullScience = scienceSubject.scienceCap;
    }

    public ExperimentView(ScienceData scienceData, bool onShip = true)
    {
        ScienceSubject scienceSubject = ResearchAndDevelopment.GetSubjectByID(scienceData.subjectID);
        _fullExperimentId = scienceData.subjectID;
        _onShip = onShip;
        _earnedScience = scienceData.dataAmount;
        _fullScience = scienceSubject.scienceCap;
    }

    public string FullExperimentId
    {
        get { return _fullExperimentId; }
    }

    public bool OnShip
    {
        get { return _onShip; }
    }

    public float EarnedScience
    {
        get { return _earnedScience; }
    }

    public float FullScience
    {
        get { return _fullScience; }
    }


    public int CompareTo(ExperimentView other)
    {
        MonoBehaviour.print("CompareTo");
        string[] splitx = _fullExperimentId.Split('@');
        string[] splity = other._fullExperimentId.Split('@');

        if (splitx[0] == splity[0])
        {
            if (_onShip && !other._onShip)
                return 1;
            if (other._onShip && !_onShip)
                return -1;
            return 0;
        }
        return splitx[0].CompareTo(splity[0]);
    }

    public bool Equals(ExperimentView other)
    {
        if (other == null) return false;
        if (_onShip != other._onShip) return false;
        if (other._fullExperimentId == _fullExperimentId)
        {
            if (!_onShip == !other._onShip)
            {
                return true;
            }
        }
        return false;
    }
}