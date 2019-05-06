using System;
using Principles;

namespace Mount
{
    public interface IMountCommand
    {
        long Id { get; }
        DateTime CreatedUtc { get; }
        bool Successful { get; }
        Exception Exception { get; }
        dynamic Result { get; }
        void Execute(Actions actions);
    }

    /// <summary>
    /// 
    /// </summary>
    public class CmdRate : IMountCommand
    {
        public long Id { get; }
        public DateTime CreatedUtc { get; }
        public bool Successful { get; private set; }
        public Exception Exception { get; private set; }
        public dynamic Result { get; }
        private readonly AxisId _axis;
        private readonly double _rate;
        public CmdRate(long id, AxisId axis, double rate)
        {
            Id = id;
            _axis = axis;
            _rate = rate;
            CreatedUtc = HiResDateTime.UtcNow;
            Successful = false;
            Result = null;
            MountQueue.AddCommand(this);
        }
        public void Execute(Actions actions)
        {
            try
            {
                actions.Rate(_axis, _rate);
                Successful = true;
            }
            catch (Exception e)
            {
                Successful = false;
                Exception = e;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CmdRateAxis : IMountCommand
    {
        public long Id { get; }
        public DateTime CreatedUtc { get; }
        public bool Successful { get; private set; }
        public Exception Exception { get; private set; }
        public dynamic Result { get; }
        private readonly AxisId _axis;
        private readonly double _rate;
        public CmdRateAxis(long id, AxisId axis, double rate)
        {
            Id = id;
            _axis = axis;
            _rate = rate;
            CreatedUtc = HiResDateTime.UtcNow;
            Successful = false;
            Result = null;
            MountQueue.AddCommand(this);
        }
        public void Execute(Actions actions)
        {
            try
            {
                actions.RateAxis(_axis, _rate);
                Successful = true;
            }
            catch (Exception e)
            {
                Successful = false;
                Exception = e;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CmdAxisSlew : IMountCommand
    {
        public long Id { get; }
        public DateTime CreatedUtc { get; }
        public bool Successful { get; private set; }
        public Exception Exception { get; private set; }
        public dynamic Result { get; }
        private readonly AxisId _axis;
        private readonly double _rate;
        public CmdAxisSlew(long id, AxisId axis, double rate)
        {
            Id = id;
            _axis = axis;
            _rate = rate;
            CreatedUtc = HiResDateTime.UtcNow;
            Successful = false;
            Result = null;
            MountQueue.AddCommand(this);
        }
        public void Execute(Actions actions)
        {
            try
            {
                actions.AxisSlew(_axis, _rate);
                Successful = true;
            }
            catch (Exception e)
            {
                Successful = false;
                Exception = e;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CmdAxesDegrees : IMountCommand
    {
        public long Id { get; }
        public DateTime CreatedUtc { get; private set; }
        public bool Successful { get; private set; }
        public Exception Exception { get; private set; }
        public dynamic Result { get; private set; }

        public CmdAxesDegrees(long id)
        {
            Id = id;
            CreatedUtc = HiResDateTime.UtcNow;
            Successful = false;
            MountQueue.AddCommand(this);
        }

        public void Execute(Actions actions)
        {
            try
            {
                Result = actions.AxesDegrees();
                Successful = true;
            }
            catch (Exception e)
            {
                Successful = false;
                Exception = e;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CmdAxisSteps : IMountCommand
    {
        public long Id { get; }
        public DateTime CreatedUtc { get; private set; }
        public bool Successful { get; private set; }
        public Exception Exception { get; private set; }
        public dynamic Result { get; private set; }

        public CmdAxisSteps(long id)
        {
            Id = id;
            CreatedUtc = HiResDateTime.UtcNow;
            Successful = false;
            MountQueue.AddCommand(this);
        }

        public void Execute(Actions actions)
        {
            try
            {
                Result = actions.AxisSteps();
                Successful = true;
            }
            catch (Exception e)
            {
                Successful = false;
                Exception = e;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CmdAxisStop : IMountCommand
    {
        public long Id { get; }
        public DateTime CreatedUtc { get; private set; }
        public bool Successful { get; private set; }
        public Exception Exception { get; private set; }
        public dynamic Result { get; private set; }
        private readonly AxisId _axis;

        public CmdAxisStop(long id, AxisId axis)
        {
            Id = id;
            _axis = axis;
            CreatedUtc = HiResDateTime.UtcNow;
            Successful = false;
            Result = null;
            MountQueue.AddCommand(this);
        }

        public void Execute(Actions actions)
        {
            try
            {
                actions.AxisStop(_axis);
                Successful = true;
            }
            catch (Exception e)
            {
                Successful = false;
                Exception = e;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CmdHcSlew : IMountCommand
    {
        public long Id { get; }
        public DateTime CreatedUtc { get; }
        public bool Successful { get; private set; }
        public Exception Exception { get; private set; }
        public dynamic Result { get; }
        private readonly AxisId _axis;
        private readonly double _rate;
        public CmdHcSlew(long id, AxisId axis, double rate)
        {
            Id = id;
            _axis = axis;
            _rate = rate;
            CreatedUtc = HiResDateTime.UtcNow;
            Successful = false;
            Result = null;
            MountQueue.AddCommand(this);
        }
        public void Execute(Actions actions)
        {
            try
            {
                actions.HcSlew(_axis, _rate);
                Successful = true;
            }
            catch (Exception e)
            {
                Successful = false;
                Exception = e;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CmdAxisTracking : IMountCommand
    {
        public long Id { get; }
        public DateTime CreatedUtc { get; }
        public bool Successful { get; private set; }
        public Exception Exception { get; private set; }
        public dynamic Result { get; }
        private readonly AxisId _axis;
        private readonly double _rate;
        public CmdAxisTracking(long id, AxisId axis, double rate)
        {
            Id = id;
            _axis = axis;
            _rate = rate;
            CreatedUtc = HiResDateTime.UtcNow;
            Successful = false;
            Result = null;
            MountQueue.AddCommand(this);
        }
        public void Execute(Actions actions)
        {
            try
            {
                actions.AxisTracking(_axis, _rate);
                Successful = true;
            }
            catch (Exception e)
            {
                Successful = false;
                Exception = e;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CmdAxisGoToTarget : IMountCommand
    {
        public long Id { get; }
        public DateTime CreatedUtc { get; }
        public bool Successful { get; private set; }
        public Exception Exception { get; private set; }
        public dynamic Result { get; }
        private readonly AxisId _axis;
        private readonly double _targetPosition;

        public CmdAxisGoToTarget(long id, AxisId axis, double targetPosition)
        {
            Id = id;
            _axis = axis;
            _targetPosition = targetPosition;
            CreatedUtc = HiResDateTime.UtcNow;
            Successful = false;
            Result = null;
            MountQueue.AddCommand(this);
        }

        public void Execute(Actions actions)
        {
            try
            {
                actions.AxisGoToTarget(_axis, _targetPosition);
                Successful = true;
            }
            catch (Exception e)
            {
                Successful = false;
                Exception = e;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CmdAxisToDegrees : IMountCommand
    {
        public long Id { get; }
        public DateTime CreatedUtc { get; }
        public bool Successful { get; private set; }
        public Exception Exception { get; private set; }
        public dynamic Result { get; }
        private readonly AxisId _axis;
        private readonly double _degrees;

        public CmdAxisToDegrees(long id, AxisId axis, double degrees)
        {
            Id = id;
            _axis = axis;
            CreatedUtc = HiResDateTime.UtcNow;
            _degrees = degrees;
            Successful = false;
            Result = null;
            MountQueue.AddCommand(this);
        }

        public void Execute(Actions actions)
        {
            try
            {
                actions.AxisToDegrees(_axis, _degrees);
                Successful = true;
            }
            catch (Exception e)
            {
                Successful = false;
                Exception = e;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CmdAxisStatus : IMountCommand
    {
        public long Id { get; }
        public DateTime CreatedUtc { get; private set; }
        public bool Successful { get; private set; }
        public Exception Exception { get; private set; }
        public dynamic Result { get; private set; }
        private readonly AxisId _axis;

        public CmdAxisStatus(long id, AxisId axis)
        {
            Id = id;
            _axis = axis;
            CreatedUtc = HiResDateTime.UtcNow;
            Successful = false;
            MountQueue.AddCommand(this);
        }

        public void Execute(Actions actions)
        {
            try
            {
                Result = actions.AxisStatus(_axis);
                Successful = true;
            }
            catch (Exception e)
            {
                Successful = false;
                Exception = e;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CmdCapabilities : IMountCommand
    {
        public long Id { get; }
        public DateTime CreatedUtc { get; private set; }
        public bool Successful { get; private set; }
        public Exception Exception { get; private set; }
        public dynamic Result { get; private set; }

        public CmdCapabilities(long id)
        {
            Id = id;
            CreatedUtc = HiResDateTime.UtcNow;
            Successful = false;
            MountQueue.AddCommand(this);
        }

        public void Execute(Actions actions)
        {
            try
            {
                Result = actions.MountInfo;
                Successful = true;
            }
            catch (Exception e)
            {
                Successful = false;
                Exception = e;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CmdAxisPulse : IMountCommand
    {
        public long Id { get; }
        public DateTime CreatedUtc { get; private set; }
        public bool Successful { get; private set; }
        public Exception Exception { get; private set; }
        public dynamic Result { get; private set; }
        private readonly AxisId _axis;
        private readonly double _guiderate;
        private readonly int _duration;

        public CmdAxisPulse(long id, AxisId axis, double guiderate, int duration)
        {
            Id = id;
            _axis = axis;
            _guiderate = guiderate;
            _duration = duration;
            CreatedUtc = HiResDateTime.UtcNow;
            Successful = false;
            Result = null;
            MountQueue.AddCommand(this);
        }

        public void Execute(Actions actions)
        {
            try
            {
                Result = actions.AxisPulse(_axis, _guiderate, _duration);
                Successful = true;
            }
            catch (Exception e)
            {
                Successful = false;
                Exception = e;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CmdDriverName : IMountCommand
    {
        public long Id { get; }
        public DateTime CreatedUtc { get; private set; }
        public bool Successful { get; private set; }
        public Exception Exception { get; private set; }
        public dynamic Result { get; private set; }

        public CmdDriverName(long id)
        {
            Id = id;
            CreatedUtc = HiResDateTime.UtcNow;
            Successful = false;
            MountQueue.AddCommand(this);
        }

        public void Execute(Actions actions)
        {
            try
            {
                Result = actions.DriverName;
                Successful = true;
            }
            catch (Exception e)
            {
                Successful = false;
                Exception = e;
            }
        }
    }








    
    
    
    
    
}
