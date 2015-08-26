using Codestellation.Galaxy.Host.ConfigManagement;

namespace Codestellation.Galaxy.Host
{
    public interface IConsulConfigAware<TConfig>
        where TConfig : new()
    {
        ValidationResult Accept(TConfig config);
    }
}