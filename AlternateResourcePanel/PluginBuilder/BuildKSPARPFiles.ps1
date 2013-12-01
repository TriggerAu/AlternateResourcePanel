param([String]$VersionString)
#param([String]$VersionString = $(throw "-VersionString is required so we know what folder to build.`r`n`r`n"))

#Run by powershell BuildKACFiles.ps1 -VersionString "X.X.X.X"
#  or it will try and read the dll version

$SourcePath= "D:\Programming\KSP\KSPAlternateResourcePanel\DevBranch\Source"
$DestRootPath="D:\Programming\KSP\KSPAlternateResourcePanelUpload"
$7ZipPath="c:\Program Files\7-Zip\7z.exe" 

if ($VersionString -eq "")
{
	$dll = get-item "$SourcePath\bin\Debug\KSPAlternateResourcePanel.dll"
	$VersionString = $dll.VersionInfo.ProductVersion
}

if ($VersionString -eq "")
{
	throw "No version read from the dll and no -VersionString provided so we don't know what folder to build.`r`n`r`n"

}

$DestFullPath= "$($DestRootPath)\v$($VersionString)"


"`r`nThis will build v$($VersionString) of the Kerbal Alarm Clock"
"`tFrom:`t$($SourcePath)"
"`tTo:`t$($DestFullPath)"
$Choices= [System.Management.Automation.Host.ChoiceDescription[]] @("&Yes","&No")
$ChoiceRtn = $host.ui.PromptForChoice("Do you wish to Continue?","This will erase any existing $($VersionString) Folder",$Choices,1)

if($ChoiceRtn -eq 0)
{
    "Here Goes..."

    if(Test-Path -Path $DestFullPath)
    {
        "Deleting $DestFullPath"
        Remove-Item -Path $DestFullPath -Recurse

    }

    #Create the folders
    "Creating Folders..."
    New-Item $DestRootPath -name "v$($VersionString)" -ItemType Directory

    #Dont create this or it will copy the files into a subfolder
    #New-Item $DestFullPath -name "KSPAlternateResourcePanel_$($VersionString)" -ItemType Directory
    New-Item $DestFullPath -name "KSPAlternateResourcePanelSource_$($VersionString)" -ItemType Directory

    #Copy the items 
    "Copying Plugin..."
    Copy-Item "$SourcePath\PluginFiles" "$($DestFullPath)\KSPAlternateResourcePanel_$($VersionString)" -Recurse
    Copy-Item "$SourcePath\bin\Debug\KSPAlternateResourcePanel.dll" "$($DestFullPath)\KSPAlternateResourcePanel_$($VersionString)\GameData\TriggerTech" 
    #Update the Text files with the version String
    (Get-Content "$($DestFullPath)\KSPAlternateResourcePanel_$($VersionString)\info.txt") |
        ForEach-Object {$_ -replace "%VERSIONSTRING%",$VersionString} |
            Set-Content "$($DestFullPath)\KSPAlternateResourcePanel_$($VersionString)\info.txt"
    (Get-Content "$($DestFullPath)\KSPAlternateResourcePanel_$($VersionString)\ReadMe-KSPAlternateResourcePanel.txt") |
        ForEach-Object {$_ -replace "%VERSIONSTRING%",$VersionString} |
            Set-Content "$($DestFullPath)\KSPAlternateResourcePanel_$($VersionString)\ReadMe-KSPAlternateResourcePanel.txt"
	Move-Item "$($DestFullPath)\KSPAlternateResourcePanel_$($VersionString)\ReadMe-KSPAlternateResourcePanel.txt" "$($DestFullPath)\KSPAlternateResourcePanel_$($VersionString)\GameData\TriggerTech\"

    #Copy the source files
    "Copying Source..."
    Copy-Item "$SourcePath\*.cs"  "$($DestFullPath)\KSPAlternateResourcePanelSource_$($VersionString)"
    Copy-Item "$SourcePath\*.csproj"  "$($DestFullPath)\KSPAlternateResourcePanelSource_$($VersionString)"
    New-Item "$DestFullPath\KSPAlternateResourcePanelSource_$($VersionString)\" -name "Properties" -ItemType Directory
    Copy-Item "$SourcePath\Properties\*.cs" "$($DestFullPath)\KSPAlternateResourcePanelSource_$($VersionString)\Properties\"
    

    # Now Zip it up

    & "$($7ZipPath)" a "$($DestFullPath)\KSPAlternateResourcePanel_$($VersionString).zip" "$($DestFullPath)\KSPAlternateResourcePanel_$($VersionString)" -xr!"info.txt"
	& "$($7ZipPath)" a "$($DestFullPath)\KSPAlternateResourcePanel_$($VersionString).zip" "$($DestFullPath)\KSPAlternateResourcePanel_$($VersionString)\info.txt"
	& "$($7ZipPath)" a "$($DestFullPath)\KSPAlternateResourcePanel_$($VersionString).zip" "$($DestFullPath)\KSPAlternateResourcePanel_$($VersionString)\GameData\TriggerTech\ReadMe-KSPAlternateResourcePanel.txt"
    & "$($7ZipPath)" a "$($DestFullPath)\KSPAlternateResourcePanelSource_$($VersionString).zip" "$($DestFullPath)\KSPAlternateResourcePanelSource_$($VersionString)" 
}

else
{
    "Skipping..."
}

