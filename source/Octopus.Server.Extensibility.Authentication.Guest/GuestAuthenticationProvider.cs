﻿using Octopus.Node.Extensibility.Authentication.Extensions;
using Octopus.Node.Extensibility.Authentication.Resources;
using Octopus.Server.Extensibility.Authentication.Guest.Configuration;

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
                IsGuestProvider = true,
            };

            return authenticationProviderElement;
        }

        public string[] GetAuthenticationUrls()
        {
            return new string[0];
        }
    }
}