require 'albacore'

# --------------------------------------------------------------------------------------------------------------------
# build variables

@projectname = "Galaxy"

@script_description = "rake automated build script for #{@projectname}"
@script_version = "script version 1.0"

# relative path to .sln file
@solutionfolder = "./src"

@nugetpackagesfolder = "#{@solutionfolder}/packages"
@nugetexefolder = "tools"
@authors = "Codestellation Team"
@description = "Windows hosting service"

# default parameters, actual version is build using git describe command
@buildversion_native = "1.0"
@buildversion_package = "1.0.0"
@buildversion_full = "1.0.0"

@buildconfigname = "Release"

@testsexceptions = ""

@buildsfolder = "build"
@artifactsfolder = "#{@buildsfolder}/artifacts"

@debug_output = true



def projectfullname
  "v#{@buildversion_full}__#{@buildconfigname}"
end

def buildfolderpath
  "#{@buildsfolder}/#{projectfullname}"
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
  puts "Solution binaries in #{buildfolderpath}"
end

desc "Restore packages"
exec :installNUnitRunners do |cmd|
  cmd.command = "./#{@nugetexefolder}/NuGet.exe"
  cmd.parameters = "restore #{@solutionfolder}/#{@projectname}.sln"
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

  @buildversion_native = version.native
  @buildversion_package = version.package
  @buildversion_full = version.full
  puts "Build version detected as #{@buildversion_full}"
end

task :buildAll do
  Rake::Task["buildSolution"].execute
end

desc "Clean and build the solution"
msbuild :buildSolution do |msb|
  msb.properties :configuration => @buildconfigname
  msb.targets :Clean, :Build
  msb.solution = "#{@solutionfolder}/#{@projectname}.sln"
  msb.verbosity = "quiet"
  msb.parameters = "/nologo"
end

desc "Creates clean build folder structure"
task :createCleanBuildFolder do
  FileUtils.rm_rf(@buildsfolder)
  FileUtils.mkdir_p("#{@buildsfolder}")
end

desc "Copy Galaxy.Host binaries to output"
task :copyHostBinaries do
  solutionpart = "Galaxy.Host"
  if @debug_output
    puts "Copying output for Galaxy.Host..."
    puts "#{@solutionfolder}/#{solutionpart}/bin/#{@buildconfigname} -> #{buildfolderpath}/#{solutionpart}/lib"
  end
  FileUtils.mkdir_p("#{buildfolderpath}/#{solutionpart}/lib")
  FileUtils.cp_r(
      FileList["#{@solutionfolder}/#{solutionpart}/bin/#{@buildconfigname}/*.#{solutionpart}.*"], 
      "#{buildfolderpath}/#{solutionpart}/lib")
end

desc "Copy Galaxy service binaries to output"
task :copyServiceBinaries do |msb|
  
  solutionpart = "Galaxy"
  if @debug_output
    puts "Copying output for Galaxy service..."
    puts "#{@solutionfolder}/#{solutionpart}/bin/#{@buildconfigname} -> #{buildfolderpath}/#{solutionpart}"
  end
  FileUtils.rm_rf("#{buildfolderpath}/#{solutionpart}")
  FileUtils.mkdir_p("#{buildfolderpath}/#{solutionpart}")
  
  service_source = "#{@solutionfolder}/#{solutionpart}/bin/#{@buildconfigname}/*";
  files_to_copy = FileList.new(service_source)
  files_to_copy.exclude(/.xml/)
  files_to_copy.exclude(/vshost/)

  Dir.glob(service_source).select {|f| File.directory?(f) }.each do |dir|
    files_to_copy.exclude(dir)
  end

  FileUtils.cp_r(files_to_copy, "#{buildfolderpath}/#{solutionpart}")
end

desc "Create nuspec file for Galaxy.Host"
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
end

desc "Run unit tests"
nunit :runUnitTests do |nunit|
  if @debug_output
    puts "Test libs:"
    FileList["#{@solutionfolder}/**.Tests/bin/#{@buildconfigname}/*.Tests.dll"].exclude(@testsexceptions).each do |pathitem|
      puts pathitem
    end
  end

  nunit.command = Dir.glob("#{@nugetpackagesfolder}/**/nunit-console.exe").first;

  nunit.options "/framework=v4.0.30319","/xml=#{buildfolderpath}/NUnit-results-#{@projectname}-UnitTests.xml","/nologo"
  nunit.assemblies = FileList["#{@solutionfolder}/**.Tests/bin/#{@buildconfigname}/*.Tests.dll"].exclude(@testsexceptions)
end

desc "Creates artifacts folder"
task :createArtifactsFolder do
  FileUtils.rm_rf(@artifactsfolder)
  FileUtils.mkdir_p(@artifactsfolder)
end

desc "Create the nuget package for Galaxy.Host"
nugetpack :createHostNuget do |nuget|
    solutionpart = "Galaxy.Host"
    puts "Creating .nuget file for #{solutionpart}..."

    nuget.command     = "#{@nugetexefolder}/nuget.exe"
    nuget.nuspec      = "#{buildfolderpath}/#{solutionpart}/Codestellation.#{solutionpart}.nuspec"
    nuget.base_folder = "#{buildfolderpath}/#{solutionpart}/"
    nuget.output      = "#{@artifactsfolder}"
end

desc "Create ZIPs package Galaxy service"
zip :createServiceZip  do |zip|
  solutionpart = "Galaxy"
  puts "Creating .zip file for #{solutionpart}..."

  zip.directories_to_zip "#{buildfolderpath}/#{solutionpart}"
  zip.output_path = "#{@artifactsfolder}"
  zip.output_file = "Codestellation.#{solutionpart}-#{@buildversion_package}.zip"
end