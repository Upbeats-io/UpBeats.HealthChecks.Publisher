#addin "Cake.FileHelpers"
#addin "Cake.Git"

// Get Build Arguments
var branchName = Argument("Branch", "")
    ?? "Missing Branch Name";
var revision = Argument("Revision", "")
    ?? "Missing Revision Number";
var buildVersion = Argument("BuildVersion", "");
var isLocalBuild = Argument("IsLocalBuild", true);
var gitUser = Argument("GitUser", "");
var gitPassword = Argument("GitPassword", ""); 

 var nuGetPackageSource = Argument("nuGetPackageSource", "https://api.nuget.org/v3/index.json");
 var nuGetApiKey = Argument("nuGetApiKey", "");

var logger = Argument("Logger", "trx");
var testResultsPath = Argument("TestResultsPath", "../testresults");
var packagesPath = Argument("PackagesPath", "../packages");

// Setup Parameters
var configuration = "Release";
var isMasterBranch = StringComparer.OrdinalIgnoreCase.Equals("master", branchName);
var buildNumberSuffix = string.IsNullOrWhiteSpace(revision)?"":$".{revision}";
var branchSuffix = isMasterBranch ? "" : string.IsNullOrWhiteSpace(branchName)?"":"-" + branchName;
var revisionSuffix = $"{buildNumberSuffix}{branchSuffix}";
var baseVersion = FileReadText("../version.txt");
var fullVersion = !string.IsNullOrWhiteSpace(buildVersion) ? buildVersion : baseVersion + revisionSuffix;

var publishNugetPackages = !isLocalBuild && !string.IsNullOrWhiteSpace(nuGetPackageSource) && !string.IsNullOrWhiteSpace(nuGetApiKey);

var solutionFolder = "../";
var solutionPath = $"../UpBeats.HealthChecks.Publisher.sln";

// Log Parameters
Information("========================================");
Information("Usage: .\\build.ps1 [--target=Default] --Branch=UB-123 --Revision=34 [--IsLocalBuild=true] [--nuGetPackageSource=https://api.nuget.org/v3/index.json] [--nuGetApiKey=az] [--GitUser=user] [--GitPassword=pass]");
Information("BUILD PARAMETERS");
Information("========================================");
Information($"IsLocalBuild: {(isLocalBuild ? "Yes" : "No")}");
Information($"Branch: {branchName}");
Information($"Configuration: {configuration}");
Information($"Version: {fullVersion}");
// Information($"nuGet NuGet Package Source: {nuGetPackageSource}");
Information($"Publish nuget packages: {publishNugetPackages}");

Task("Clean")
    .Does(() =>
    {
        CleanDirectories("../**/**/bin");
        Information("Cleaning 'bin' folders.");
        CleanDirectories("../**/**/obj");
        Information("Cleaning 'obj' folders.");
        CleanDirectories(packagesPath);
        Information("Cleaning 'packages' folder.");
    });

Task("Restore")
    .Does(() =>
    {
        DotNetCoreRestore(solutionPath);
    });

Task("Build")
    .Does(() =>
    {
        DotNetCoreBuild(
            solutionPath,
            new DotNetCoreBuildSettings
            {
                Configuration = configuration,
                NoRestore = true,
                MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersion(fullVersion)
            });
    });

Task("UnitTest")
    .Does(() =>
    {
        var dotNetCoreTestSettings = new DotNetCoreTestSettings
        {
            Configuration = configuration,
            NoBuild = true,
            NoRestore = true,
            Logger = logger,
            ResultsDirectory = testResultsPath
        };

        var projectFiles = GetFiles("../test/*/*.Tests.Unit.csproj");
        foreach (var projectFile in projectFiles) {
            Information($"Executing Tests From {projectFile.GetFilename()}");

            DotNetCoreTest(
                projectFile.FullPath,
                dotNetCoreTestSettings
            );
        }
    });

Task("IntegrationTest")
    .Does(() =>
    {
        var dotNetCoreTestSettings = new DotNetCoreTestSettings
        {
            Configuration = configuration,
            Filter = "FullyQualifiedName!~Mambu",
            NoBuild = true,
            NoRestore = true,
            Logger = logger,
            ResultsDirectory = testResultsPath
        };

        var projectFiles = GetFiles("../test/*/*.Tests.Integration.csproj");
        foreach (var projectFile in projectFiles) {
            Information($"Executing Tests From {projectFile.GetFilename()}");

            DotNetCoreTest(
                projectFile.FullPath,
                dotNetCoreTestSettings
            );
        }
    });

Task("Pack")
    .Does(() =>
    {
        var dotNetCorePackSettings = new DotNetCorePackSettings
        {
            Configuration = configuration,
            NoBuild = true,
            NoRestore = true,
            IncludeSymbols = false,
            OutputDirectory = packagesPath,
            MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersion(fullVersion)
        };

        DotNetCorePack(solutionPath, dotNetCorePackSettings);
    });

Task("PublishPackages")
    .Does(() =>
    {
		var packages = new List<string>{
			$"UpBeats.HealthChecks.Publisher",		
		};
	
        var dotNetCoreNuGetPushSettings = new DotNetCoreNuGetPushSettings
        {
            Source = nuGetPackageSource,
            ApiKey = nuGetApiKey,
			SkipDuplicate = true
        };

		foreach(var p in packages)
		{
			var path = MakeAbsolute(Directory(packagesPath)).Combine($"{p}.{fullVersion}.nupkg").FullPath;
			DotNetCoreNuGetPush(path, dotNetCoreNuGetPushSettings);
		}        
    });

 Task("AddGitTag")
    .Does(() =>
    {
        GitTag(solutionFolder, fullVersion);
        GitPushRef(solutionFolder, gitUser, gitPassword, "origin", fullVersion); 
    });

Task("Default")
   .IsDependentOn("Clean")
   .IsDependentOn("Restore")
   .IsDependentOn("Build")
   .IsDependentOn("UnitTest")
   //.IsDependentOn("IntegrationTest")
   .IsDependentOn("Pack");
   
Task("BuildAndPublish")
   .IsDependentOn("Default")
   .IsDependentOn("AddGitTag")
   .IsDependentOn("PublishPackages");
   
Task("TagAndPublish")
   .IsDependentOn("AddGitTag")
   .IsDependentOn("PublishPackages");

var target = Argument("target", "Default");

RunTarget(target);