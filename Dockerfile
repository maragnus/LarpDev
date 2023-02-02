FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ARG LARPDATA__CONNECTIONSTRING
ARG LARPDATA__DATABASE

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Add Node.js to build
RUN apt-get update -y
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash --debug
RUN apt-get install nodejs -yq
RUN npm install -g yarn

# Update npm packages first (changes rarely)
WORKDIR /src
COPY ["ts/landing/package.json", "ts/landing/package.json"]
COPY ["ts/landing/yarn.lock", "ts/landing/yarn.lock"]
WORKDIR /src/ts/landing
RUN yarn install

# Update nuget packages first (changes rarely)
WORKDIR /src
RUN mkdir -p cs/Larp
COPY ["cs/Larp/Larp.sln", "cs/Larp/Larp.sln"]
COPY ["cs/Larp/Larp.Common/Larp.Common.csproj", "cs/Larp/Larp.Common/Larp.Common.csproj"]
COPY ["cs/Larp/Larp.Data/Larp.Data.csproj", "cs/Larp/Larp.Data/Larp.Data.csproj"]
COPY ["cs/Larp/Larp.Data.Seeder/Larp.Data.Seeder.csproj", "cs/Larp/Larp.Data.Seeder/Larp.Data.Seeder.csproj"]
COPY ["cs/Larp/Larp.Data.TestFixture/Larp.Data.TestFixture.csproj", "cs/Larp/Larp.Data.TestFixture/Larp.Data.TestFixture.csproj"]
COPY ["cs/Larp/Larp.Data.Tests/Larp.Data.Tests.csproj", "cs/Larp/Larp.Data.Tests/Larp.Data.Tests.csproj"]
COPY ["cs/Larp/Larp.Notify/Larp.Notify.csproj", "cs/Larp/Larp.Notify/Larp.Notify.csproj"]
COPY ["cs/Larp/Larp.Protos/Larp.Protos.csproj", "cs/Larp/Larp.Protos/Larp.Protos.csproj"]
COPY ["cs/Larp/Larp.Test.Common/Larp.Test.Common.csproj", "cs/Larp/Larp.Test.Common/Larp.Test.Common.csproj"]
COPY ["cs/Larp/Larp.WebService/Larp.WebService.csproj", "cs/Larp/Larp.WebService/Larp.WebService.csproj"]
COPY ["cs/Larp/Larp.WebService.Tests/Larp.WebService.Tests.csproj", "cs/Larp/Larp.WebService.Tests/Larp.WebService.Tests.csproj"]
RUN dotnet restore "cs/Larp/Larp.sln"

# Perform build
COPY . .

RUN dotnet publish cs/Larp/Larp.WebService/Larp.WebService.csproj -c Release -o /src/publish

# Run web service
FROM base AS final
WORKDIR /app
COPY --from=build /src/publish .
#ENV ASPNETCORE_URLS http://+:80
ENTRYPOINT ["dotnet", "Larp.WebService.dll"]
