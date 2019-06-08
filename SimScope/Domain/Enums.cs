using System;

namespace SimServer.Domain
{
    public enum MountTaskName
    {
        AlternatingPpec,
        DecPulseToGoTo,
        Encoders,
        GetOneStepIndicators,
        LoadDefaults,
        StopAxes,
        InstantStopAxes,
        SetSouthernHemisphere,
        SyncAxes,
        SyncTarget,
        SyncAltAz,
        MonitorPulse,
        Pec,
        PecTraining,
        Capabilities,
        SetHomePositions,
        SetSt4Guiderate,
        SkySetSnapPort,
        MountName,
        GetAxisVersions,
        GetAxisStrVersions,
        MountVersion,
        StepsPerRevolution
    }

    public enum SlewType
    {
        SlewNone,
        SlewSettle,
        SlewMoveAxis,
        SlewRaDec,
        SlewAltAz,
        SlewPark,
        SlewHome,
        SlewHandpad,
        SlewComplete
    }

    [Flags]
    public enum SlewSpeed
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8
    }

    public enum SlewDirection
    {
        SlewNorth,
        SlewSouth,
        SlewEast,
        SlewWest,
        SlewUp,
        SlewDown,
        SlewLeft,
        SlewRight,
        SlewNone
    }

    public enum PointingState
    {
        Normal,
        ThroughThePole
    }

    public enum TrackingMode
    {
        Off,
        AltAz,
        EqN,
        EqS
    }
}
