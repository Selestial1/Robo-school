FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY server/RoboSchool.csproj ./server/
RUN dotnet restore ./server/RoboSchool.csproj
COPY server/ ./server/
RUN dotnet publish ./server/RoboSchool.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
COPY index.html admin.html status.html ./site/
COPY assets ./site/assets/
COPY fonts ./site/fonts/
ENV SITE_ROOT=/app/site
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
CMD ["dotnet", "RoboSchool.dll"]
