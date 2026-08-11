FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY EmailGateway/*.csproj ./EmailGateway/
COPY MessagingService.API/*.csproj ./MessagingService.API/
COPY MessagingService.Application/*.csproj ./MessagingService.Application/
COPY MessagingService.Domain/*.csproj ./MessagingService.Domain/
COPY MessagingService.Infrastructure/*.csproj ./MessagingService.Infrastructure/

RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet build -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "MessagingService.API.dll"]