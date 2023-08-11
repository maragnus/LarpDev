FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
EXPOSE 80
EXPOSE 443
ARG APP_UID
ARG MONGO_URL
ARG MONGO_DB
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_REVISION
WORKDIR /src

# Perform nuget restore first (changes rarely)
COPY \
  ["cs/Larp/Larp.sln", "cs/Larp/Larp.sln"] \
  ["cs/Larp/KiloTx.Restful.ClientGenerator/KiloTx.Restful.ClientGenerator.csproj", "cs/Larp/KiloTx.Restful.ClientGenerator/KiloTx.Restful.ClientGenerator.csproj"] \
  ["cs/Larp/KiloTx.Restful.Server/KiloTx.Restful.Server.csproj", "cs/Larp/KiloTx.Restful.Server/KiloTx.Restful.Server.csproj"] \
  ["cs/Larp/KiloTx.Restful/KiloTx.Restful.csproj", "cs/Larp/KiloTx.Restful/KiloTx.Restful.csproj"] \
  ["cs/Larp/Larp.Common/Larp.Common.csproj", "cs/Larp/Larp.Common/Larp.Common.csproj"] \
  ["cs/Larp/Larp.Data.Mongo/Larp.Data.Mongo.csproj", "cs/Larp/Larp.Data.Mongo/Larp.Data.Mongo.csproj"] \
  ["cs/Larp/Larp.Data.Seeder/Larp.Data.Seeder.csproj", "cs/Larp/Larp.Data.Seeder/Larp.Data.Seeder.csproj"] \
  ["cs/Larp/Larp.Data.TestFixture/Larp.Data.TestFixture.csproj", "cs/Larp/Larp.Data.TestFixture/Larp.Data.TestFixture.csproj"] \
  ["cs/Larp/Larp.Data.Tests/Larp.Data.Tests.csproj", "cs/Larp/Larp.Data.Tests/Larp.Data.Tests.csproj"] \
  ["cs/Larp/Larp.Data/Larp.Data.csproj", "cs/Larp/Larp.Data/Larp.Data.csproj"] \
  ["cs/Larp/Larp.Landing/Client.RestClient/Larp.Landing.Client.RestClient.csproj", "cs/Larp/Larp.Landing/Client.RestClient/Larp.Landing.Client.RestClient.csproj"] \
  ["cs/Larp/Larp.Landing/Client/Larp.Landing.Client.csproj", "cs/Larp/Larp.Landing/Client/Larp.Landing.Client.csproj"] \
  ["cs/Larp/Larp.Landing/Server/Larp.Landing.Server.csproj", "cs/Larp/Larp.Landing/Server/Larp.Landing.Server.csproj"] \
  ["cs/Larp/Larp.Landing/Shared/Larp.Landing.Shared.csproj", "cs/Larp/Larp.Landing/Shared/Larp.Landing.Shared.csproj"] \
  ["cs/Larp/Larp.Notify/Larp.Notify.csproj", "cs/Larp/Larp.Notify/Larp.Notify.csproj"] \
  ["cs/Larp/Larp.Test.Common/Larp.Test.Common.csproj", "cs/Larp/Larp.Test.Common/Larp.Test.Common.csproj"] \
RUN dotnet restore "cs/Larp/Larp.sln"

# Perform build
COPY ["cs", "cs"] 
COPY ["eng/update-revision.sh", "eng/update-revision.sh"]
RUN sh /src/eng/update-revision.sh $BUILD_REVISION
RUN dotnet publish cs/Larp/Larp.Landing/Server/Larp.Landing.Server.csproj -c Release -o /src/publish

# Run web service
FROM base AS final
WORKDIR /app
COPY --from=build /src/publish .
ENV ASPNETCORE_URLS http://+:80
ENV LARPDATA__CONNECTIONSTRING $MONGO_URL
ENV LARPDATA__DATABASE $MONGO_DB
USER $APP_UID
ENTRYPOINT ["dotnet", "Larp.Landing.Server.dll"]
