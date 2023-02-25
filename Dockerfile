FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /SignalRClient
RUN apt-get update
RUN apt-get install -y python3
RUN dotnet workload install wasm-tools
COPY ["SignalRClient/SignalRClient.csproj", "SignalRClient/"]
RUN dotnet restore "SignalRClient/SignalRClient.csproj"
COPY . .

FROM build AS publish
WORKDIR /SignalRClient
RUN dotnet publish -c Release -o /app/publish/

FROM nginx:alpine AS final
 
EXPOSE 80
EXPOSE 443
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY "nginx.conf" /etc/nginx/nginx.conf

