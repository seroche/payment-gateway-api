# Build and deploy Domain NuGet packages
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine3.10 AS base

WORKDIR /src
COPY ./src .

# Build Application to deploy
RUN dotnet publish Checkout.PaymentGateway.Api.csproj -c Release -o /app -v q

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine3.10 AS runtime
EXPOSE 80

WORKDIR /app
COPY --from=base /app .

# Required by EF Core
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "Checkout.PaymentGateway.Api.dll"]
