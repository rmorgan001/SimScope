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

using System.Collections.Concurrent;
using System.Threading.Tasks;
using Principles;
using ASCOM.Utilities;

namespace Mount
{
    public static class MountQueue
    {
        #region Fields

        private static readonly BlockingCollection<IMountCommand> _commandBlockingCollection;
        private static readonly ConcurrentDictionary<long, IMountCommand> _resultsDictionary;
        private static readonly Actions _actions;

        #endregion

        #region properties

        public static Serial Serial { get; set; }        

        #endregion
        
        static MountQueue()
        {
            _actions = new Actions();
            _resultsDictionary = new ConcurrentDictionary<long, IMountCommand>();
            _commandBlockingCollection = new BlockingCollection<IMountCommand>();
            Task.Factory.StartNew(() =>
            {
                foreach (var command in _commandBlockingCollection.GetConsumingEnumerable())
                {
                    ProcessCommandQueue(command);
                }
            });
        }

        #region Queues

        /// <summary>
        /// Add a command to the blocking queue
        /// </summary>
        /// <param name="command"></param>
        public static void AddCommand(IMountCommand command)
        {
            _commandBlockingCollection.TryAdd(command);
            CleanResults(20, 60);
        }

        /// <summary>
        /// Cleans up the results dictionary
        /// </summary>
        /// <param name="count"></param>
        /// <param name="seconds"></param>
        private static void CleanResults(int count, int seconds)
        {
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
        /// <param name="id"></param>
        /// <returns></returns>
        public static IMountCommand GetCommandResult(long id)
        {
            IMountCommand result;
            while (true)
            {
                if (!_resultsDictionary.ContainsKey(id)) continue;
                var success = _resultsDictionary.TryRemove(id, out result);
                if (!success)
                {
                    //log 
                }
                break;
            }
            return result;
        }

        /// <summary>
        /// Process command queue
        /// </summary>
        /// <param name="command"></param>
        private static void ProcessCommandQueue(IMountCommand command)
        {
            command.Execute(_actions);
            if (command.Id > 0)
            {
                _resultsDictionary.TryAdd(command.Id, command);
            }
        }
        
        #endregion
    }
}
