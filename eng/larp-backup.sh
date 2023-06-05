#!/bin/bash

PASSWORD=""

set -euox pipefail

rm -rf ~/backup/larp ~/backup/larp.tgz
mkdir -p ~/backup/larp

docker exec -it mongodb rm -rf /dump/larp
docker exec -it mongodb mongodump --authenticationDatabase=admin --uri mongodb://larpUser:$PASSWORD@localhost/larp?ssl=false\&authSource=admin
docker cp mongodb:/dump/larp ~/backup
pushd ~/backup/larp
tar -czf ~/backup/larp.tgz ./
popd