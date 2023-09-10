# Usa a imagem oficial do SDK .NET 6 para construir a aplicação
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
# Copia o arquivo .csproj e restaura qualquer dependência
COPY ["AppReadQueueSQS/AppReadQueueSQS.csproj", "AppReadQueueSQS/"]
RUN dotnet restore "AppReadQueueSQS/AppReadQueueSQS.csproj"

# Publica a aplicação para /app/publish
COPY . .
WORKDIR "/src/AppReadQueueSQS"
RUN dotnet build "AppReadQueueSQS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AppReadQueueSQS.csproj" -c Release -o /app/publish

# Usa a imagem oficial do runtime .NET 6 para rodar a aplicação
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppReadQueueSQS.dll"]
