using System;
using System.Linq;
using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Authentication.Guest.Configuration;
using Octopus.Server.Extensibility.Authentication.Web;

namespace Octopus.Server.Extensibility.Authentication.Guest
{
    class GuestLoginParametersHandler : ICanHandleLoginParameters
    {
        private readonly IGuestConfigurationStore guestConfigurationStore;
        const string AutoLoginParameterName = "autologin";
        const string AutoLoginParameterValue = "guest";
        
        public GuestLoginParametersHandler(IGuestConfigurationStore guestConfigurationStore)
        {
            this.guestConfigurationStore = guestConfigurationStore;
        }

        public string IdentityProviderName => GuestAuthenticationProvider.ProviderName;

        public bool? WasExternalLoginInitiated(string encodedQueryString)
        {
            if (!guestConfigurationStore.GetIsEnabled())
                return null;
            var parser = new EncodedQueryStringParser();
            var parameters = parser.Parse(encodedQueryString);
            
            var autoLoginParameter = GetAutoLoginParameterIfPresent(parameters);
            return autoLoginParameter != null;
        }

        private static EncodedQueryStringParser.QueryStringParameter? GetAutoLoginParameterIfPresent(EncodedQueryStringParser.QueryStringParameter[] parameters)
        {
            return parameters.FirstOrDefault(p =>
                p.Name.Equals(AutoLoginParameterName, StringComparison.OrdinalIgnoreCase) &&
                p.Value.Equals(AutoLoginParameterValue, StringComparison.OrdinalIgnoreCase));
        }
    }
}