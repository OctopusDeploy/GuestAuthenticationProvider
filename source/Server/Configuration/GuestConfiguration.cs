
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Server.Extensibility.Authentication.Guest.Configuration
{
    class GuestConfiguration : ExtensionConfigurationDocument
    {
        public GuestConfiguration() : this("Guest", "Octopus Deploy")
        {
            
        }

        public GuestConfiguration(string name, string extensionAuthor) : base(name, extensionAuthor, "1.0")
        {
            Id = GuestConfigurationStore.SingletonId;
        }
    }
}