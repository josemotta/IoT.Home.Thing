FROM microsoft/dotnet:2.0.7-runtime-stretch-arm32v7 AS base
ENV DOTNET_CLI_TELEMETRY_OPTOUT 1
ENV ASPNETCORE_URLS "http://*:5000"
WORKDIR /app

RUN \
  apt-get update \
  && apt-get upgrade -y \
  && apt-get install -y \
       lirc \
  --no-install-recommends && \
  rm -rf /var/lib/apt/lists/*

RUN \
  mkdir -p /var/run/lirc \
  && rm -f /etc/lirc/lircd.conf.d/devinput.*

COPY Lirc/setup/config.txt /boot/config.txt
COPY Lirc/setup/lirc_options.conf /etc/lirc/lirc_options.conf 
COPY Lirc/setup/ir-remote.conf /etc/modprobe.d/ir-remote.conf
COPY Lirc/remotes /etc/lirc/lircd.conf.d
COPY Raspberry.IO/AppConfig.json /app

FROM microsoft/dotnet:2.1-sdk AS build
ENV DOTNET_CLI_TELEMETRY_OPTOUT 1
ENV ASPNETCORE_URLS "http://*:5000"
WORKDIR /src
COPY *.sln ./
COPY *.dcproj ./
COPY src/IO.Swagger/*.csproj src/IO.Swagger/
COPY src/Raspberry.IO/*.csproj src/Raspberry.IO/
COPY src/Raspberry.IO.Components/*.csproj src/Raspberry.IO.Components/
COPY src/Raspberry.IO.GeneralPurpose/*.csproj src/Raspberry.IO.GeneralPurpose/
COPY src/Raspberry.IO.InterIntegratedCircuit/*.csproj src/Raspberry.IO.InterIntegratedCircuit/
COPY src/Raspberry.IO.Interop/*.csproj src/Raspberry.IO.Interop/
COPY src/Raspberry.IO.SerialPeripheralInterface/*.csproj src/Raspberry.IO.SerialPeripheralInterface/
COPY src/Raspberry.System/*.csproj src/Raspberry.System/
RUN dotnet restore src/IO.Swagger/
COPY . .
WORKDIR /src/src/IO.Swagger
RUN dotnet build -c Release -r linux-arm -o /app

FROM build AS publish
RUN dotnet publish -c Release -r linux-arm -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "IO.Swagger.dll"]
