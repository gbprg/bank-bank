FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app

COPY *.csproj ./

RUN dotnet restore

COPY . ./

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

ENTRYPOINT [ "dotnet", "transfer_bank.dll" ]