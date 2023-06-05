if (-not (Test-Path larp.tgz)) {
    Write-Error "larp.tgz does not exist in current path"
    exit
}

$tmp = (Join-Path ([System.IO.Path]::GetTempPath()) ([System.Guid]::NewGuid()))
New-Item -ItemType Directory -Path $tmp | Out-Null

try 
{
    tar xf larp.tgz -C $tmp

    ls $tmp
    
    & "C:\Program Files\MongoDB\Tools\100\bin\mongorestore.exe" `
        --db Larp $tmp --drop
}
finally {
    Remove-Item -Recurse $tmp
}
