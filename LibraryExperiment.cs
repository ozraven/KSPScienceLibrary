public class LibraryExperiment
{
    private readonly string biome;
    private readonly CelestialBody celestialBody;
    private readonly ScienceExperiment experiment;
    private readonly ExperimentSituations experimentSituation;
    private readonly string firstId;
//     private bool needAtmosphere;
//     private bool needOcean;
    private readonly float scienceCapacity;
    private bool needBiome;
    private ScienceSubject subject;

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
        subject = new ScienceSubject(experiment, experimentSituation, celestialBody, biome);
        if (firstId == "asteroidSample") subject.scienceCap = experiment.scienceCap;
        scienceCapacity = subject.scienceCap;
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
        get { return scienceCapacity; }
    }

    public bool ShowInLibrary { get; set; }

    public bool ShowInMonitor { get; set; }
}