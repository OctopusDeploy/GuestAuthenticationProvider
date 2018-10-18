using System;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Server.Extensibility.Authentication.Guest.Configuration
{
    public class GuestConfigurationMapping : IConfigurationDocumentMapper
    {
        public Type GetTypeToMap() => typeof(GuestConfiguration);
    }
}