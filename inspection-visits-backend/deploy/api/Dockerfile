# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ./src ./src
COPY ./InspectionVisits.sln ./
RUN dotnet restore
RUN dotnet publish src/InspectionVisits.Api/InspectionVisits.Api.csproj -c Release -o /app

# Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "InspectionVisits.Api.dll"]
