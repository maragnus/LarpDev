#!/bin/bash

set -euo pipefail

echo "Identifying .proto files"
pushd /work/in > /dev/null
PROTOS=`find . -type f -name "*.proto" | sed 's!\./!!' | tr '\n' ' '`
popd > /dev/null

echo "Generating protos..."
protoc --plugin="$TS_PROTO_GEN_BIN" \
    -I=/work/in $PROTOS \
    --ts_proto_opt=enumsAsLiterals=true \
    --ts_proto_opt=useSnakeTypeName=false \
    --ts_proto_opt=useAsyncIterable=true \
    --ts_proto_opt=lowerCaseServiceMethods=true \
    --ts_proto_opt=env=browser \
    --ts_proto_out=import_style=commonjs,binary:/work/out \

echo "Finished"
