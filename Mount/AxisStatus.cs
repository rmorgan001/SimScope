
namespace Mount
{
    public struct AxisStatus
    {
        public AxisId Axis { get; set; }
        public bool Slewing { get; set; }
        public bool Stopped { get; set; }
        public bool Tracking { get; set; }
        public bool Rate { get; set; }
    }
}
