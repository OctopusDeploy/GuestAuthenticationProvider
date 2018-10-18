using System.Collections.Generic;
using Octopus.Server.Extensibility.Authentication.Guest.GuestAuth;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;
using Octopus.Server.Extensibility.HostServices.Mapping;

namespace Octopus.Server.Extensibility.Authentication.Guest.Configuration
{
    public class GuestConfigurationSettings : ExtensionConfigurationSettings<GuestConfiguration, GuestConfigurationResource, IGuestConfigurationStore>, IGuestConfigurationSettings
    {
        private readonly IGuestUserStateChecker guestUserStateChecker;

        public GuestConfigurationSettings(
            IGuestConfigurationStore guestConfigurationStore,
            IGuestUserStateChecker guestUserStateChecker) : base(guestConfigurationStore)
        {
            this.guestUserStateChecker = guestUserStateChecker;
        }

        public override string Id => GuestConfigurationStore.SingletonId;

        public override string ConfigurationSetName => "Guest Login";

        public override string Description => "Guest login authentication settings";

        public override IEnumerable<ConfigurationValue> GetConfigurationValues()
        {
            yield return new ConfigurationValue("Octopus.WebPortal.GuestLoginEnabled", ConfigurationDocumentStore.GetIsEnabled().ToString(), ConfigurationDocumentStore.GetIsEnabled(), "Is Enabled");
        }

        public override void BuildMappings(IResourceMappingsBuilder builder)
        {
            builder.Map<GuestConfigurationResource, GuestConfiguration>()
                .EnrichModel((model, resource) =>
                {
                    guestUserStateChecker.EnsureGuestUserIsInCorrectState(resource.IsEnabled);
                });
        }
    }
}