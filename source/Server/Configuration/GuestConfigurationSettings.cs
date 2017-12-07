using System.Collections.Generic;
using Octopus.Node.Extensibility.Extensions.Infrastructure.Configuration;
using Octopus.Node.Extensibility.HostServices.Mapping;
using Octopus.Server.Extensibility.Authentication.Guest.GuestAuth;

namespace Octopus.Server.Extensibility.Authentication.Guest.Configuration
{
    public class GuestConfigurationSettings : ExtensionConfigurationSettings<GuestConfiguration, GuestConfigurationResource, IGuestConfigurationStore>, IGuestConfigurationSettings
    {
        private readonly IGuestUserStateChecker guestUserStateChecker;

        public GuestConfigurationSettings(
            IGuestConfigurationStore guestConfigurationStore,
            IGuestUserStateChecker guestUserStateChecker,
            IResourceMappingFactory factory) : base(guestConfigurationStore, factory)
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

        public override IEnumerable<IResourceMapping> GetMappings()
        {
            return new[]
            {
                ResourceMappingFactory.Create<GuestConfigurationResource, GuestConfiguration>()
                    .EnrichModel((resource, model, context) =>
                    {
                        guestUserStateChecker.EnsureGuestUserIsInCorrectState(resource.IsEnabled);
                    })
            };
        }
    }
}