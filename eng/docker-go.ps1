docker build -t larpdev -f Dockerfile .

docker run -p 80:80 -p 443:443 -e DOTNET_LARPDATA_CONNECTIONSTRING=mongodb://host.docker.internal:27017
