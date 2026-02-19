namespace EntropySyndicate.Data
{
    public enum GameFlowState
    {
        Boot,
        Home,
        Run,
        RunPaused,
        UpgradeChoice,
        GameOver,
        Shards,
        Forge,
        Missions,
        Shop,
        Settings,
        Debug
    }

    public enum CurrencyType
    {
        Scrap,
        Sigils,
        Prisms
    }

    public enum ShardType
    {
        GravityWarp,
        MomentumLock,
        RepulsionPulse,
        TimeShear,
        EnergyDrainField,
        DuplicationEcho,
        VectorRedirect,
        ChaosAmplifier,
        OrbitSnare,
        PhaseAnchor,
        FrictionBloom,
        PulseMine
    }

    public enum MissionCadence
    {
        Daily,
        Weekly
    }

    public enum AdPlacement
    {
        EndRunDoubleReward,
        InstantEnergyRefill,
        Revive
    }
}
