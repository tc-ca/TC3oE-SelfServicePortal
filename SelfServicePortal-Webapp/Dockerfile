# https://docs.microsoft.com/en-us/dotnet/core/docker/build-container?tabs=linux
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy everything
COPY . .

# Restore as distinct layers
RUN dotnet restore SelfServicePortal.Web
# Build and publish a release
RUN dotnet publish SelfServicePortal.Web -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /SelfServicePortal.Web
COPY --from=build-env /app/out .

# Declare ports above 1024 as an unprivileged non-root user cannot bind to > 1024
# https://stackoverflow.com/a/62886597/11141271
USER 1001
ENV ASPNETCORE_URLS http://+:8000
EXPOSE 8000

# Fix issue with read-only file system
# > Failed to create CoreCLR, HRESULT: 0x80004005
# https://github.com/dotnet/runtime/issues/46093#issuecomment-873351388
ENV COMPlus_EnableDiagnostics=0

ENTRYPOINT ["dotnet", "SelfServicePortal.Web.dll"]