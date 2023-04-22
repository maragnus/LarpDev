#/bin/sh
set -e

DOCKERFILE="$(dirname "$0")/../Dockerfile"

docker build -t larpdev -f $DOCKERFILE .
docker network create larpdev-net
docker run -d --rm --name larpdev-mongo --network larpdev-net mongo
open http://localhost:8080
docker run -p 8080:80 -it --rm -e LARPDATA__CONNECTIONSTRING=mongodb://larpdev-mongo -e LARPDATA__DATABASE=Larp -e ASPNETCORE_URLS=http://+:80 --name larpdev --network larpdev-net larpdev
docker rm -f larpdev-mongo
docker network rm larpdev-net