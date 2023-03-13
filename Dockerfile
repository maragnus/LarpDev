FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ARG LARPDATA__CONNECTIONSTRING
ARG LARPDATA__DATABASE

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

ARG BUILD_REVISION

# Add Node.js to build
RUN apt-get update -y
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash --debug
RUN apt-get install nodejs -yq
RUN npm install -g pnpm

# Update npm packages first (changes rarely)
WORKDIR /src
COPY ["ts/landing/package.json", "ts/landing/package.json"]
COPY ["ts/landing/pnpm-lock.yaml", "ts/landing/pnpm-lock.yaml"]
WORKDIR /src/ts/landing
RUN pnpm install

# Update nuget packages first (changes rarely)
WORKDIR /src
RUN mkdir -p cs/Larp
COPY ["cs/Larp/Larp.sln", "cs/Larp/Larp.sln"]
COPY ["cs/Larp/Larp.Common/Larp.Common.csproj", "cs/Larp/Larp.Common/Larp.Common.csproj"]
COPY ["cs/Larp/Larp.Data/Larp.Data.csproj", "cs/Larp/Larp.Data/Larp.Data.csproj"]
COPY ["cs/Larp/Larp.Data.Mongo/Larp.Data.Mongo.csproj", "cs/Larp/Larp.Data.Mongo/Larp.Data.Mongo.csproj"]
COPY ["cs/Larp/Larp.Data.Seeder/Larp.Data.Seeder.csproj", "cs/Larp/Larp.Data.Seeder/Larp.Data.Seeder.csproj"]
COPY ["cs/Larp/Larp.Data.TestFixture/Larp.Data.TestFixture.csproj", "cs/Larp/Larp.Data.TestFixture/Larp.Data.TestFixture.csproj"]
COPY ["cs/Larp/Larp.Data.Tests/Larp.Data.Tests.csproj", "cs/Larp/Larp.Data.Tests/Larp.Data.Tests.csproj"]
COPY ["cs/Larp/Larp.Notify/Larp.Notify.csproj", "cs/Larp/Larp.Notify/Larp.Notify.csproj"]
COPY ["cs/Larp/Larp.Test.Common/Larp.Test.Common.csproj", "cs/Larp/Larp.Test.Common/Larp.Test.Common.csproj"]
COPY ["cs/Larp/Larp.Landing/Shared/Larp.Landing.Shared.csproj", "cs/Larp/Larp.Landing/Shared/Larp.Landing.Shared.csproj"]
COPY ["cs/Larp/Larp.Landing/Server/Larp.Landing.Server.csproj", "cs/Larp/Larp.Landing/Server/Larp.Landing.Server.csproj"]
COPY ["cs/Larp/Larp.Landing/Client/Larp.Landing.Client.csproj", "cs/Larp/Larp.Landing/Client/Larp.Landing.Client.csproj"]
RUN dotnet restore "cs/Larp/Larp.sln"

# Perform build
COPY . .

RUN sh /src/eng/update-revision.sh $BUILD_REVISION
RUN dotnet publish cs/Larp/Larp.Landing/Server/Larp.Landing.Server.csproj -c Release -o /src/publish

# Run web service
FROM base AS final
WORKDIR /app
COPY --from=build /src/publish .
#ENV ASPNETCORE_URLS http://+:80
ENTRYPOINT ["dotnet", "Larp.Landing.Server.dll"]
