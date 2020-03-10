using Octopus.Server.Extensibility.Authentication.Extensions;

namespace Octopus.Server.Extensibility.Authentication.Guest.GuestAuth
{
    interface IGuestCredentialValidator : IDoesBasicAuthentication
    {
    }
}