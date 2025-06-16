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
# --- FIXED: Corrected all project paths to SM.API ---
COPY ["SM.API/SM.API.csproj", "SM.API/"]
COPY ["SM.BAL/SM.BAL.csproj", "SM.BAL/"]
COPY ["SM.DAL/SM.DAL.csproj", "SM.DAL/"]
RUN dotnet restore "./SM.API/SM.API.csproj"
COPY . .

# --- DEBUGGING STEP: List the contents of /src to verify the build context ---
RUN ls -laR /src

# --- FIXED: Corrected work directory path ---
WORKDIR "/src/SM.API"
RUN dotnet build "./SM.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
# --- FIXED: Corrected publish path ---
RUN dotnet publish "./SM.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# --- FINAL FIX: Direct Copy of the Fonts Folder ---
# This command copies the 'fonts' folder directly from your project's source code
# into the final '/app' directory inside the container. This is the most reliable method.
# --- FIXED: Corrected source path for fonts ---
COPY --from=build /src/SM.API/fonts /app/fonts

# --- FIXED: Corrected entrypoint DLL ---
ENTRYPOINT ["dotnet", "SM.API.dll"]
