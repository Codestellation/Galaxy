namespace Codestellation.Quarks.IO
{
    /// <summary>
    /// Contains settings for thread-pool
    /// </summary>
    internal struct PoolSizeSettings
    {
        /// <summary>
        /// Settings used by default
        /// </summary>
        public static PoolSizeSettings Default = new PoolSizeSettings(20, 1024);
        
        /// <summary>
        /// Number of streams in pool
        /// </summary>
        public readonly byte PoolSize;
        
        /// <summary>
        /// Initial stream size
        /// </summary>
        public readonly int StreamSize;

        /// <summary>
        /// Initialize new instance of <see cref="PoolSizeSettings"/>
        /// </summary>
        public PoolSizeSettings(byte poolSize, int streamSize)
        {
            PoolSize = poolSize;
            StreamSize = streamSize;
        }
    }
}