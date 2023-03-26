docker build -t larpdev -f Dockerfile .

docker run -p 8080:80 -it --rm -e LARPDATA__CONNECTIONSTRING=mongodb://host.docker.internal:27017 -e LARPDATA__DATABASE=Larp -e ASPNETCORE_URLS=http://+:80 --name larpdev larpdev