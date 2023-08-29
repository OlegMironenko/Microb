FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Rozum.BrainTrainer.Api/Rozum.BrainTrainer.Api.csproj", "Rozum.BrainTrainer.Api/"]
RUN dotnet restore "Rozum.BrainTrainer.Api/Rozum.BrainTrainer.Api.csproj"
COPY . .
WORKDIR "/src/Rozum.BrainTrainer.Api"
RUN dotnet build "Rozum.BrainTrainer.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Rozum.BrainTrainer.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Rozum.BrainTrainer.Api.dll"]
