Push-Location (Join-Path $PSScriptRoot "..")

"RUN mkdir -p \"
(@() + (Get-ChildItem -Recurse -Include *.sln) + (Get-ChildItem -Recurse -Include *.csproj)) |`
    ForEach-Object { Split-Path $_.FullName -Parent } | `
    Resolve-Path -Relative | `
    ForEach-Object { $_.Replace("\", "/") } | `
    ForEach-Object { "  ""$($_.Substring(2))"" \" }

(@() + (Get-ChildItem -Recurse -Include *.sln) + (Get-ChildItem -Recurse -Include *.csproj)) |`
    Resolve-Path -Relative | `
    ForEach-Object { $_.Replace("\", "/") } | `
    ForEach-Object { "COPY [""$($_.Substring(2))"", ""$($_.Substring(2))""]" }

Pop-Location