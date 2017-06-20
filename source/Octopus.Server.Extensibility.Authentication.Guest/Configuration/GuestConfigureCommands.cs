﻿using System;
using System.Collections.Generic;
using Octopus.Diagnostics;
using Octopus.Node.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Server.Extensibility.Authentication.Guest.Configuration
{
    public class GuestConfigureCommands : IContributeToConfigureCommand
    {
        readonly ILog log;
        readonly Lazy<IGuestConfigurationStore> configurationStore;

        public GuestConfigureCommands(
            ILog log,
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