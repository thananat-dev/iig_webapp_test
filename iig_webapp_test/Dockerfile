#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["iig_webapp_test/iig_webapp_test.csproj", "iig_webapp_test/"]
RUN dotnet restore "iig_webapp_test/iig_webapp_test.csproj"
COPY . .
WORKDIR "/src/iig_webapp_test"
RUN dotnet build "iig_webapp_test.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "iig_webapp_test.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "iig_webapp_test.dll"]