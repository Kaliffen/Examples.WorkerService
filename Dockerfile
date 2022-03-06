#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Examples.WorkerService/Examples.WorkerService.csproj", "Examples.WorkerService/"]
RUN dotnet restore "Examples.WorkerService/Examples.WorkerService.csproj"
COPY . .
WORKDIR "/src/Examples.WorkerService"
RUN dotnet build "Examples.WorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Examples.WorkerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Examples.WorkerService.dll"]