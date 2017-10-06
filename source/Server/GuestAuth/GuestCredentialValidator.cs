﻿using System;
using System.Threading;
using Octopus.Data.Model.User;
using Octopus.Data.Storage.User;
using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Authentication.Guest.Configuration;
using Octopus.Node.Extensibility.Authentication.Storage.User;

namespace Octopus.Server.Extensibility.Authentication.Guest.GuestAuth
{
    public class GuestCredentialValidator : IGuestCredentialValidator
    {
        readonly ILog log;
        readonly IUserStore userStore;
        readonly IGuestConfigurationStore configurationStore;

        public GuestCredentialValidator(
            ILog log,
            IUserStore userStore,
            IGuestConfigurationStore configurationStore)
        {
            this.log = log;
            this.userStore = userStore;
            this.configurationStore = configurationStore;
        }

        public int Priority => 1;

        public AuthenticationUserCreateResult ValidateCredentials(string username, string password, CancellationToken cancellationToken)
        {
            if ((!configurationStore.GetIsEnabled() || !string.Equals(username, User.GuestLogin, StringComparison.InvariantCultureIgnoreCase)))
                return new AuthenticationUserCreateResult();

            var user = userStore.GetByUsername(username);
            var messageText = "Error retrieving Guest user details";

            if (user != null && user.IsActive)
            {
                return new AuthenticationUserCreateResult(user);
            }
            else if (user == null)
            {
                messageText = "Guest login is enabled, but the guest user acccount could not be found so the login request was rejected. Please restart the Octopus server.";
            }
            else if (user.IsActive == false)
            {
                messageText = "Guest login is enabled, but the guest acccount is disabled so the login request was rejected. Please re-enable the guest account if you want guest logins to work.";
            }

            log.Warn(messageText);
            return new AuthenticationUserCreateResult(messageText);
        }
    }
}