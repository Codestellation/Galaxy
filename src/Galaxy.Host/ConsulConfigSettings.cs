namespace Codestellation.Galaxy.Host
{
    public class ConsulConfigSettings
    {
        /// <summary>
        /// Root path for service config
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Http addres of consul host {host:port}. Used default if not set
        /// </summary>
        public string Address { get; set; }
    }
}