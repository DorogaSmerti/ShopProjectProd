FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY . ./


RUN dotnet restore "MyFirstProject.csproj"

RUN dotnet publish "MyFirstProject.csproj" -c Release -o /out

WORKDIR /out

ENTRYPOINT [ "dotnet", "MyFirstProject.dll" ]