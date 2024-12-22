FROM mcr.microsoft.com/dotnet/sdk:8.0

# Clean APT cache and update
RUN apt-get clean && rm -rf /var/lib/apt/lists/* \
    && apt-get update \
    && apt-get install -y netcat-openbsd

WORKDIR /app

COPY . .

# Specify the correct path for the script
RUN chmod +x OrderService/run-ef-database-update.sh

ENTRYPOINT ["/bin/sh", "/app/OrderService/run-ef-database-update.sh"]