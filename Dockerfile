FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Conduit.Person.WebApi/Conduit.Person.WebApi.csproj", "Conduit.Person.WebApi/"]
RUN dotnet restore "Conduit.Person.WebApi/Conduit.Person.WebApi.csproj"
COPY . .
WORKDIR "/src/Conduit.Person.WebApi"

ARG CONFIG=Debug
ENV CONFIG=$CONFIG

RUN dotnet build "Conduit.Person.WebApi.csproj" -c $CONFIG -o /app/build

FROM build AS publish

ARG CONFIG=Debug
ENV CONFIG=$CONFIG

RUN dotnet publish "Conduit.Person.WebApi.csproj" -c $CONFIG -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Conduit.Person.WebApi.dll"]
