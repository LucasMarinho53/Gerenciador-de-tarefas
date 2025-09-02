FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TaskManagementApi.csproj", "."]
RUN dotnet restore "./TaskManagementApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "TaskManagementApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskManagementApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManagementApi.dll"]

