﻿/* SimScope - ASCOM Telescope Control Simulator Copyright (c) 2019 Robert Morgan
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

namespace Mount
{
    internal class IOSerial
    {
        private readonly Controllers _controllers; 
        
        internal IOSerial()
        {
            _controllers = new Controllers();
        }

        internal string Send(string command)
        {
            //if (Queues.Serial.Connected) return null; 
            var received = _controllers.Command(command.ToLower().Trim());
            return received;
        }

        /*  Sample Serial Code....

        #region SerialIO

        private const int _threadLockTimeout = 50; // milliseconds
        private const char _endChar = (char)13;

        /// <summary>
        /// One communication between mount and client
        /// </summary>
        /// <param name="axis">AxisId.Axis1 or AxisId.Axis2</param>
        /// <param name="command">The comamnd char set</param>
        /// <param name="cmdDataStr">The data need to send</param>
        /// <returns>The response string from mount</returns>
        private string CmdToAxis(AxisId axis, char command, string cmdDataStr)
        {
            for (var i = 0; i < 5; i++)
            {
                var acquiredLock = false;
                try
                {
                    Monitor.TryEnter(MountQueue.Serial, _threadLockTimeout, ref acquiredLock);
                    if (acquiredLock)
                    {
                        // Code that accesses resources that are protected by the lock.
                        try
                        {
                            MountQueue.Serial.ClearBuffers();
                            // send the request
                            cmdDataStr = SendRequest(axis, command, cmdDataStr);
                            // receive the response
                            var responseString = RecieveResponse(axis, command, cmdDataStr);
                            return responseString;
                        }
                        catch (TimeoutException ex)
                        {
                            throw axis == AxisId.Axis1
                                ? new MountException(ErrorCode.ErrNoresponseAxis1, "Timeout", ex)
                                : new MountException(ErrorCode.ErrNoresponseAxis2, "Timeout", ex);
                        }
                        catch (IOException ex)
                        {
                            //log

                            throw new MountException(ErrorCode.ErrNotConnected, "IO Error", ex);
                        }
                        catch (Exception)
                        {
                            //log
                            throw;
                        }
                    }
                    else
                    {
                        // deal with the fact that the lock was not acquired.
                    }
                }
                finally
                {
                    if (acquiredLock) Monitor.Exit(MountQueue.Serial);
                }
                Thread.Sleep(3);
            }
            // deal with the fact that the lock was not acquired.
            return null;
        }

        /// <summary>
        /// Builds the command string
        /// </summary>
        /// <param name="axis">AxisId.Axis1 or AxisId.Axis2</param>
        /// <param name="command"></param>
        /// <param name="cmdDataStr"></param>
        private string SendRequest(AxisId axis, char command, string cmdDataStr)
        {
            const char startCharOut = ':';
            if (cmdDataStr == null) cmdDataStr = "";
            const int bufferSize = 20;
            var commandStr = new StringBuilder(bufferSize);
            commandStr.Append(startCharOut);                    // 0: Leading char
            commandStr.Append(command);                         // 1: Length of command( Source, distination, command char, data )
            // Target Device
            commandStr.Append(axis == AxisId.Axis1 ? '1' : '2');// 2: Target Axis
            // Copy command data to buffer
            commandStr.Append(cmdDataStr);
            commandStr.Append(_endChar);                         // CR Character            

            MountQueue.Serial.Transmit(commandStr.ToString());
            return $"{commandStr}";
        }

        /// <summary>
        /// Constructs a string from the responce
        /// </summary>
        /// <returns></returns>
        private string RecieveResponse(AxisId axis, char command, string cmdDataStr)
        {
            var sw = new Stopwatch();
            sw.Start();
            while (sw.Elapsed.TotalMilliseconds < MountQueue.Serial.ReceiveTimeoutMs)
            {
                var receivedData = MountQueue.Serial.ReceiveTerminated("\r");
                if (receivedData.Length <= 0) continue;

                receivedData = receivedData.Replace("\0", string.Empty).Trim();

                switch (receivedData[0].ToString())
                {
                    //receive '= DDDDDD [0D]'    or '! D [0D]'
                    case "=":  // Normal response
                        sw.Stop();
                        return receivedData;
                    case "!":  // Abnormal response.
                        string errormsg;
                        switch (receivedData.Trim())
                        {
                            case "!0":
                                errormsg = "Invalid Command: Command doesnt apply to the model";
                                break;
                        }
                        //log and do something about errors
                        sw.Stop();
                        return receivedData;
                }
                Thread.Sleep(1);
            }
            sw.Stop();
            throw new TimeoutException();
        }

        /// <summary>
        /// Sends :e1 to the mounts and evaluates responce to see its an appropriate response.
        /// </summary>
        internal void TestSerial()
        {
            var iserror = true;
            MountQueue.Serial.ClearBuffers();
            // send the request
            SendRequest(AxisId.Axis1, 'e', null);
            // receive the response
            var responseString = MountQueue.Serial.ReceiveCounted(8);

            if (responseString.Length > 0)
            {
                responseString = responseString.Replace("\0", string.Empty).Trim();
                // check to see if the response is valid 
                switch (responseString[0].ToString())
                {
                    case "=":
                        iserror = false;
                        break;
                    case "!":
                        iserror = false;
                        break;
                }
                // check to see if the number for the mount type is valid
                if (!iserror)
                {
                    var parsed = int.TryParse(responseString.Substring(6, 1), out var mountnumber);
                    if (parsed)
                    {
                        if (mountnumber < 0 || mountnumber > 6) iserror = true;
                    }
                    else
                    {
                        iserror = true;
                    }
                }

            }

            if (!iserror) return;
            throw new MountException(ErrorCode.ErrMountNotFound);
        }

        /// <summary>
        /// Converts the string to a long
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private long StringToLong(string str)
        {
            try
            {
                long value = 0;
                for (var i = 1; i + 1 < str.Length; i += 2)
                {
                    value += (long)(int.Parse(str.Substring(i, 2), System.Globalization.NumberStyles.AllowHexSpecifier) * Math.Pow(16, i - 1));
                }
                return value;
            }
            catch (FormatException e)
            {
               // AxisStop(AxisId.Axis1);
               // AxisStop(AxisId.Axis2);
                throw new MountException(ErrorCode.ErrInvalidData, "Response Parse Error: " + str, e);
            }
        }

        /// <summary>
        /// Converts a long to Hex command
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static string LongToHex(long number)
        {
            // 31 -> 0F0000
            var a = ((int)number & 0xFF).ToString("X").ToUpper();
            var b = (((int)number & 0xFF00) / 256).ToString("X").ToUpper();
            var c = (((int)number & 0xFF0000) / 256 / 256).ToString("X").ToUpper();

            if (a.Length == 1)
                a = "0" + a;
            if (b.Length == 1)
                b = "0" + b;
            if (c.Length == 1)
                c = "0" + c;
            return a + b + c;
        }
        
        #endregion
        */
    }
}
