using System;
using System.Threading;
using Octopus.Data.Model.User;
using Octopus.Data.Storage.User;
using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Authentication.Guest.Configuration;
using Octopus.Server.Extensibility.Results;

namespace Octopus.Server.Extensibility.Authentication.Guest.GuestAuth
{
    class GuestCredentialValidator : IGuestCredentialValidator
    {
        readonly ISystemLog log;
        readonly IUserStore userStore;
        readonly IGuestConfigurationStore configurationStore;

        public GuestCredentialValidator(
            ISystemLog log,
            IUserStore userStore,
            IGuestConfigurationStore configurationStore)
        {
            this.log = log;
            this.userStore = userStore;
            this.configurationStore = configurationStore;
        }

        public string IdentityProviderName => GuestAuthenticationProvider.ProviderName;

        public int Priority => 1;

        public IResultFromExtension<IUser> ValidateCredentials(string username, string password, CancellationToken cancellationToken)
        {
            if ((!configurationStore.GetIsEnabled() || !string.Equals(username, User.GuestLogin, StringComparison.OrdinalIgnoreCase)))
                return ResultFromExtension<IUser>.ExtensionDisabled();

            var user = userStore.GetByUsername(username);
            var messageText = "Error retrieving Guest user details";

            if (user != null && user.IsActive)
            {
                return ResultFromExtension<IUser>.Success(user);
            }
            else if (user == null)
            {
                messageText = "Guest login is enabled, but the guest user account could not be found so the login request was rejected. Please restart the Octopus server.";
            }
            else if (user.IsActive == false)
            {
                messageText = "Guest login is enabled, but the guest account is disabled so the login request was rejected. Please re-enable the guest account if you want guest logins to work.";
            }

            log.Warn(messageText);
            return ResultFromExtension<IUser>.Failed(messageText);
        }
    }
}