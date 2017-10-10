﻿using System.Collections.Generic;
using Octopus.Data.Storage.Configuration;
using Octopus.Node.Extensibility.Extensions.Infrastructure.Configuration;
using Octopus.Server.Extensibility.Authentication.Guest.GuestAuth;

namespace Octopus.Server.Extensibility.Authentication.Guest.Configuration
{
    public class GuestConfigurationStore : ExtensionConfigurationStore<GuestConfiguration, GuestConfiguration>, IGuestConfigurationStore
    {
        public static string SingletonId = "authentication-guest";

        readonly IGuestUserStateChecker guestUserStateChecker;

        public GuestConfigurationStore(
            IConfigurationStore configurationStore,
            IGuestUserStateChecker guestUserStateChecker) : base(configurationStore)
        {
            this.guestUserStateChecker = guestUserStateChecker;
        }

        protected override GuestConfiguration MapToResource(GuestConfiguration doc)
        {
            return doc;
        }

        protected override GuestConfiguration MapFromResource(GuestConfiguration resource)
        {
            return resource;
        }

        public override void SetIsEnabled(bool isEnabled)
        {
            base.SetIsEnabled(isEnabled);
            guestUserStateChecker.EnsureGuestUserIsInCorrectState(isEnabled);
        }

        public override string Id => SingletonId;

        public override string ConfigurationSetName => "Guest Login";

        public override string Description => "Guest login authentication settings";

        public override IEnumerable<ConfigurationValue> GetConfigurationValues()
        {
            yield return new ConfigurationValue("Octopus.WebPortal.GuestLoginEnabled", GetIsEnabled().ToString(), GetIsEnabled(), "Is Enabled");
        }
    }
}