using Octopus.Data.Storage.Configuration;
using Octopus.Server.Extensibility.Authentication.Guest.GuestAuth;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Server.Extensibility.Authentication.Guest.Configuration
{
    class GuestConfigurationStore : ExtensionConfigurationStore<GuestConfiguration>, IGuestConfigurationStore
    {
        public static string SingletonId = "authentication-guest";

        readonly IGuestUserStateChecker guestUserStateChecker;

        public GuestConfigurationStore(
            IConfigurationStore configurationStore,
            IGuestUserStateChecker guestUserStateChecker) : base(configurationStore)
        {
            this.guestUserStateChecker = guestUserStateChecker;
        }

        public override string Id => SingletonId;

        public override void SetIsEnabled(bool isEnabled)
        {
            base.SetIsEnabled(isEnabled);
            guestUserStateChecker.EnsureGuestUserIsInCorrectState(isEnabled);
        }
    }
}