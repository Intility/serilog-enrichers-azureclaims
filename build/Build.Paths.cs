using Nuke.Common.IO;

partial class Build
{
    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath TestResultDirectory => OutputDirectory / "test-results";
    AbsolutePath CoverageReportDirectory => OutputDirectory / "coberage-reports";

    AbsolutePath PackageDirectory => OutputDirectory / "packages";

    AbsolutePath TestProjectDir => TestsDirectory / "Serilog.Enrichers.AzureAuthInfo.Tests";
    AbsolutePath TestProjectFile => TestProjectDir / "Serilog.Enrichers.AzureAuthInfo.Tests.csproj";
}
