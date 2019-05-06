using System;
using System.Collections.Concurrent;
using System.Threading;
using ASCOM.Utilities;

namespace SimServer
{
    public static class Connection
    {
        #region Fields

        //Setup defaults for number of com connections
        private static readonly ConcurrentDictionary<long, bool> ConnectStates;
        private static long _idCount;

        #endregion

        static Connection()
        {
            ConnectStates = new ConcurrentDictionary<long, bool>();
            _idCount = 0;
        }

        #region Properties

        public static Serial Serial { get; private set; }

        /// <summary>
        /// show one or more connections
        /// </summary>
        public static bool Connected => ConnectStates.Count > 0;

        #endregion

        #region Methods

        public static bool IsConnected()
        {
            return true;
        }

        /// <summary>
        /// add to the count of connections
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        public static void SetConnected(long id, bool value)
        {
            // add or remove the instance, this is done once regardless of the number of calls
            if (value)
            {
                var _ = ConnectStates.TryAdd(id, true);
            }
            else
            {
                var _ = ConnectStates.TryRemove(id, out value);
            }
        }

        /// <summary>
        /// Close Connected 
        /// </summary>
        internal static void CloseConnected()
        {
            if (ConnectStates.Count <= 0) return;
            foreach (var cons in ConnectStates)
            {
                SetConnected(cons.Key, false);
            }
        }

        /// <summary>
        /// Serial connection
        /// </summary>
        internal static bool ConnectSerial
        {
            get => Serial != null && Serial.Connected;
            set
            {
                if (value)
                {
                    try
                    {
                        if (Serial != null)
                        {
                            if (Serial.Connected)
                            {
                                Serial.Connected = false;
                            }
                            Serial.Dispose();
                            Serial = null;
                        }

                        Serial = new Serial
                        {
                            PortName = Settings.ComPort,
                            Speed = Settings.BaudRate,
                            StopBits = SerialStopBits.One,
                            DataBits = Settings.DataBit,
                            DTREnable = Settings.DTREnable,
                            RTSEnable = Settings.RTSEnable,
                            Handshake = Settings.HandShake,
                            Parity = SerialParity.None,
                            Connected = true
                        };
                    }
                    catch (Exception)
                    {
                        if (Serial != null)
                        {
                            Serial.Connected = false;
                        }
                        throw;
                    }

                }
                else
                {
                    if (Serial == null) return;
                    if (Serial.Connected)
                    {
                        Serial.Connected = false;
                    }
                    Serial.Dispose();
                }
            }
        }

        /// <summary>
        /// Increments the number when an application creates and instance of telescope driver
        /// </summary>
        /// <returns></returns>
        public static long GetConnId()
        {
            return Interlocked.Increment(ref _idCount); // Increment the counter in a threadsafe fashion
        }

        #endregion
    }
}
