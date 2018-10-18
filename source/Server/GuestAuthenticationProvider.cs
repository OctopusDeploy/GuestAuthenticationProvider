using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Authentication.Guest.Configuration;
using Octopus.Server.Extensibility.Authentication.Resources;

namespace Octopus.Server.Extensibility.Authentication.Guest 
{
    public class GuestAuthenticationProvider : IAuthenticationProvider
    {
        public const string ProviderName = "Octopus - Guest";
        readonly IGuestConfigurationStore configurationStore;

        public GuestAuthenticationProvider(IGuestConfigurationStore configurationStore)
        {
            this.configurationStore = configurationStore;
        }

        public string IdentityProviderName => ProviderName;

        public bool IsEnabled => configurationStore.GetIsEnabled();

        public bool SupportsPasswordManagement => false;

        public AuthenticationProviderElement GetAuthenticationProviderElement()
        {
            var authenticationProviderElement = new AuthenticationProviderElement
            {
                Name = IdentityProviderName,
                IdentityType = IdentityType.Guest
            };

            return authenticationProviderElement;
        }

        public string[] GetAuthenticationUrls()
        {
            return new string[0];
        }
    }
}