FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Larp.WebService/Larp.WebService.csproj", "Larp.WebService/"]
RUN dotnet restore "Larp.WebService/Larp.WebService.csproj"
COPY . .
WORKDIR "/src/Larp.WebService"
RUN dotnet build "Larp.WebService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Larp.WebService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Larp.WebService.dll"]
