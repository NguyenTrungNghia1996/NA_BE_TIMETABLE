FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore
COPY NA_Xepthoikhoabieu/*.csproj ./NA_Xepthoikhoabieu/
RUN ls -l NA_Xepthoikhoabieu
RUN dotnet restore NA_Xepthoikhoabieu/NA_Xepthoikhoabieu.csproj

# Copy all source code
COPY NA_Xepthoikhoabieu/. ./NA_Xepthoikhoabieu/

# Build
RUN dotnet build NA_Xepthoikhoabieu/NA_Xepthoikhoabieu.csproj -c Release -o /app/build

# Publish
RUN dotnet publish NA_Xepthoikhoabieu/NA_Xepthoikhoabieu.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NA_Xepthoikhoabieu.dll"]
