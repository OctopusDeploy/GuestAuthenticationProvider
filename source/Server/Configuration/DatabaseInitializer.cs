using Octopus.Configuration;
using Octopus.Data.Storage.Configuration;
using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Authentication.Guest.GuestAuth;
using Octopus.Server.Extensibility.Extensions.Infrastructure;

namespace Octopus.Server.Extensibility.Authentication.Guest.Configuration
{
    class DatabaseInitializer : ExecuteWhenDatabaseInitializes
    {
        readonly ISystemLog log;
        readonly IWritableKeyValueStore settings;
        readonly IConfigurationStore configurationStore;
        readonly IGuestUserStateChecker guestUserStateChecker;

        private bool cleanupRequired = false;

        public DatabaseInitializer(ISystemLog log, IWritableKeyValueStore settings, IConfigurationStore configurationStore, IGuestUserStateChecker guestUserStateChecker)
        {
            this.log = log;
            this.settings = settings;
            this.configurationStore = configurationStore;
            this.guestUserStateChecker = guestUserStateChecker;
        }

        public override void Execute()
        {
            var doc = configurationStore.Get<GuestConfiguration>(GuestConfigurationStore.SingletonId);
            if (doc != null)
            {
                // TODO: to cover a dev team edge case during 4.0 Alpha. Can be removed before final release
                if (doc.ConfigurationSchemaVersion != "1.0")
                {
                    doc.ConfigurationSchemaVersion = "1.0";
                    configurationStore.Update(doc);
                }
                return;
            }

            log.Info("Moving Octopus.WebPortal.GuestLoginEnabled from config file to DB");

            var guestLoginEnabled = settings.Get("Octopus.WebPortal.GuestLoginEnabled", false);

            doc = new GuestConfiguration()
            {
                IsEnabled = guestLoginEnabled
            };

            configurationStore.Create(doc);

            guestUserStateChecker.EnsureGuestUserIsInCorrectState(guestLoginEnabled);

            cleanupRequired = true;
        }

        public override void PostExecute()
        {
            if (!cleanupRequired)
                return;

            settings.Remove("Octopus.WebPortal.GuestLoginEnabled");
            settings.Save();
        }
    }
}