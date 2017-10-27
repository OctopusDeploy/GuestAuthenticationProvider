using Octopus.Data.Model;
using Octopus.Data.Resources;

namespace Octopus.Server.Extensibility.Authentication.Guest.Configuration
{
    public class GuestConfigurationResource : IResource
    {
        public string Id { get; }

        public LinkCollection Links { get; set; }
    }
}