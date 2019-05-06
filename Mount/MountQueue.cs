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
