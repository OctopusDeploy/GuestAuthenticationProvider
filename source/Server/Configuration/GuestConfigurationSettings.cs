using System.Collections.Generic;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;
using Octopus.Server.Extensibility.HostServices.Mapping;

namespace Octopus.Server.Extensibility.Authentication.Guest.Configuration
{
    class GuestConfigurationSettings : ExtensionConfigurationSettings<GuestConfiguration, GuestConfigurationResource, IGuestConfigurationStore>, IGuestConfigurationSettings
    {
        public GuestConfigurationSettings(
            IGuestConfigurationStore guestConfigurationStore) : base(guestConfigurationStore)
        {
        }

        public override string Id => GuestConfigurationStore.SingletonId;

        public override string ConfigurationSetName => "Guest Login";

        public override string Description => "Guest login authentication settings";

        public override IEnumerable<IConfigurationValue> GetConfigurationValues()
        {
            yield return new ConfigurationValue<bool>("Octopus.WebPortal.GuestLoginEnabled", ConfigurationDocumentStore.GetIsEnabled(), ConfigurationDocumentStore.GetIsEnabled(), "Is Enabled");
        }

        public override void BuildMappings(IResourceMappingsBuilder builder)
        {
            builder.Map<GuestConfigurationResource, GuestConfiguration>();
        }
    }
}