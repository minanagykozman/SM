# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000



# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SM.APP/SM.APP.csproj", "SM.APP/"]
COPY ["SM.BAL/SM.BAL.csproj", "SM.BAL/"]
COPY ["SM.DAL/SM.DAL.csproj", "SM.DAL/"]
RUN dotnet restore "./SM.APP/SM.APP.csproj"
COPY . .
WORKDIR "/src/SM.APP"
RUN dotnet build "./SM.APP.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SM.APP.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# -------------------- FONT INSTALLATION START --------------------
# Switch to root user to install packages
USER root

# Install fontconfig which manages fonts on the system. --no-install-recommends keeps the image lean.
RUN apt-get update && apt-get install -y --no-install-recommends fontconfig

# Create a standard directory for the custom fonts
RUN mkdir -p /usr/share/fonts/truetype/custom

# IMPORTANT: This copies from a 'fonts' folder inside your SM.APP project directory.
# Make sure you have created this folder and placed your .ttf files inside it.
COPY SM.APP/fonts/ /usr/share/fonts/truetype/custom/

# Rebuild the font cache so the system recognizes the new fonts
RUN fc-cache -f -v

# Switch back to the user defined in the base image for security
USER $APP_UID
# -------------------- FONT INSTALLATION END ----------------------

ENTRYPOINT ["dotnet", "SM.APP.dll"]
