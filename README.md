# IoT.Home.Thing

#### IoT Home Intelligence with Raspberry Pi

### Home of Things based on Raspberry Pi, Linux, Swagger, Docker & .NET Core.

## Introduction

After building IoT embryos at [IoT.Starter.Pi.Thing](https://github.com/josemotta/IoT.Starter.Pi.Thing) series, we are now ready to climb one more step towards IoT Home Intelligence. The development strategy and environment are based on the following:  

- **Raspberry Pi**: Raspbian GNU/Linux 9.1 Stretch Lite is installed at Raspberry Pi (RPI) host and [debian:stretch-slim](https://github.com/dotnet/dotnet-docker/blob/master/2.1/runtime/stretch-slim/arm32v7/Dockerfile) equip docker images based on [.NET Core 2.1 Preview 1](https://blogs.msdn.microsoft.com/dotnet/2018/02/27/announcing-net-core-2-1-preview-1/). More details about RPI setup is available [here](https://github.com/josemotta/IoT.Starter.Pi.Thing/wiki/2.-IoT.Starter.Pi.Thing#2-setup) and also at [wiki](https://github.com/josemotta/IoT.Starter.Pi.Thing/wiki/RPI-Setup).
- **Swagger**: the API First Design strategy is used to develop an ASP.NET Core Web Server automatically generated by SwaggerHub. The [Thing specification](https://github.com/josemotta/IoT.Starter.Pi.Thing/wiki/2.-IoT.Starter.Pi.Thing#1-specs) exhibits the minimal requirements for `Thing` embryos used at Home Intelligence applications. After each change on the API, SwaggerHub automatically generates updated code to build the `home-srv` docker image that is finally pushed to DockerHub.
- **Docker**: A multi-stage docker image build is accomplished at a speedy Windows x64 machine, generating code of linux-arm framework. The images are pushed to the cloud and then pulled back into the Raspberry Pi with Linux. Executed at both x64 and linux-arm sides, the docker-compose command helps to build, deploy and run containers properly.
- **Lirc**: IoT.Home.Thing, powered by [RemoteAPI](https://app.swaggerhub.com/apis/motta/home/1.0.2#/Remote), emulates legacy infrared remote controls. The Linux Infrared Remote Control for Raspberry Pi is installed at both RPI host and `home-srv` container, extending the `Thing` with IR remotes and their respective IR codes.

## Start the repository for a new project

This is an opportunity to apply the [`IoT.Starter.Pi.Thing`](https://github.com/josemotta/IoT.Starter.Pi.Thing) to build the repository for this new project. The first step is just copying the whole  source code as a starter for `IoT.Home.Thing`. Then, `home-compose.yml` is created, based on previous files as follows, :

	version: "3"
	
	services:
	  io.swagger:
	    container_name: home-srv
	    image: josemottalopes/home-srv
	    build:
	      context: .
	      dockerfile: Lirc/srv.Dockerfile
	    ports:
	    - "5000:5000"
	    network_mode: bridge
	    privileged: true
	    restart: always
	    devices:
	      - /dev/mem:/dev/mem
	    volumes:
	    - /var/run/lirc:/var/run/lirc
	    environment:
	      - ASPNETCORE_ENVIRONMENT=Release
	
	  home.ui:
	    container_name: home-cli
	    image: josemottalopes/home-cli
	    build:
	      context: .
	      dockerfile: src/Home.UI/cli.Dockerfile
	    ports:
	    - "80:80"
	    network_mode: bridge
	    restart: always
	    environment:
	      - ASPNETCORE_ENVIRONMENT=Release
	 
	  ssl.proxy:
	    container_name: ssl-proxy
	    image: josemottalopes/home-ssl
	    build:
	      context: .
	      dockerfile: Proxy/proxy.Dockerfile
	    ports:
	    - "443:443"
	    network_mode: bridge
	    restart: always

## Raspberry# IO

The current [Raspberry# IO](https://github.com/raspberry-sharp/raspberry-sharp-io) is a .NET/Mono IO Library for Raspberry Pi, an initiative of the [Raspberry# Community](http://www.raspberry-sharp.org/). It was [updated](https://github.com/Ramon-Balaguer/raspberry-sharp-io) to .Net Standard 1.6 compatibility by Ramon Balaguer. Targeting RPI projects with .NET Core 2, this same library code was upgraded again, now to [.NET Core 2.1 Preview 1](https://blogs.msdn.microsoft.com/dotnet/2018/02/27/announcing-net-core-2-1-preview-1/).

The following modules are available, as you can check at [home/src](https://github.com/josemotta/IoT.Home.Thing/tree/master/home/src), the tests were not included at this time. 

- **Raspberry.System**: include definitions for processor, board, models, etc. Support for RPi3/BCM2835 was added, based on this [post](https://github.com/raspberry-sharp/raspberry-sharp-io/issues/88) from Michaltalaga;
- **Raspberry.IO**: inludes basic I/O for input & output of digital & analog pins;
- **Raspberry.IO.GeneralPurpose**: access Raspberry Pi GPIO pins through memory with support for edge detection, allowing sub-millisecond polling of input pins;
- **Raspberry.IO.Interop**: Linux I/O control device and memory management;
- **Raspberry.IO.SerialPeripheralInterface**: provides preliminary support for SPI,  using Linux's kernel SPI module driver;
- **Raspberry.IO.InterIntegratedCircuit**: provides preliminary support for I2C;
- **Raspberry.IO.Components**: provides preliminary support for various components, including HC-SR04 distance detector that will be used as an example.



*Did you like it? Please give me a :star:!*