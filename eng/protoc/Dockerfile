FROM ubuntu:latest

RUN apt-get update && apt-get install -y unzip

VOLUME [ "/work/in", "/work/out" ]

# https://github.com/protocolbuffers/protobuf/releases
ENV GRPC_WEB_VERSION=21.9

# https://github.com/grpc/grpc-web/releases
ENV GEN_GRPC_VERSION=1.4.1

# https://github.com/protocolbuffers/protobuf-javascript/releases
ENV GEN_JS_VERSION=3.21.2

WORKDIR /work

ADD https://github.com/protocolbuffers/protobuf/releases/download/v${GRPC_WEB_VERSION}/protoc-${GRPC_WEB_VERSION}-linux-x86_64.zip /work/protoc.zip
RUN unzip /work/protoc.zip

ADD https://github.com/grpc/grpc-web/releases/download/${GEN_GRPC_VERSION}/protoc-gen-grpc-web-${GEN_GRPC_VERSION}-linux-x86_64 /work/bin/protoc-gen-grpc-web
RUN chmod +x /work/bin/protoc-gen-grpc-web

ADD https://github.com/protocolbuffers/protobuf-javascript/releases/download/v${GEN_JS_VERSION}/protobuf-javascript-${GEN_JS_VERSION}-linux-x86_64.zip /work/protobuf-javascript.zip
RUN unzip /work/protobuf-javascript.zip

RUN cp /work/bin/* /usr/bin

ADD protoc.sh /work/protoc.sh
RUN chmod +x /work/protoc.sh

CMD [ "bash", "/work/protoc.sh" ]
#ENTRYPOINT [ "bash" ]