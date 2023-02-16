FROM node:slim
ENV NPM_CONFIG_LOGLEVEL info

RUN apt-get update && apt-get install -y unzip

WORKDIR /work

# https://github.com/protocolbuffers/protobuf/releases
ENV PROTOC_VERSION=21.9 
ADD https://github.com/protocolbuffers/protobuf/releases/download/v${PROTOC_VERSION}/protoc-${PROTOC_VERSION}-linux-x86_64.zip /work/protoc.zip
RUN unzip /work/protoc.zip && mv /work/bin/protoc /bin

RUN npm install ts-proto
ENV TS_PROTO_GEN_BIN="/work/node_modules/ts-proto/protoc-gen-ts_proto"

ADD protoc.sh /work/protoc.sh

CMD [ "/bin/bash", "/work/protoc.sh" ]