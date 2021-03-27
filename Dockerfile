#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["WebService/WebService.csproj", "WebService/"]
COPY ["DomainModels/DomainModels.csproj", "DomainModels/"]
COPY ["Entities/Entities.csproj", "Entities/"]
COPY ["Services.Abstractions/Services.Abstractions.csproj", "Services.Abstractions/"]
COPY ["Repository/Repository.csproj", "Repository/"]
COPY ["Services/Services.csproj", "Services/"]
RUN dotnet restore "WebService/WebService.csproj"
COPY . .
WORKDIR "/src/WebService"
RUN dotnet build "WebService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebService.dll"]