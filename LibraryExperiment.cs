using System;

public class LibraryExperiment
{
    private readonly string biome;
    private readonly CelestialBody celestialBody;
    private readonly ScienceExperiment experiment;
    private readonly ExperimentSituations experimentSituation;
    private readonly string firstId;
//     private bool needAtmosphere;
//     private bool needOcean;

    private readonly ScienceSubject subject;

    private bool needBiome;

    public LibraryExperiment(string firstId, ExperimentSituations experimentSituation, CelestialBody celestialBody, ScienceExperiment experiment, bool shouldHaveBiome, string biome)
    {
        ShowInLibrary = true;
        ShowInMonitor = true;
        this.celestialBody = celestialBody;
        this.experimentSituation = experimentSituation;
        this.biome = biome;
        needBiome = shouldHaveBiome;
        this.firstId = firstId;
        this.experiment = experiment;
        //subject = new ScienceSubject(experiment, experimentSituation, celestialBody, biome);
        bool foundSubject;
        subject = LibraryUtils.GetExperimentSubject(experiment, experimentSituation, celestialBody, biome, out foundSubject);
        if (firstId == "asteroidSample") subject.scienceCap = experiment.scienceCap;
    }

    public ScienceSubject Subject
    {
        get { return subject; }
    }

    public ExperimentSituations ExperimentSituation
    {
        get { return experimentSituation; }
    }

    public string Biome
    {
        get { return biome; }
    }

    public CelestialBody CelestialBody
    {
        get { return celestialBody; }
    }

    public ScienceExperiment ScienceExperiment
    {
        get { return experiment; }
    }

    public string FirstId
    {
        get { return firstId; }
    }

    public float ScienceCapacity
    {
        get { return (float) Math.Round(subject.scienceCap, 1); }
    }

    public bool ShowInLibrary { get; set; }

    public bool ShowInMonitor { get; set; }


    public float Earned
    {
        get { return (float) Math.Round(subject.science, 1); }
    }

    public float EarnedPercent
    {
        get { return (float) Math.Round(100.0*subject.science/subject.scienceCap, 0); }
    }

    public float Remain
    {
        get { return (float) Math.Round(subject.scienceCap - subject.science, 1); }
    }

    public float RemainPercent
    {
        get { return (float) Math.Round(100.0*(subject.scienceCap - subject.science)/subject.scienceCap, 0); }
    }
}