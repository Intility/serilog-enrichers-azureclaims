using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
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
partial class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;


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
}