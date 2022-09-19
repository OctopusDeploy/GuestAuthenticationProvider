using System;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using Nuke.Common.Tools.OctoVersion;
using Serilog;

class Build : NukeBuild
{
    [Parameter("Configuration to build - 'Release' (server)")]
    readonly Configuration Configuration = Configuration.Release;
    
    [Solution] readonly Solution Solution;

    [Parameter("Whether to auto-detect the branch name - this is okay for a local build, but should not be used under CI.")] readonly bool AutoDetectBranch = IsLocalBuild;

    [OctoVersion(BranchMember = nameof(BranchName), AutoDetectBranchMember = nameof(AutoDetectBranch), Framework = "net6.0")]
    public OctoVersionInfo OctoVersionInfo;

    const string CiBranchNameEnvVariable = "OCTOVERSION_CurrentBranch";
    [Parameter("Branch name for OctoVersion to use to calculate the version number. Can be set via the environment variable " + CiBranchNameEnvVariable + ".", Name = CiBranchNameEnvVariable)]
    string BranchName { get; set; }

    AbsolutePath LocalPackagesDirectory => RootDirectory / ".." / "LocalPackages";
    AbsolutePath SourceDirectory => RootDirectory / "source";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath PublishDirectory => RootDirectory / "publish";

    Target Clean =>
        _ => _
            .Executes(() =>
            {
                EnsureCleanDirectory(ArtifactsDirectory);
                EnsureCleanDirectory(PublishDirectory);
                SourceDirectory
                    .GlobDirectories("**/bin", "**/obj")
                    .ForEach(EnsureCleanDirectory);
            });

    Target Restore =>
        _ => _
            .DependsOn(Clean)
            .Executes(() =>
            {
                DotNetRestore(_ => _
                    .SetProjectFile(Solution));
            });

    Target Compile =>
        _ => _
            .DependsOn(Restore)
            .Executes(() =>
            {
                Log.Logger.Information("Building Octopus Server Guest Authentication Provider v{Version}", OctoVersionInfo.FullSemVer);

                // This is done to pass the data to github actions
                Console.Out.WriteLine($"::set-output name=semver::{OctoVersionInfo.FullSemVer}");
                Console.Out.WriteLine($"::set-output name=prerelease_tag::{OctoVersionInfo.PreReleaseTagWithDash}");

                DotNetBuild(_ => _
                    .SetProjectFile(Solution)
                    .SetConfiguration(Configuration)
                    .SetVersion(OctoVersionInfo.FullSemVer));
            });

    Target Pack =>
        _ => _
            .DependsOn(Compile)
            .Executes(() =>
            {
                Log.Logger.Information("Packing Octopus Server Guest Authentication Provider v{Version}", OctoVersionInfo.FullSemVer);

                const string nuspecFile = "Octopus.Server.Extensibility.Authentication.Guest.nuspec";
                
                CopyFileToDirectory(BuildProjectDirectory / nuspecFile, PublishDirectory);
                CopyFileToDirectory(RootDirectory / "LICENSE.txt", PublishDirectory);
                CopyFileToDirectory(BuildProjectDirectory / "icon.png", PublishDirectory);
                CopyFileToDirectory(SourceDirectory / "Server" / "bin" / Configuration / "net6.0" / "Octopus.Server.Extensibility.Authentication.Guest.dll" , PublishDirectory);


                DotNetPack(_ => _
                    .SetProject(SourceDirectory / "Server"/ "Server.csproj")
                    .SetVersion(OctoVersionInfo.FullSemVer)
                    .SetConfiguration(Configuration)
                    .SetOutputDirectory(ArtifactsDirectory)
                    .EnableNoBuild()
                    .DisableIncludeSymbols()
                    .SetVerbosity(DotNetVerbosity.Normal)
                    .SetProperty("NuspecFile", PublishDirectory / nuspecFile)
                    .SetProperty("NuspecProperties", $"Version={OctoVersionInfo.FullSemVer}"));
            
                DotNetPack(_ => _
                    .SetProject(SourceDirectory / "Client"/ "Client.csproj")
                    .SetVersion(OctoVersionInfo.FullSemVer)
                    .SetConfiguration(Configuration)
                    .SetOutputDirectory(ArtifactsDirectory)
                    .EnableNoBuild()
                    .DisableIncludeSymbols()
                    .SetVerbosity(DotNetVerbosity.Normal));
            });

    Target CopyToLocalPackages =>
        _ => _
            .OnlyWhenStatic(() => IsLocalBuild)
            .TriggeredBy(Pack)
            .Executes(() =>
            {
                EnsureExistingDirectory(LocalPackagesDirectory);
                ArtifactsDirectory.GlobFiles("*.nupkg")
                    .ForEach(package =>
                    {
                        CopyFileToDirectory(package, LocalPackagesDirectory, FileExistsPolicy.Overwrite);
                    });
            });

    Target Default => _ => _
        .DependsOn(Pack)
        .DependsOn(CopyToLocalPackages);

    public static int Main() => Execute<Build>(x => x.Default);
}