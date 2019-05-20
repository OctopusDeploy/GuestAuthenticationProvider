using Autofac;
using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Authentication.Guest.Configuration;
using Octopus.Server.Extensibility.Authentication.Guest.GuestAuth;
using Octopus.Server.Extensibility.Extensions;
using Octopus.Server.Extensibility.Extensions.Infrastructure;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;
using Octopus.Server.Extensibility.Extensions.Mappings;

namespace Octopus.Server.Extensibility.Authentication.Guest
{
    [OctopusPlugin("Guest", "Octopus Deploy")]
    public class GuestExtension : IOctopusExtension
    {
        public void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GuestConfigurationMapping>().As<IConfigurationDocumentMapper>().InstancePerDependency();

            builder.RegisterType<GuestUserStateChecker>().As<IGuestUserStateChecker>().InstancePerDependency();
            builder.RegisterType<DatabaseInitializer>().As<IExecuteWhenDatabaseInitializes>().InstancePerDependency();

            builder.RegisterType<GuestConfigurationStore>()
                .As<IGuestConfigurationStore>()
                .As<IExtensionConfigurationStore<GuestConfiguration>>()
                .InstancePerDependency();
            builder.RegisterType<GuestConfigurationSettings>()
                .As<IGuestConfigurationSettings>()
                .As<IHasConfigurationSettings>()
                .As<IHasConfigurationSettingsResource>()
                .As<IContributeMappings>()
                .InstancePerDependency();
            builder.RegisterType<GuestConfigureCommands>().As<IContributeToConfigureCommand>().InstancePerDependency();

            builder.RegisterType<GuestCredentialValidator>()
                .As<IGuestCredentialValidator>()
                .As<IDoesBasicAuthentication>()
                .InstancePerDependency();
            builder.RegisterType<GuestLoginParametersHandler>().As<ICanHandleLoginParameters>().InstancePerDependency();
            builder.RegisterType<GuestAuthenticationProvider>().As<IAuthenticationProvider>().InstancePerDependency();
        }
    }
}