using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;

[GitHubActions("Publish",
           GitHubActionsImage.UbuntuLatest,
           OnPushTags = new[] { "v*" },
           InvokedTargets = new[] { nameof(Push) },
           FetchDepth = 0,
           ImportSecrets = new[] { nameof(NugetApiKey) },
    AutoGenerate = true)]
partial class Build : NukeBuild
{
    [Parameter("Api key to use when pushing the package")]
    readonly string NugetApiKey;

    [Parameter("NuGet artifact target uri - Defaults to https://api.nuget.org/v3/index.json")]
    readonly string PackageSource = "https://api.nuget.org/v3/index.json";

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion(NoFetch = true, Framework = "net8.0")] readonly GitVersion GitVersion;

    Target Pack => _ => _
        .DependsOn(Clean, Compile)
        .Produces(PackageDirectory / "*nupkg")
        .Executes(() =>
        {
            DotNetTasks.DotNetPack(s => s
                .SetProject(Solution)
                .SetTreatWarningsAsErrors(true)
                .EnableNoBuild()
                .SetConfiguration(Configuration)
                .SetVersion(GitVersion.NuGetVersionV2)
                .SetOutputDirectory(PackageDirectory));
        });

    Target Push => _ => _
        .Consumes(Pack)
        .DependsOn(Pack)
        .Requires(() => NugetApiKey)
        .Requires(() => PackageSource)
        .Executes(() =>
        {
            DotNetTasks.DotNetNuGetPush(s => s
                .SetTargetPath(PackageDirectory / $"*.nupkg")
                .SetApiKey(NugetApiKey)
                .SetSource(PackageSource));
        });

}

