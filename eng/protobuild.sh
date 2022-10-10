#!/bin/sh

PROTOBUF_VERSION=21.7
GEN_GRPC_VERSION=1.4.1

PROTO_DIR=../proto/larp
TS_DIR=../ts/proto/larp

set -eu # exit on error, fail on unset variables

mkdir -p "$TS_DIR"

ARCH=$(uname -m)
case "$(uname -s)" in
    Linux*) 
        OS=linux
        PROTOC_BIN='protoc'
        PROTOBUF_SUFFIX="${OS}-${ARCH}"
        GEN_GRPC_SUFFIX="${OS}-${ARCH}.exe"
        GEN_GRPC_BIN="protoc-gen-grpc-web";;
    Darwin*)
        OS=osx
        PROTOC_BIN='protoc'
        PROTOBUF_SUFFIX="${OS}-${ARCH}"
        GEN_GRPC_SUFFIX="${OS}-${ARCH}.exe"
        GEN_GRPC_BIN="protoc-gen-grpc-web";;   
    MINGW64*) 
        OS=win
        PROTOC_BIN='protoc.exe'
        PROTOBUF_SUFFIX="win64"
        GEN_GRPC_SUFFIX="windows-x86_64.exe"
        GEN_GRPC_BIN="protoc-gen-grpc-web.exe";;
    *)
        echo "Unknown OS: $(uname -s)"
        exit 1;;
esac

if [ ! -f $PROTOC_BIN ]; then
    echo "Updating PROTOC"
    PROTOBUF_URL="https://github.com/protocolbuffers/protobuf/releases/download/v${PROTOBUF_VERSION}/protoc-${PROTOBUF_VERSION}-${PROTOBUF_SUFFIX}.zip"
    TMPFILE=`mktemp`
    curl -Ls "$PROTOBUF_URL" -o $TMPFILE
    unzip -p $TMPFILE "bin/${PROTOC_BIN}" > "protoc.exe"
    rm -f $TMPFILE
fi

if [ ! -f $GEN_GRPC_BIN ]; then
    echo "Updating GRPC Web"
    GEN_GRPC_URL="https://github.com/grpc/grpc-web/releases/download/${GEN_GRPC_VERSION}/protoc-gen-grpc-web-${GEN_GRPC_VERSION}-${GEN_GRPC_SUFFIX}"
    curl -Ls "$GEN_GRPC_URL" -o $GEN_GRPC_BIN
fi

PROTOC_BIN="$PWD/$PROTOC_BIN"

pushd $PROTO_DIR
PROTOS=`find . -type f -name "*.proto" | sed 's!\./!!' | tr '\n' ' '`
popd

find $TS_DIR -type f -name '*.ts' -delete

echo $PROTOS

$PROTOC_BIN -I=$PROTO_DIR $PROTOS \
  --grpc-web_out=import_style=typescript,mode=grpcweb:$TS_DIR