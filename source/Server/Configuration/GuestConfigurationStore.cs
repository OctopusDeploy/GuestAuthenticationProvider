using System.Collections.Generic;
using Octopus.Data.Storage.Configuration;
using Octopus.Node.Extensibility.Extensions.Infrastructure.Configuration;
using Octopus.Node.Extensibility.HostServices.Mapping;
using Octopus.Server.Extensibility.Authentication.Guest.GuestAuth;

namespace Octopus.Server.Extensibility.Authentication.Guest.Configuration
{
    public class GuestConfigurationStore : ExtensionConfigurationStore<GuestConfiguration, GuestConfigurationResource>, IGuestConfigurationStore
    {
        public static string SingletonId = "authentication-guest";

        readonly IGuestUserStateChecker guestUserStateChecker;

        public GuestConfigurationStore(
            IConfigurationStore configurationStore,
            IGuestUserStateChecker guestUserStateChecker,
            IResourceMappingFactory factory) : base(configurationStore, factory)
        {
            this.guestUserStateChecker = guestUserStateChecker;
        }

        public override IResourceMapping GetMapping()
        {
            return ResourceMappingFactory.Create<GuestConfigurationResource, GuestConfiguration>();
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