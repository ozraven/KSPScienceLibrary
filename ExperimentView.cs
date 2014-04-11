using System;

internal class ExperimentView : IEquatable<ExperimentView>
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
        //var subject = ResearchAndDevelopment.GetSubjectByID(scienceData.subjectID);
        _fullExperimentId = scienceData.subjectID;
        _onShip = onShip;
        _earnedScience = 0;
        _fullScience = 0; //subject.scienceCap
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

    public bool Equals(ExperimentView other)
    {
        if (other == null) return false;
        if (_onShip || other._onShip) return false;
        return other._fullExperimentId == _fullExperimentId && other._onShip == _onShip;
    }
}