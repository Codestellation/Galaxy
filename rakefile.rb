require 'albacore'

# --------------------------------------------------------------------------------------------------------------------
# build variables

@env_projectname = "Codestellation.Galaxy"

@script_description = "rake automated build script for #{@env_projectname}"
@script_version = "script version 1.0"

# relative path to .sln file
@env_solutionfolder = "./src"

@env_nugetpackagesfolder = "#{@env_solutionfolder}/packages"
@env_nugetexefolder = "#{@env_solutionfolder}/.nuget"
@env_authors = "Codestellation Team"
@env_description = "Windows hosting service"

# default parameters, actual version is build using git describe command
@env_buildversion_native = "1.0"
@env_buildversion_package = "1.0.0"
@env_buildversion_full = "1.0.0"

@env_buildconfigname = "Release"

@env_testsexceptions = ""

@env_buildsfolder = "build"
@env_artifactsfolder = "#{@env_buildsfolder}/artifacts"

@env_is_x64 = true

@env_debug_output = true



def env_projectfullname
  "v#{@env_buildversion_full}__#{@env_buildconfigname}"
end

def env_buildfolderpath
  "#{@env_buildsfolder}/#{env_projectfullname}"
end

# --------------------------------------------------------------------------------------------------------------------
# auxiliary classes

class Version
  def initialize(git_description)
    @git_description = git_description
    
    version_parts = @git_description.match /(\d+).(\d+)-(\d+)-(\w+)[-]*(\w*)/
    if version_parts.nil?
      version_parts = @git_description.match /(\w+)[-]*(\w*)/
      @major = 0
      @minor = 0
      @revision = 1
      @hash = version_parts[1] [1...7]
      @dirty = version_parts[2]
    else
      @major = version_parts[1]
      @minor = version_parts[2]
      @revision = version_parts[3]
      @hash = version_parts[4] [1...7]
      @dirty = version_parts[5]
    end
  end

  def native
    "#{@major}.#{@minor}"
  end

  def standard
    "#{@major}.#{@minor}.#{@revision}"
  end

  def package
    package_version = standard
    if @dirty != ""
      package_version += "-#{@dirty}"
    end
    package_version
  end

  def full
    full_version = "#{@major}.#{@minor}.#{@revision}-#{@hash}"
    if @dirty != ""
      full_version += "-#{@dirty}"
    end
    full_version
  end
end

class Dependency
    attr_accessor :Name, :Version
    def initialize(name, version)
        @Name = name
        @Version = version
    end
end

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

# --------------------------------------------------------------------------------------------------------------------


task :default => [:presentYourself, :buildIt , :testIt, :deployIt, :presentResults]
task :buildIt => [:versionIt, :buildAll, :createCleanBuildFolder, :copyHostBinaries, :copyServiceBinaries, :createHostNuspec]
task :testIt => [:installNUnitRunners, :runUnitTests]
task :deployIt => [:createArtifactsFolder, :createHostNuget, :createServiceZip]

task :presentYourself do
  puts "#{@script_description}"
  puts "#{@script_version}"
end

task :presentResults do
  puts "Solution binaries in #{env_buildfolderpath}"
end

desc "Install NUnit.Runners package"
exec :installNUnitRunners do |cmd|
  cmd.command = "./#{@env_nugetexefolder}/NuGet.exe"
  cmd.parameters = "install NUnit.Runners -o #{@env_nugetpackagesfolder} -ExcludeVersion"
end

desc "Generate solution version"
assemblyinfo :versionIt do |asm|
  puts "Detecting build version..."
  
  # parsing output. It looks this way: v1.0-0-g69d5874d6aa1cbfd2ef5d5205162b872cccb0471-dirty
  git_description = `git describe --abbrev=64 --first-parent --long --dirty --always`
  version = Version.new(git_description)

  asm.version = version.standard
  asm.file_version = version.standard
  asm.custom_attributes :AssemblyInformationalVersionAttribute => version.full
  asm.output_file = "src/SolutionVersion.cs"

  @env_buildversion_native = version.native
  @env_buildversion_package = version.package
  @env_buildversion_full = version.full
  puts "Build version detected as #{@env_buildversion_full}"
end

task :buildAll do
  Rake::Task["buildSolution"].execute
end

desc "Clean and build the solution"
msbuild :buildSolution do |msb|
  msb.properties :configuration => @env_buildconfigname
  msb.targets :Clean, :Build
  msb.solution = "#{@env_solutionfolder}/#{@env_projectname}.sln"
  msb.verbosity = "quiet"
  msb.parameters = "/nologo"
end

