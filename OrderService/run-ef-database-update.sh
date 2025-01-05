#!/bin/bash
# for the DB to be fully startet and ready for changes if any
sleep 30

cd /app/OrderService
dotnet tool install --global dotnet-ef
/root/.dotnet/tools/dotnet-ef database update