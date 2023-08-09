FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj and restore for both projects
COPY ["Pacer.Data/Pacer.Data.csproj", "Pacer.Data/"]
COPY ["Pacer.Web/Pacer.Web.csproj", "Pacer.Web/"]
RUN dotnet restore "Pacer.Web/Pacer.Web.csproj"

# Copy all the source files and build
COPY . .
WORKDIR "/src/Pacer.Web"
RUN dotnet build "Pacer.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pacer.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pacer.Web.dll"]