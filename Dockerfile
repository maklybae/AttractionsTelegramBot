FROM ubuntu:22.04 AS builder

# install the .NET 6 SDK from the Ubuntu archive
# (no need to clean the apt cache as this is an unpublished stage)
RUN apt-get update && apt-get install -y dotnet6 ca-certificates

# add your application code (the HelloWorld example from above)
WORKDIR /source
COPY . .

# publish your .NET app
RUN dotnet publish -c Release -o /app

FROM ubuntu/dotnet-runtime:6.0-22.04_beta

WORKDIR /app
COPY --from=builder /app ./

ENTRYPOINT ["dotnet", "/app/TelegramBot.dll"]
