using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Server.Extensibility.Authentication.Guest.Configuration
{
    public interface IGuestConfigurationStore : IExtensionConfigurationStore<GuestConfiguration>
    {
    }
}