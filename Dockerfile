# FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
# WORKDIR /app

# # Copy everything
# COPY . ./
# # Restore as distinct layers
# RUN dotnet restore
# # Build and publish a release
# WORKDIR /app/VeeGroupsApi
# RUN dotnet publish -c Release -o out

## Either uncoment things above or do this commend
# dotnet publish -c Release

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app

COPY VeeGroupsApi/nlog.config.xml .

# depending on what you are choosing above uncomment and comment these thins))

# COPY --from=build-env /app/VeeGroupsApi/out .
COPY VeeGroupsApi/bin/Release/net5.0/publish .


EXPOSE 80
ENTRYPOINT ["dotnet", "VeeGroupsApi.dll"]
