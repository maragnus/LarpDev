#!/bin/bash

set -euo pipefail

pushd /work/in
PROTOS=`find . -type f -name "*.proto" | sed 's!\./!!' | tr '\n' ' '`
popd 

echo "Generating protos..."
protoc -I=/work/in $PROTOS --js_out=import_style=commonjs,binary:/work/out --grpc-web_out=import_style=typescript,mode=grpcweb:/work/out
echo "Finished"