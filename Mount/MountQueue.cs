/* SimScope - ASCOM Telescope Control Simulator Copyright (c) 2019 Robert Morgan
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

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Principles;
using ASCOM.Utilities;

namespace Mount
{
    public static class MountQueue
    {
        #region Fields

        private static BlockingCollection<IMountCommand> _commandBlockingCollection;
        private static ConcurrentDictionary<long, IMountCommand> _resultsDictionary;
        private static Actions _actions;
        private static CancellationTokenSource _cts;

        #endregion

        #region properties

        public static Serial Serial { get; set; }
        public static bool IsRunning { get; private set; }
        private static long _id;
        public static long NewId => Interlocked.Increment(ref _id);

        #endregion

        #region Queues

        /// <summary>
        /// Add a command to the blocking queue
        /// </summary>
        /// <param name="command"></param>
        public static void AddCommand(IMountCommand command)
        {
            if (!IsRunning) return;
            CleanResults(20, 120);
            _commandBlockingCollection.TryAdd(command);
        }

        /// <summary>
        /// Cleans up the results dictionary
        /// </summary>
        /// <param name="count"></param>
        /// <param name="seconds"></param>
        private static void CleanResults(int count, int seconds)
        {
            if (!IsRunning) return;
            if (_resultsDictionary.IsEmpty) return;
            var recordscount = _resultsDictionary.Count;
            if (recordscount == 0) return;
            if (count == 0 && seconds == 0)
            {
                _resultsDictionary.Clear();
                return;
            }

            if (recordscount < count) return;
            var now = HiResDateTime.UtcNow;
            foreach (var result in _resultsDictionary)
            {
                if (result.Value.CreatedUtc.AddSeconds(seconds) >= now) continue;
                _resultsDictionary.TryRemove(result.Key, out _);
            }
        }

        /// <summary>
        /// Mount data results
        /// </summary>
        /// <returns></returns>
        public static IMountCommand GetCommandResult(IMountCommand command)
        {
            if (!IsRunning || _cts.IsCancellationRequested)
            {
                var e = new MountException(ErrorCode.ErrQueueFailed, "Queue not running");
                command.Exception = e;
                command.Successful = false;
                return command;
            }
            var sw = Stopwatch.StartNew();
            while (sw.Elapsed.TotalMilliseconds < 5000)
            {
                if (_resultsDictionary == null) break;
                var success = _resultsDictionary.TryRemove(command.Id, out var result);
                if (!success)
                {
                    Thread.Sleep(1);
                    continue;
                }
                sw.Stop();
                return result;
            }
            sw.Stop();
            var ex = new MountException(ErrorCode.ErrQueueFailed, "Queue Read Timeout");
            command.Exception = ex;
            command.Successful = false;
            return command;
        }

        /// <summary>
        /// Process command queue
        /// </summary>
        /// <param name="command"></param>
        private static void ProcessCommandQueue(IMountCommand command)
        {
            try
            {
                if (!IsRunning || _cts.IsCancellationRequested || !Actions.IsConnected) return;
                command.Execute(_actions);
                if (command.Id > 0)
                {
                    _resultsDictionary.TryAdd(command.Id, command);
                }
            }
            catch (Exception e)
            {
                command.Exception = e;
                command.Successful = false;
            }

            command.Execute(_actions);
            if (command.Id > 0)
            {
                _resultsDictionary.TryAdd(command.Id, command);
            }
        }

        public static void Start()
        {
            Stop();
            if (_cts == null) _cts = new CancellationTokenSource();
            var ct = _cts.Token;

            _actions = new Actions();
            _actions.InitializeAxes();
            _resultsDictionary = new ConcurrentDictionary<long, IMountCommand>();
            _commandBlockingCollection = new BlockingCollection<IMountCommand>();
            IsRunning = true;

            Task.Factory.StartNew(() =>
            {
                while (!ct.IsCancellationRequested)
                {
                    foreach (var command in _commandBlockingCollection.GetConsumingEnumerable())
                    {
                        ProcessCommandQueue(command);
                    }
                }
            }, ct);
        }

        public static void Stop()
        {
            _actions?.Shutdown();
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            IsRunning = false;
            _resultsDictionary = null;
            _commandBlockingCollection = null;
        }


        #endregion
    }
}
