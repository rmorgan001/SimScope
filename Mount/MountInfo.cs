namespace Mount
{
    public struct MountInfo
    {
        public bool CanAxisSlewsIndependent { get; set; }
        public bool CanAzEq { get; set; }
        public bool CanDualEncoders { get; set; }
        public bool CanHalfTrack { get; set; }
        public bool CanHomeSensors { get; set; }
        public bool CanPolarLed { get; set; }
        public bool CanPpec { get; set; }
        public bool CanWifi { get; set; }
        public string MountName { get; set; }
        public string MountVersion { get; set; }
    }
}
