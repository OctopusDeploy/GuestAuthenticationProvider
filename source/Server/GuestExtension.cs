using Autofac;
using Octopus.Node.Extensibility.Authentication.Extensions;
using Octopus.Node.Extensibility.Extensions;
using Octopus.Node.Extensibility.Extensions.Infrastructure;
using Octopus.Node.Extensibility.Extensions.Infrastructure.Configuration;
using Octopus.Node.Extensibility.Extensions.Mappings;
using Octopus.Server.Extensibility.Authentication.Guest.Configuration;
using Octopus.Server.Extensibility.Authentication.Guest.GuestAuth;

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
                .As<IHasConfigurationSettings>()
                .As<IContributeMappings>()
                .InstancePerDependency();
            builder.RegisterType<GuestConfigureCommands>().As<IContributeToConfigureCommand>().InstancePerDependency();

            builder.RegisterType<GuestCredentialValidator>()
                .As<IGuestCredentialValidator>()
                .As<IDoesBasicAuthentication>()
                .InstancePerDependency();

            builder.RegisterType<GuestAuthenticationProvider>().As<IAuthenticationProvider>().InstancePerDependency();
        }
    }
}