docker build -t protoc $PWD\protoc

$ProtoDir="../proto"
$TsDir="../ts/landing/src/Protos"

$ProtoDir = "/" + (Resolve-Path $ProtoDir).Path.Replace(":", "").Replace("\", "/") + ":/work/in"
$TsDir = "/" + (Resolve-Path $TsDir).Path.Replace(":", "").Replace("\", "/") + ":/work/out"

$ProtoDir
$TsDir

docker run -it -v $ProtoDir -v $TsDir grpc