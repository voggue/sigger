param(
    [switch]$incrementVersion = $true
)

function incrementPackageVersion(){
    Write-Host "Increment Package Version";
    $filePath = "$PSScriptRoot\..\directory.build.targets";
    $xml = New-Object XML
    $xml.Load($filePath)

    $element =  $xml.SelectSingleNode("//Version")

    $packageVersionText = $element.InnerText
    Write-Host "OldVersion: $packageVersionText"

    $packageVersion = [version]$packageVersionText
    $newVersion = "{0}.{1}.{2}" -f $packageVersion.Major, $packageVersion.Minor, ([int]$packageVersion.Build + 1)
    Write-Host "NewVersion: $newVersion"

    $element.InnerText = $newVersion
    $xml.Save($filePath)
}

function publish($project) {
    dotnet pack $project
    dotnet publish $project

    # Neueste .nupkg suchen
    $nupkg = (ls $PSScriptRoot\$project\bin\Debug -filter "*.nupkg"|sort lastwritetime -desc|select -first 1)
    if ($nupkg) {
        nuget push -source https://api.nuget.org/v3/index.json $nupkg.fullname
    }
    else {
        write-error ".nupkg file not found for $project"
    }
    
    # Cleanup
    ls $PSScriptRoot\$project\bin\Debug -filter "*.nupkg"|sort lastwritetime -desc|select -skip 1|foreach {
        $t = $_.lastwritetime
        rm $_.fullname
    }
}

if($incrementVersion){
    incrementPackageVersion
}else{
    Write-Host "Version will not be incremented"
}


publish("..\src\Sigger.Generator")
publish("..\src\Sigger.Abstractions")
publish("..\src\Sigger.Extensions")
publish("..\src\Sigger.UI")
