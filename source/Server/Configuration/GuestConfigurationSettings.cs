using System.Collections.Generic;
using Octopus.Node.Extensibility.Extensions.Infrastructure.Configuration;
using Octopus.Node.Extensibility.HostServices.Mapping;

namespace Octopus.Server.Extensibility.Authentication.Guest.Configuration
{
    public class GuestConfigurationSettings : ExtensionConfigurationSettings<GuestConfiguration, GuestConfigurationResource, IGuestConfigurationStore>, IGuestConfigurationSettings
    {
        public GuestConfigurationSettings(
            IGuestConfigurationStore guestConfigurationStore,
            IResourceMappingFactory factory) : base(guestConfigurationStore, factory)
        {
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
            return new[] { ResourceMappingFactory.Create<GuestConfigurationResource, GuestConfiguration>() };
        }
    }
}