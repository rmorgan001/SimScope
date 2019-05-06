namespace Mount
{

    public enum AxisId { Axis1 = 0, Axis2 = 1 };

    public enum ErrorCode
    {
        ErrInvalidId = 1,			    // Invalid mount ID
        ErrAlreadyConnected = 2,	    // Already connected to another mount ID
        ErrNotConnected = 3,		    // Telescope not connected.
        ErrInvalidData = 4, 		    // Invalid data, over range etc
        ErrSerialPortBusy = 5, 	        // Serial port is busy.
        ErrMountNotFound = 6,           // Serial test command did not get correct response
        ErrNoresponseAxis1 = 100,	    // No response from axis1
        ErrNoresponseAxis2 = 101,	    // The secondary axis of the telescope does not respond
        ErrAxisBusy = 102,			    // This operation cannot be performed temporarily
        ErrMaxPitch = 103,              // Target position elevation angle is too high
        ErrMinPitch = 104,			    // Target position elevation angle is too low
        ErrUserInterrupt = 105,	        // User forced termination
        ErrAlignFailed = 200,		    // Calibration telescope failed
        ErrUnimplement = 300,           // Unimplemented method
        ErrWrongAlignmentData = 400,	// The alignment data is incorect.
    };

    public enum Mountid
    {
        // Telescope ID, they must be started from 0 and coded continuously.
        IdCelestronAz = 0,              // Celestron Alt/Az Mount
        IdCelestronEq = 1,              // Celestron EQ Mount
        IdSkywatcherAz = 2,             // Skywatcher Alt/Az Mount
        IdSkywatcherEq = 3,             // Skywatcher EQ Mount
        IdOrionEqg = 4,                 // Orion EQ Mount
        IdOrionTeletrack = 5,           // Orion TeleTrack Mount
        IdEqEmulator = 6,               // EQ Mount Emulator
        IdAzEmulator = 7,               // Alt/Az Mount Emulator
        IdNexstargt80 = 8,              // NexStarGT-80 mount
        IdNexstargt114 = 9,             // NexStarGT-114 mount
        IdStarseeker80 = 10,                // NexStarGT-80 mount
        IdStarseeker114 = 11,			// NexStarGT-114 mount
    }
}
