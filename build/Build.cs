using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.ReportGenerator;
using System;
using System.Linq;
using static Nuke.Common.IO.PathConstruction;

[GitHubActions("Build and Test",
           GitHubActionsImage.UbuntuLatest,
           OnPushBranches = new[] { "main" },
           OnPullRequestBranches = new[] { "main" },
           InvokedTargets = new[] { nameof(Test) },
           FetchDepth = 0,
    AutoGenerate = true)]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Api key to use when pushing the package")]
    readonly string NugetApiKey;

    [Parameter("NuGet artifact target uri - Defaults to https://api.nuget.org/v3/index.json")]
    readonly string PackageSource = "https://api.nuget.org/v3/index.json";

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion(NoFetch = true, Framework = "net7.0")] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath TestResultDirectory => OutputDirectory / "test-results";
    AbsolutePath CoverageReportDirectory => OutputDirectory / "coberage-reports";

    AbsolutePath TestProjectDir => TestsDirectory / "Serilog.Enrichers.AzureAuthInfo.Tests";
    AbsolutePath TestProjectFile => TestProjectDir / "Serilog.Enrichers.AzureAuthInfo.Tests.csproj";


    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").DeleteDirectories();
            TestsDirectory.GlobDirectories("**/bin", "**/obj").DeleteDirectories();
            OutputDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {

            DotNetTasks.DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetTreatWarningsAsErrors(true)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });


    Target Test => _ => _
        .DependsOn(Compile)
        .Produces(TestResultDirectory / "*.trx")
        .Produces(TestResultDirectory / "*.xml")    
        .Executes(() =>
        {
            DotNetTasks.DotNetTest(s => s
               .SetProjectFile(TestProjectFile)
               .SetConfiguration(Configuration)
               .SetNoBuild(InvokedTargets.Contains(Compile))
               .EnableNoRestore()
               .ResetVerbosity()
               .SetResultsDirectory(TestResultDirectory)
               .EnableCollectCoverage()
               .SetCoverletOutputFormat(CoverletOutputFormat.opencover)
               .SetProcessArgumentConfigurator(a => a.Add("--collect:\"XPlat Code Coverage\""))
               .SetLoggers($"junit;LogFileName={"coverage.trx"}")
               .SetCoverletOutput(TestResultDirectory / "coverage.cobertura.xml")
               );
        });

    Target ReportCoverage => _ => _
        .DependsOn(Test)
        .Consumes(Test)
    .Executes(() =>
    {
        ReportGeneratorTasks.ReportGenerator(s => s
           .SetReports(TestResultDirectory / "**/*.xml")
           .SetReportTypes(ReportTypes.Cobertura, ReportTypes.TextSummary)
           .SetTargetDirectory(CoverageReportDirectory)
           .SetAssemblyFilters("-*Tests"));
    });

    Target Pack => _ => _
        .DependsOn(Clean, Compile)
        .Executes(() =>
        {
            DotNetTasks.DotNetPack(s => s
                .SetProject(Solution)
                .SetTreatWarningsAsErrors(true)
                .EnableNoBuild()
                .SetConfiguration(Configuration)
                .SetVersion(GitVersion.NuGetVersionV2)
                .SetOutputDirectory(OutputDirectory));
        });

    Target Push => _ => _
        .DependsOn(Pack)
        .Requires(() => NugetApiKey)
        .Requires(() => PackageSource)
        .Executes(() =>
        {
            DotNetTasks.DotNetNuGetPush(s => s
                .SetTargetPath(OutputDirectory / $"*.nupkg")
                .SetApiKey(NugetApiKey)
                .SetSource(PackageSource));
        });
}
