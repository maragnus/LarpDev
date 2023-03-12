FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Larp.Landing/Client/Larp.Landing.Client.csproj", "Larp.Landing/Client/"]
COPY ["Larp.Landing/Shared/Larp.Landing.Shared.csproj", "Larp.Landing/Shared/"]
RUN dotnet restore "Larp.Landing/Client/Larp.Landing.Client.csproj"
COPY . .
WORKDIR "/src/Larp.Landing/Client"
RUN dotnet build "Larp.Landing.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Larp.Landing.Client.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Larp.Landing.Client.dll"]
