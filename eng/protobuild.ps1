$ProtobufVersion="21.7"
$GenGrpcVersion="1.4.1"

$ProtoDir="../proto/larp"
$TsDir="../ts/proto/larp"

New-Item -Type Directory -Force $TsDir

if (-not (Test-Path "${PWD}\protoc.exe")) {
    $Url="https://github.com/protocolbuffers/protobuf/releases/download/v${ProtobufVersion}/protoc-${ProtobufVersion}-win64.zip"
    Invoke-WebRequest -UseBasicParsing $Url -OutFile "${PWD}\protoc.zip"

    Add-Type -Assembly System.IO.Compression.FileSystem
    $zip = [IO.Compression.ZipFile]::OpenRead("${PWD}\protoc.zip")
    $zip.Entries | Where-Object {$_.Name -like 'protoc.exe'} | ForEach-Object {[System.IO.Compression.ZipFileExtensions]::ExtractToFile($_, "${PWD}\protoc.exe", $true)}
    $zip.Dispose()
    Remove-Item "protoc.zip"
}


if (-not (Test-Path "${PWD}\protoc-gen-grpc-web.exe")) {
    $Url="https://github.com/grpc/grpc-web/releases/download/${GenGrpcVersion}/protoc-gen-grpc-web-${GenGrpcVersion}-windows-x86_64.exe"
    Invoke-WebRequest -UseBasicParsing $Url -OutFile "${PWD}\protoc-gen-grpc-web.exe"
}

Push-Location $ProtoDir
[string[]]$ProtoFiles = Get-ChildItem -File -Recurse | Resolve-Path -Relative | ForEach-Object { $_.Substring(2).Replace("\", "/") }
Pop-Location | Out-Null

& "./protoc.exe" "-I=${ProtoDir}" @ProtoFiles `
  "--grpc-web_out=import_style=typescript,mode=grpcweb:${TsDir}"