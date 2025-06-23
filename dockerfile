# Use the official .NET runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Use the official .NET SDK as a build image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Eagles_Portal.csproj", "."]
RUN dotnet restore "Eagles_Portal.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "Eagles_Portal.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Eagles_Portal.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Eagles_Portal.dll"]