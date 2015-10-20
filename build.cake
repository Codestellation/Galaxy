//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var configurationName = "Release";
var target = Argument("target", "Default");
var configuration = Argument("configuration", configurationName);

var packageVersion = string.Empty;
var product = "Codestellation.Galaxy";
var copyright = string.Format("Copyright (c) Codestellation Team 2014 - {0}", DateTime.Now.Year);

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDirInfo = new DirectoryInfo("./build");
var buildDir = Directory(buildDirInfo.FullName);
var nugetDir = Directory("./nuget");

var solutionPath = "./src/Galaxy.sln";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
    CleanDirectory(nugetDir);
});

//////////////////////////////////////////////////////////////////////

Task("Generate-Solution-Version")
    .Does(() =>
{
    var command = new ProcessSettings
    {
        Arguments = "describe --abbrev=7 --first-parent --long --dirty --always",
        RedirectStandardOutput = true
    };
    IEnumerable<string> output;
    var exitCode = StartProcess("git", command, out output);

    var describe = output.Single();

    Information("Git describe is '{0}'", describe);
    var annotatedTagPattern = @"(?<major>[0-9]+).(?<minor>[0-9]+)-(?<revision>[0-9]+)-g(?<hash>[\w]+)-?(?<dirty>[\w]+)*";
    var parts = System.Text.RegularExpressions.Regex.Match(describe, annotatedTagPattern);

    string major = "0";
    string minor = "0";
    string revision = "0";
    string build = "0"; // get it from appveyor
    string hash = string.Empty;
    string dirty = string.Empty;

    if(parts.Success)
    {
      major = parts.Groups["major"].Value;
      minor = parts.Groups["minor"].Value;
      revision = parts.Groups["revision"].Value;
      hash = parts.Groups["hash"].Value;
      dirty = parts.Groups["dirty"].Value;
    }
    else
    {
        var tokens = describe.Split('-');
        hash = tokens[0];
        if(tokens.Length > 1)
        {
          dirty = tokens[1];
        }
    }

    if(AppVeyor.IsRunningOnAppVeyor)
    {
        build = AppVeyor.Environment.Build.Number.ToString();
    }

    var assemblyVersion = string.Format("{0}.{1}", major, minor);
    var fullVersion = string.Format("{0}.{1}.{2}.{3}", major, minor, revision, build);

    packageVersion = fullVersion;
    if(!string.IsNullOrWhiteSpace(dirty))
    {
        packageVersion += ("-" + dirty);
    }

    var infoVersion = string.Format("{0} {1}", packageVersion, hash);

    var asmInfo = new AssemblyInfoSettings
    {
        Product = product,
        Version = assemblyVersion,
        FileVersion = fullVersion,
        InformationalVersion = infoVersion,
        Copyright = copyright
    };

    var file = "./src/SolutionVersion.cs";
    CreateAssemblyInfo(file, asmInfo);

    Information("AssemblyVersion is '{0}'", infoVersion);
});

//////////////////////////////////////////////////////////////////////

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(solutionPath);
});

//////////////////////////////////////////////////////////////////////

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Generate-Solution-Version")
    .Does(() =>
{
    MSBuild(solutionPath, settings => {
      settings.SetConfiguration(configuration);
      settings.Properties["OutDir"] = new List<string>{ buildDirInfo.FullName };
      settings.Verbosity = Verbosity.Minimal;
      });
});

//////////////////////////////////////////////////////////////////////

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
     var testsAssembly = buildDirInfo
         .EnumerateFiles("*.Tests.dll", SearchOption.AllDirectories)
         .Select(x => x.FullName)
         .First();

     var nunitPath = new DirectoryInfo("./")
        .EnumerateFiles("nunit-console.exe", SearchOption.AllDirectories)
        .First();

     var settings = new NUnitSettings { ToolPath = nunitPath.FullName };
     NUnit(testsAssembly, settings);
});


//////////////////////////////////////////////////////////////////////

Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
{
/*
  def get_nuget_dependencies(packages_folder)
    nuget_packages_file = "#{packages_folder}/packages.config"
    dependencies = Array.new
    xml = File.read nuget_packages_file
    doc = REXML::Document.new xml
    doc.elements.each "packages/package" do |package|
        packageId = package.attributes["id"]
        packageVersion = package.attributes["version"]
        dependencies << Dependency.new(packageId, packageVersion)
    end
    dependencies
  end

  nuspec :createHostNuspec do |nuspec|
    solutionpart = "Galaxy.Host"

    puts "Creating .nuspec file for #{solutionpart}..."
    nuspec.id="Codestellation.#{solutionpart}"
    nuspec.version = "#{@buildversion_package}"
    nuspec.authors = @authors
    nuspec.description = @description
    nuspec.title = "#{solutionpart}"
    nuspec.working_directory = "#{buildfolderpath}/#{solutionpart}"
    nuspec.output_file = "Codestellation.#{solutionpart}.nuspec"

    get_nuget_dependencies("#{@solutionfolder}/#{solutionpart}").each do |dep|
      nuspec.dependency dep.Name, dep.Version
    end
*/
var nuGetPackSettings   = new NuGetPackSettings {
    Id                      = "Codestellation.Galaxy.Host",
    Version                 = packageVersion,
    Title                   = "Codestellation Galaxy Host",
    Authors                 = new[] {"Codestellation Team"},
    Owners                  = new[] {"Codestellation Team"},
    Description             = "Provides extended functionality over TopShelf service",
    //Summary                 = "Excellent summare of what the package does",
    ProjectUrl              = new Uri("https://github.com/Codestellation/Galaxy"),
    //IconUrl                 = new Uri("http://cdn.rawgit.com/SomeUser/TestNuget/master/icons/testnuget.png"),
    LicenseUrl              = new Uri("https://github.com/Codestellation/Galaxy/blob/master/LICENSE"),
    Copyright               = copyright,
    //ReleaseNotes            = new [] {"Bug fixes", "Issue fixes", "Typos"},
    Tags                    = new [] {"Windows", "Service", "Consul", "Config"},
    RequireLicenseAcceptance= false,
    Symbols                 = false,
    //NoPackageAnalysis       = true,
    Files                   = new [] { new NuSpecContent {Source = "Codestellation.Galaxy.Host.???", Target = "lib/net45"}, },
    BasePath                = "./build",
    OutputDirectory         = "./nuget"
};

NuGetPack("./src/Galaxy.Host/galaxy.host.nuspec", nuGetPackSettings);
});

Task("Push")
    .IsDependentOn("Pack")
    .Does(() =>
{

  var packages = buildDirInfo
      .EnumerateFiles("./nuget/*.nupkg")
      .Select(x => x.FullName);

  foreach(var package in packages)
  {
    NuGetPush(package, new NuGetPushSettings {
        Source = "https://www.myget.org/F/codestellation/api/v2/package",
        ApiKey = EnvironmentVariable("myget_key")
        });
  }

});


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Test");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
