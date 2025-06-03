# Stage 1: Build stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy tất cả các project .csproj để restore dependencies
COPY ["NA_Xepthoikhoabieu/*.csproj", "NA_Xepthoikhoabieu/"]
COPY ["NA_Entities/*.csproj", "NA_Entities/"]
COPY ["NA_Logic/*.csproj","NA_Logic/"]

# Restore dependencies (cần có đủ csproj để restore)
RUN dotnet restore "NA_Xepthoikhoabieu/NA_Xepthoikhoabieu.csproj"

# Copy toàn bộ source code của tất cả các project
COPY . .
WORKDIR "/src/NA_Xepthoikhoabieu"
# Build project chính
RUN dotnet build "NA_Xepthoikhoabieu.csproj" -c $BUILD_CONFIGURATION -o /app/build
FROM build AS publish 
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NA_Xepthoikhoabieu.csproj" -c $BUILD_CONFIGURATION -o /app/publish
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "NA_Xepthoikhoabieu.dll" ]