using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using Nuke.Common.Tools.NuGet;
using Nuke.OctoVersion;
using OctoVersion.Core;

[CheckBuildProjectConfigurations]
class Build : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [NukeOctoVersion] readonly OctoVersionInfo OctoVersionInfo;

    AbsolutePath LocalPackagesDirectory => RootDirectory / ".." / "LocalPackages";
    AbsolutePath SourceDirectory => RootDirectory / "source";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath PublishDirectory => RootDirectory / "publish";
    AbsolutePath AssetDirectory => RootDirectory / "BuildAssets";

    Target Clean =>
        _ => _
            .Before(Restore)
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
                DotNetRestore(settings => settings
                    .SetProjectFile(Solution)
                    .SetVersion(OctoVersionInfo.NuGetVersion));
            });

    Target Compile =>
        _ => _
            .DependsOn(Restore)
            .Executes(() =>
            {
                DotNetBuild(settings => settings
                    .SetProjectFile(Solution)
                    .SetConfiguration(Configuration)
                    .SetVersion(OctoVersionInfo.NuGetVersion));
            });

    Target Pack =>
        _ => _
            .DependsOn(Compile)
            .Executes(() =>
            {
                DotNetPack(settings => settings
                    .SetProject(Solution)
                    .SetConfiguration(Configuration)
                    .SetOutputDirectory(ArtifactsDirectory)
                    .EnableNoBuild()
                    .SetVersion(OctoVersionInfo.NuGetVersion));

                var odNugetPackDir = PublishDirectory / "od";
                var nuspecFile = "Octopus.Server.Extensibility.Authentication.Guest.nuspec";

                EnsureExistingDirectory(odNugetPackDir);
                CopyFileToDirectory(AssetDirectory / nuspecFile, odNugetPackDir);

                var dllPattern = SourceDirectory / "Server" / "bin" / "**" / "netstandard2.1" / "Octopus.Server.Extensibility.Authentication.Guest.dll";
                GlobFiles(dllPattern)
                    .ForEach(dll => CopyFileToDirectory(dll, odNugetPackDir));

                var newNuspec = odNugetPackDir / nuspecFile;

                NuGetTasks.NuGetPack(settings => settings
                    .SetVersion(OctoVersionInfo.NuGetVersion)
                    .SetOutputDirectory(ArtifactsDirectory)
                    .SetTargetPath(newNuspec));
            });

    Target CopyToLocalPackages =>
        _ => _
            .OnlyWhenStatic(() => IsLocalBuild)
            .TriggeredBy(Pack)
            .Executes(() =>
            {
                EnsureExistingDirectory(LocalPackagesDirectory);
                var nupkgs = GlobFiles(ArtifactsDirectory, $"Octopus.*.Extensibility.Authentication.Guest.*.nupkg");
                nupkgs.ForEach(x => CopyFileToDirectory(x, LocalPackagesDirectory));
            });

    Target Default => _ => _.DependsOn(Pack);

    public static int Main() => Execute<Build>(x => x.Default);
}