desc "Creates clean build folder structure"
task :createCleanBuildFolder do
  FileUtils.rm_rf(@env_buildsfolder)
  FileUtils.mkdir_p("#{@env_buildsfolder}")
end

desc "Copy Galaxy.Host binaries to output"
task :copyHostBinaries do
  solutionpart = "Galaxy.Host"
  if @env_debug_output
    puts "Copying output for Galaxy.Host..."
    puts "#{@env_solutionfolder}/#{solutionpart}/bin/#{@env_buildconfigname} -> #{env_buildfolderpath}/#{solutionpart}/lib"
  end
  FileUtils.mkdir_p("#{env_buildfolderpath}/#{solutionpart}/lib")
  FileUtils.cp_r(
      FileList["#{@env_solutionfolder}/#{solutionpart}/bin/#{@env_buildconfigname}/*.#{solutionpart}.*"], 
      "#{env_buildfolderpath}/#{solutionpart}/lib")
end

desc "Copy Galaxy service binaries to output"
task :copyServiceBinaries do
  solutionpart = "Galaxy"
  if @env_debug_output
    puts "Copying output for Galaxy service..."
    puts "#{@env_solutionfolder}/#{solutionpart}/bin/#{@env_buildconfigname} -> #{env_buildfolderpath}/#{solutionpart}"
  end
  FileUtils.mkdir_p("#{env_buildfolderpath}/#{solutionpart}")
  FileUtils.cp_r(
      FileList["#{@env_solutionfolder}/#{solutionpart}/bin/#{@env_buildconfigname}/*.*"].exclude(/vshost/).exclude("*.xml"), 
      "#{env_buildfolderpath}/#{solutionpart}")
end

desc "Create nuspec file for Galaxy.Host"
nuspec :createHostNuspec do |nuspec|
  solutionpart = "Galaxy.Host"

  puts "Creating .nuspec file for #{solutionpart}..."
  nuspec.id="Codestellation.#{solutionpart}"
  nuspec.version = "#{@env_buildversion_package}"
  nuspec.authors = @env_authors
  nuspec.description = @env_description
  nuspec.title = "#{solutionpart}"
  nuspec.working_directory = "#{env_buildfolderpath}/#{solutionpart}"
  nuspec.output_file = "Codestellation.#{solutionpart}.nuspec"

  get_nuget_dependencies("#{@env_solutionfolder}/#{solutionpart}").each do |dep|
    nuspec.dependency dep.Name, dep.Version
  end
end

desc "Run unit tests"
nunit :runUnitTests do |nunit|
  if @env_debug_output
    puts "Test libs:"
    FileList["#{@env_solutionfolder}/**.Tests/bin/#{@env_buildconfigname}/*.Tests.dll"].exclude(@env_testsexceptions).each do |pathitem|
      puts pathitem
    end
  end
  if @env_is_x64
    nunit.command = "#{@env_nugetpackagesfolder}/NUnit.Runners/tools/nunit-console.exe"
  elsif 
    nunit.command = "#{@env_nugetpackagesfolder}/NUnit.Runners/tools/nunit-console-x86.exe"
  end
  nunit.options "/framework=v4.0.30319","/xml=#{env_buildfolderpath}/NUnit-results-#{@env_projectname}-UnitTests.xml","/nologo"
  nunit.assemblies = FileList["#{@env_solutionfolder}/**.Tests/bin/#{@env_buildconfigname}/*.Tests.dll"].exclude(@env_testsexceptions)
end

desc "Creates artifacts folder"
task :createArtifactsFolder do
  FileUtils.rm_rf(@env_artifactsfolder)
  FileUtils.mkdir_p(@env_artifactsfolder)
end

desc "Create the nuget package for Galaxy.Host"
nugetpack :createHostNuget do |nuget|
    solutionpart = "Galaxy.Host"
    puts "Creating .nuget file for #{solutionpart}..."

    nuget.command     = "#{@env_nugetexefolder}/nuget.exe"
    nuget.nuspec      = "#{env_buildfolderpath}/#{solutionpart}/Codestellation.#{solutionpart}.nuspec"
    nuget.base_folder = "#{env_buildfolderpath}/#{solutionpart}/"
    nuget.output      = "#{@env_artifactsfolder}"
end

desc "Create ZIPs package Galaxy service"
zip :createServiceZip  do |zip|
  solutionpart = "Galaxy"
  puts "Creating .zip file for #{solutionpart}..."

  zip.directories_to_zip "#{env_buildfolderpath}/#{solutionpart}"
  zip.output_path = "#{@env_artifactsfolder}"
  zip.output_file = "Codestellation.#{solutionpart}-#{@env_buildversion_package}.zip"
end