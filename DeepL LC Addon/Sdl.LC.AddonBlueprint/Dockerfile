#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Sdl.LC.AddonBlueprint/Sdl.LC.AddonBlueprint.csproj", "Sdl.LC.AddonBlueprint/"]
RUN dotnet restore "Sdl.LC.AddonBlueprint/Sdl.LC.AddonBlueprint.csproj"
COPY . .
WORKDIR "/src/Sdl.LC.AddonBlueprint"
RUN dotnet build "Sdl.LC.AddonBlueprint.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sdl.LC.AddonBlueprint.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sdl.LC.AddonBlueprint.dll"]