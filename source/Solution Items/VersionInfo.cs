// Versioned by GitVersion

using System.Reflection;

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("0.0.0-local")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace Octopus
{
    static class GitVersionInformation
    {
        public static string BranchName = "UNKNOWNBRANCH";
        public static string NuGetVersion = "0.0.0-local";
    }
}