dotnet clean | Out-Null
Get-ChildItem -include bin,obj,packages,'_ReSharper.Caches','.vs',Implementations -Force -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse -ErrorAction SilentlyContinue }
if (Test-Path -Path $env:TEMP\VS\AnalyzerAssemblyLoader) {
    Remove-Item $env:TEMP\VS\AnalyzerAssemblyLoader -Recurse -Force
}
dotnet build-server shutdown
dotnet build
