# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /serviceorderupdate

EXPOSE 82
EXPOSE 5002

COPY ./*.csproj ./
RUN dotnet restore 

COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/sdk:8.0 
WORKDIR /serviceorderupdate
COPY --from=build /serviceorderupdate/out .
ENTRYPOINT ["dotnet", "services-order-update.dll"]
