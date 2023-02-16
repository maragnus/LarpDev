#!/bin/sh

ROOT="$(realpath "$(dirname "$0")/..")"

docker run -t --rm -v $ROOT/proto:/work/in -v $ROOT/ts/landing/src/Protos:/work/out --platform linux/x86_64 $(docker build --platform linux/x86_64 -q .)