FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY [".FactoryExecution.API/.FactoryExecution.API.csproj", ".FactoryExecution.API/"]
RUN dotnet restore ".FactoryExecution.API/.FactoryExecution.API.csproj"
COPY . .
WORKDIR /src/.FactoryExecution.API
RUN dotnet build ".FactoryExecution.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish ".FactoryExecution.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", ".FactoryExecution.API.dll"]
