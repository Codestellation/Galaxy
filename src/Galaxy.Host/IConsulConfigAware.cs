namespace Codestellation.Galaxy.Host.ConfigManagement
{
    public interface IConsulConfigAware<TConfig>
        where TConfig : new()
    {
        ValidationResult Accept(TConfig config);
    }
}