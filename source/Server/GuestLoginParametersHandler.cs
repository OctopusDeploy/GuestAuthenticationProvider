using System;
using System.Linq;
using Octopus.CoreUtilities;
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
        
        public Maybe<LoginInitiatedResult> WasExternalLoginInitiated(string encodedQueryString)
        {
            if (!guestConfigurationStore.GetIsEnabled())
                return Maybe<LoginInitiatedResult>.None;
            var parser = new EncodedQueryStringParser();
            var parameters = parser.Parse(encodedQueryString);
            
            var autoLoginParameter = GetAutoLoginParameterIfPresent(parameters);
            
            return autoLoginParameter.SelectValueOr(
                selector: LoginAsGuest,
                ifNone: Maybe<LoginInitiatedResult>.None);
        }

        private static Maybe<LoginInitiatedResult> LoginAsGuest(EncodedQueryStringParser.QueryStringParameter arg)
        {
            return new LoginInitiatedResult(GuestAuthenticationProvider.ProviderName).AsSome();
        }

        private static Maybe<EncodedQueryStringParser.QueryStringParameter> GetAutoLoginParameterIfPresent(EncodedQueryStringParser.QueryStringParameter[] parameters)
        {
            return parameters.FirstOrDefault(p =>
                p.Name.Equals(AutoLoginParameterName, StringComparison.OrdinalIgnoreCase) &&
                p.Value.Equals(AutoLoginParameterValue, StringComparison.OrdinalIgnoreCase)).ToMaybe();
        }
    }
}