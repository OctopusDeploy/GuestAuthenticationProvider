using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Server.Extensibility.Authentication.Guest.Configuration
{
    interface IGuestConfigurationStore : IExtensionConfigurationStore<GuestConfiguration>
    {
    }
}