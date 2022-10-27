#!/bin/bash

pushd /work/in
PROTOS=`find . -type f -name "*.proto" | sed 's!\./!!' | tr '\n' ' '`
popd

protoc -I=/work/in $PROTOS --js_out=import_style=commonjs,binary:/work/out --grpc-web_out=import_style=typescript,mode=grpcweb:/work/out
