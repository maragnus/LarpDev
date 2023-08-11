DOCKERFILE=(Join-Path $PSScriptRoot "../Dockerfile")

docker build -t larpdev -f $DOCKERFILE .
docker network create larpdev-net
docker run -d --rm --name larpdev-mongo --network larpdev-net mongo
open http://localhost:8080
docker run -p 8080:80 -it --rm -e APP_UID 1000 -e MONGO_URL=mongodb://larpdev-mongo -e MONGO_DB=Larp -e ASPNETCORE_URLS=http://+:80 --name larpdev --network larpdev-net larpdev
docker rm -f larpdev-mongo
docker network rm larpdev-net