#addin "nuget:?package=Cake.DocFx&version=0.13.1"
#addin "nuget:?package=Cake.Git&version=0.21.0"
#addin "nuget:?package=Cake.ReSharperReports&version=0.11.1"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var configuration = Argument("configuration", "Debug");
var revision = EnvironmentVariable("BUILD_NUMBER") ?? Argument("revision", "9999");
var target = Argument("target", "Default");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define git commit id
var commitId = "SNAPSHOT";

// Define product name and version
var product = "EasyCaptcha.Wpf";
var companyName = "";
var version = "0.9.0";
var semanticVersion = string.Format("{0}.{1}", version, revision);
var ciVersion = string.Format("{0}.{1}", version, "0");
var nugetTags = new [] {"captcha", "wpf", "widget"};

// Define copyright
var copyright = string.Format("Copyright © 2020 - {0}", DateTime.Now.Year);

// Define path
var solutionFile = File(string.Format("./source/{0}.sln", product));

// Define directories.
var distDir = Directory("./dist");
var tempDir = Directory("./temp");
var generatedDir = Directory("./source/generated");
var packagesDir = Directory("./source/packages");
var nugetDir = distDir + Directory(configuration) + Directory("nuget");
var homeDir = Directory(EnvironmentVariable("USERPROFILE") ?? EnvironmentVariable("HOME"));
var reportReSharperDupFinder = distDir + Directory(configuration) + Directory("report/ReSharper/DupFinder");
var reportReSharperInspectCode = distDir + Directory(configuration) + Directory("report/ReSharper/InspectCode");
var nugetApiKey = EnvironmentVariable("NUGET_APIKEY") ?? "NOTSET";
var nugetSource = EnvironmentVariable("NUGET_SOURCE") ?? "NOTSET";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Fetch-Git-Commit-ID")
    .ContinueOnError()
    .Does(() =>
{
    var lastCommit = GitLogTip(MakeAbsolute(Directory(".")));
    commitId = lastCommit.Sha;
});

Task("Display-Config")
    .IsDependentOn("Fetch-Git-Commit-ID")
    .Does(() =>
{
    Information("Build target: {0}", target);
    Information("Build configuration: {0}", configuration);
    Information("Build commitId: {0}", commitId);
    if ("Release".Equals(configuration))
    {
        Information("Build version: {0}", semanticVersion);
    }
    else
    {
        Information("Build version: {0}-CI{1}", ciVersion, revision);
    }
});

Task("Clean-Workspace")
    .IsDependentOn("Display-Config")
    .Does(() =>
{
    CleanDirectory(distDir);
    CleanDirectory(tempDir);
    CleanDirectory(generatedDir);
    CleanDirectory(packagesDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean-Workspace")
    .Does(() =>
{
    NuGetRestore(string.Format("./source/{0}.sln", product));
});

Task("Generate-AssemblyInfo")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    CreateDirectory(generatedDir);
    var file = "./source/generated/SharedAssemblyInfo.cs";
    var assemblyVersion = semanticVersion;
    CreateAssemblyInfo(
            file,
            new AssemblyInfoSettings
            {
                    Company = companyName,
                    Copyright = copyright,
                    Product = string.Format("{0} : {1}", product, commitId),
                    Version = assemblyVersion,
                    FileVersion = assemblyVersion,
                    InformationalVersion = assemblyVersion
            }
    );
});

Task("Build-Assemblies")
    .IsDependentOn("Generate-AssemblyInfo")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings
    {
            Configuration = configuration
    };
    DotNetCoreBuild("./source/", settings);
});

Task("Run-DupFinder")
    .IsDependentOn("Build-Assemblies")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
        DupFinder(
                string.Format("./source/{0}.sln", product),
                new DupFinderSettings()
                {
                        ShowStats = true,
                        ShowText = true,
                        OutputFile = new FilePath(reportReSharperDupFinder.ToString() + "/" + product + ".xml"),
                        ThrowExceptionOnFindingDuplicates = false
                }
        );
        ReSharperReports(
                new FilePath(reportReSharperDupFinder.ToString() + "/" + product + ".xml"),
                new FilePath(reportReSharperDupFinder.ToString() + "/" + product + ".html")
        );
    }
});

Task("Run-InspectCode")
    .IsDependentOn("Run-DupFinder")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
        InspectCode(
                string.Format("./source/{0}.sln", product),
                new InspectCodeSettings()
                {
                        SolutionWideAnalysis = true,
                        OutputFile = new FilePath(reportReSharperInspectCode.ToString() + "/" + product + ".xml"),
                        ThrowExceptionOnFindingViolations = false
                }
        );
        ReSharperReports(
                new FilePath(reportReSharperInspectCode.ToString() + "/" + product + ".xml"),
                new FilePath(reportReSharperInspectCode.ToString() + "/" + product + ".html")
        );
    }
});

Task("Build-NuGet-Package")
    .IsDependentOn("Run-InspectCode")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var settings = new DotNetCorePackSettings
    {
            Configuration = configuration,
            OutputDirectory = nugetDir,
            NoBuild = true,
            ArgumentCustomization = (args) =>
            {
                    return args.Append("/p:Version={0}", nugetPackVersion);
            }
    };

    DotNetCorePack("./source/" + product + "/", settings);
});

Task("Publish-NuGet-Package")
    .WithCriteria(() => "Release".Equals(configuration) && !"NOTSET".Equals(nugetApiKey) && !"NOTSET".Equals(nugetSource))
    .IsDependentOn("Build-NuGet-Package")
    .Does(() =>
{
    var nugetPushVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPushVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Publish version: {0}", nugetPushVersion);
    var package = string.Format("./dist/{0}/nuget/{1}.{2}.nupkg", configuration, product, nugetPushVersion);
    NuGetPush(
            package,
            new NuGetPushSettings
            {
                    Source = nugetSource,
                    ApiKey = nugetApiKey
            }
    );
});


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Publish-NuGet-Package");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
