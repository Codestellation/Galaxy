using System.IO;
using System.Linq;

namespace Codestellation.Quarks.IO
{
    /// <summary>
    /// Encapsulates functionality for pooling streams. 
    /// </summary>
    internal class StreamPool
    {
        private readonly PoolSizeSettings _settings;
        private readonly PooledMemoryStream[] _pooledStreams;

        /// <summary>
        /// Initialize new instance of <see cref="StreamPool"/> with default <see cref="PoolSizeSettings"/>
        /// </summary>
        public StreamPool() : this(PoolSizeSettings.Default)
        {
            
        }
        /// <summary>
        /// Initialize new instance of <see cref="StreamPool"/> with passed <see cref="PoolSizeSettings"/>
        /// </summary>
        public StreamPool(PoolSizeSettings settings)
        {
            _settings = settings;
            var streams = Enumerable.Range(0, _settings.PoolSize).Select(x => new PooledMemoryStream(_settings.StreamSize));
            _pooledStreams = streams.ToArray();
        }
        
        /// <summary>
        /// Gets stream from the pool or creates new one. New stream will not be return to pool.
        /// </summary>
        public MemoryStream GetStream()
        {
            for (int index = 0; index < _pooledStreams.Length; index++)
            {
                var candidate = _pooledStreams[index];
                if (candidate.TryOwn())
                {
                    return candidate;
                }
            }
            return new MemoryStream(_settings.StreamSize);
        }
    }
}