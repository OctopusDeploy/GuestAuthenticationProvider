using System;
using System.Threading;
using Octopus.Data.Model.User;
using Octopus.Data.Storage.User;
using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Authentication.HostServices;

namespace Octopus.Server.Extensibility.Authentication.Guest.GuestAuth
{
    class GuestUserStateChecker : IGuestUserStateChecker
    {
        readonly ILog log;
        readonly IUpdateableUserStore userStore;

        public GuestUserStateChecker(ILog log, IUpdateableUserStore userStore)
        {
            this.log = log;
            this.userStore = userStore;
        }

        public void EnsureGuestUserIsInCorrectState(bool isEnabled)
        {
            var user = userStore.GetByUsername(User.GuestLogin);

            // if there isn't a guest user and we're enabling then create the user
            if (user == null && isEnabled)
            {
                log.Info("Creating guest user");
                var userResult = userStore.Create(
                    User.GuestLogin,
                    "Guest",
                    string.Empty,
                    CancellationToken.None,
                    apiKeyDescriptor: new ApiKeyDescriptor("API-GUEST", "API-GUEST"),
                    password: Guid.NewGuid().ToString());
                if (userResult.WasFailure)
                {
                    log.Error("Error creating guest account: " + userResult.ErrorString);
                    return;
                }
                user = userResult.Value;

                // When the special guest login mode is enabled, no password is actually needed for the guest. 
                // But we give them a default password anyway just in case someone disables guest login and then re-enables the 
                // account
                var randomMilliseconds = new Random(DateTimeOffset.UtcNow.Millisecond).Next(100000);
                var pwd = DateTimeOffset.UtcNow.AddMilliseconds(randomMilliseconds).ToString();
                user.SetPassword(pwd);
            }

            // if we're enabling then by now the user must exist (we're doing the null check here to keep the compiler happy)
            if (user != null && isEnabled)
            {
                userStore.EnableUser(user.Id);
            }
            else if (user != null)  // if we're ensuring is disabled user may never have existed
            {
                userStore.DisableUser(user.Id);
            }
        }

    }

    public interface IGuestUserStateChecker
    {
        void EnsureGuestUserIsInCorrectState(bool isEnabled);
    }
}