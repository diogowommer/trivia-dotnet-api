FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["TriviaDotNetApi.API/TriviaDotNetApi.API.csproj", "TriviaDotNetApi.API/"]
RUN dotnet restore "TriviaDotNetApi.API/.Personnel.API.csproj"
COPY . .
WORKDIR /src/TriviaDotNetApi.API
RUN dotnet build "TriviaDotNetApi.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TriviaDotNetApi.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TriviaDotNetApi.API.dll"]