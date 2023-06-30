using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Codecov;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.ReportGenerator;

[GitHubActions("Build and Test",
           GitHubActionsImage.UbuntuLatest,
           OnPushBranches = new[] { "main" },
           OnPullRequestBranches = new[] { "main" },
           InvokedTargets = new[] { nameof(Test) },
           FetchDepth = 0,
    AutoGenerate = true)]
partial class Build : NukeBuild
{
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
           .SetReportTypes(ReportTypes.Cobertura)
           .SetTargetDirectory(CoverageReportDirectory)
           .SetAssemblyFilters("-*Tests"));
    });
}
