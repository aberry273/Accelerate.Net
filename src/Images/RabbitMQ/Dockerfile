FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src.csproj", "src/"]
RUN dotnet restore "src/src.csproj"

COPY . src/.
RUN dotnet publish -c Release -o /app src/src.csproj 

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS publish
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "src.dll"]