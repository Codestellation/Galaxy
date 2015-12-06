namespace Codestellation.Galaxy.Host
{
    public interface IService
    {
        HostConfig HostConfig { get; set; }

        void Start();

        void Stop();
    }
}