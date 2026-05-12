# Usa la imagen oficial de Microsoft para construir el proyecto (.NET 8)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia todo tu código a la caja
COPY . ./

# Compila el proyecto en modo "Producción" (Release)
RUN dotnet publish -c Release -o out

# Crea la imagen final, más ligera, solo para ejecutar la app
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Expone el puerto 8080 (el estándar para la nube)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Comando para arrancar tu aplicación (Verifica que sea el nombre exacto de tu .dll)
ENTRYPOINT ["dotnet", "GestionProyectosWeb.dll"]