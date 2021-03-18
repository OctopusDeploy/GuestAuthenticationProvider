using System;
using System.Collections.Generic;
using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Server.Extensibility.Authentication.Guest.Configuration
{
    class GuestConfigureCommands : IContributeToConfigureCommand
    {
        readonly ISystemLog log;
        readonly Lazy<IGuestConfigurationStore> configurationStore;

        public GuestConfigureCommands(
            ISystemLog log,
            Lazy<IGuestConfigurationStore> configurationStore)
        {
            this.log = log;
            this.configurationStore = configurationStore;
        }

        public IEnumerable<ConfigureCommandOption> GetOptions()
        {
            yield return new ConfigureCommandOption("guestloginenabled=", "Whether guest login should be enabled", v =>
            {
                var isEnabled = bool.Parse(v);
                configurationStore.Value.SetIsEnabled(isEnabled);
                log.Info($"Guest login enabled: {isEnabled}");
            });
        }
    }
}