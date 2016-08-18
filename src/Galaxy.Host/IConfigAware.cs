using Codestellation.Galaxy.Host.ConfigManagement;

namespace Codestellation.Galaxy.Host
{
    public interface IConfigAware<TConfig>
        where TConfig : new()
    {
        bool CanGetSample { get; }

        TConfig GetSample();

        ValidationResult Accept(TConfig config);
    }
}