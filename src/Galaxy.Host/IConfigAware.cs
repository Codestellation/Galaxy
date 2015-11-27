using Codestellation.Galaxy.Host.ConfigManagement;

namespace Codestellation.Galaxy.Host
{
    public interface IConfigAware<TConfig>
        where TConfig : new()
    {
        ValidationResult Accept(TConfig config);
    }
}