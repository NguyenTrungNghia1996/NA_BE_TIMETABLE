# Stage 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy tất cả các project .csproj để restore dependencies
COPY NA_Xepthoikhoabieu/*.csproj ./NA_Xepthoikhoabieu/
COPY NA_Entities/*.csproj ./NA_Entities/
COPY NA_Logic/*.csproj ./NA_Logic/

# Restore dependencies (cần có đủ csproj để restore)
RUN dotnet restore NA_Xepthoikhoabieu/NA_Xepthoikhoabieu.csproj

# Copy toàn bộ source code của tất cả các project
COPY NA_Xepthoikhoabieu/. ./NA_Xepthoikhoabieu/
COPY NA_Entities/. ./NA_Entities/
COPY NA_Logic/. ./NA_Logic/

# Build project chính
RUN dotnet build NA_Xepthoikhoabieu/NA_Xepthoikhoabieu.csproj -c Release -o /app/build

# Publish project chính
RUN dotnet publish NA_Xepthoikhoabieu/NA_Xepthoikhoabieu.csproj -c Release -o /app/publish --no-self-contained

# Stage 2: Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy file đã publish từ build stage
COPY --from=build /app/publish .

# Khai báo entrypoint chạy app
ENTRYPOINT ["dotnet", "NA_Xepthoikhoabieu.dll"]
