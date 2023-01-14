FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /work

# Add Node.js to build
RUN apt-get update -y
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash --debug
RUN apt-get install nodejs -yq
RUN npm install -g yarn

# Copy everything
COPY . ./

WORKDIR /work/cs/Larp

ENV DOTNET_EnableDiagnostics=0

# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish Larp.WebService/Larp.WebService.csproj -c Release -o /work/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /work
COPY --from=build-env /work/out .
ENTRYPOINT ["dotnet", "Larp.WebService.dll"]
