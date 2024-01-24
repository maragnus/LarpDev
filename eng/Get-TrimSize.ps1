function Get-TotalSizeOfFiles {
    param (
        [string]$Filter,
        [string]$FolderPath
    )

    # Check if the folder path exists
    if (-not (Test-Path -Path $FolderPath -PathType Container)) {
        Write-Error "Folder does not exist: $FolderPath"
        return
    }

    # Get all files with the .br extension
    $brFiles = Get-ChildItem -Path $FolderPath -Filter $Filter -File

    # Initialize a variable to store the total size
    $totalSize = 0

    # Calculate the total size
    foreach ($file in $brFiles) {
        $totalSize += $file.Length
    }

    return $totalSize / 1MB
}

Push-Location (Join-Path $PSScriptRoot "../cs/Larp")
$PublishPath = "Larp.Landing/Client/bin/Release/net7.0/publish"
$FrameworkPath = (Join-Path $PublishPath "wwwroot/_framework")

Remove-Item -Recurse $PublishPath

dotnet publish -c Release

$all = Get-TotalSizeOfFiles "*.*" $FrameworkPath
$br = Get-TotalSizeOfFiles "*.br" $FrameworkPath
$gz = Get-TotalSizeOfFiles "*.gz" $FrameworkPath

Write-Host "BR size: $br"
Write-Host "GZ size: $gz"
Write-Host "Uncompressed size: $($all - $br - $gz)"

Pop-Location