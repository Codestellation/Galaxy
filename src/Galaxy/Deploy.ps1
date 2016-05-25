#requires -version 4.0
#requires –runasadministrator

Param(
    [int]$WebPort = 8088,
    [string]$RootPath = "D:\",
    [string]$App = "services",
    [string]$Configs = "configs",
    [string]$Logs = "logs",
    [string]$Data = "data"
)


function Goto-ScriptLocation
{
    Set-Location $PSScriptRoot
}

function Combine-Path([string] $folder)
{
    return ([System.IO.Path]::Combine($RootPath, $folder, $galaxyName));
}

function Ensure-Folder([string] $fullPath)
{
    if(Test-Path $fullPath)
    {
        write "Folder '$fullPath' already exists"
    }
    else
    {
        write "Making folder '$fullPath'";
        New-Item $fullPath -Type directory
    }
}

function Create-Folders
{
    if([System.IO.Path]::IsPathRooted($RootPath))
    {
        Ensure-Folder ($appFolder);
        Ensure-Folder ($configsFolder);
        Ensure-Folder ($logsFolder);
        Ensure-Folder ($dataFolder);
    }
}

function Stop-Service
{
    if(Test-Path $serviceExe)
    {
        write "Found '$serviceExe'. Stopping service"
        & $serviceExe stop
    }

}

function Copy-Files
{
    $files =  Get-ChildItem -Exclude deploy.ps1,nlog.config;

    foreach($file in $files)
    {
        Copy-Item $file $appFolder -Force -Verbose
    }

}

function Deploy-HostConfig
{
    $hostConfig = @{logs="$logsFolder";data="$dataFolder";configs="$configsFolder"} | ConvertTo-Json
    $hostConfigPath = Join-Path  $appFolder host.config.json
    $hostConfig | Out-File $hostConfigPath

    write "";
    Write "Generated host config at '$hostConfigPath':"
    write $hostConfig
}

function Deploy-Configs
{
    $utf8WithoutBom = New-Object System.Text.UTF8Encoding($false)

    $appConfig = @{webPort="$WebPort"} | ConvertTo-Json
    $appConfigPath = Join-Path $configsFolder config.json
    $appConfig | Out-File $appConfigPath

    write "";
    Write "Generated service config at '$appConfigPath':";
    write $appConfig;


    [xml]$nlogContent = Get-Content nlog.config
    $nlogSecion = $nlogContent.LastChild
    $rootVariable = $nlogSecion.FirstChild;

    $rootVariable.SetAttribute("value", $logsFolder)
    
    $nlogPath = Join-Path $configsFolder nlog.config
    $streamWriter = New-Object System.IO.StreamWriter($nlogPath, $false, $utf8WithoutBom)
    $nlogContent.Save($streamWriter)
    $streamWriter.Close()
}

function Install-Service
{
    & $serviceExe install
}

function Start-Service
{
    & $serviceExe start
}

function Write-Success
{
    Write "";
    write "Service probably installed"
    $url = "http://" + $env:COMPUTERNAME + ":" + $WebPort;
    Write "Checkout $url for success" 
}

$galaxyName = "galaxy";
$appFolder = Combine-Path $App;
$configsFolder = Combine-Path $Configs;
$logsFolder = Combine-Path $Logs;
$dataFolder = Combine-Path $Data
$serviceExe = Join-Path $appFolder "Codestellation.Galaxy.Exe"

Goto-ScriptLocation
Create-Folders
Stop-Service
Copy-Files
Deploy-HostConfig
Deploy-Configs
Install-Service
Start-Service
Write-Success