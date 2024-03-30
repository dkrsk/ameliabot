FROM alpine
RUN apk update && apk upgrade && \
	apk add openjdk17 curl build-base gcompat bash icu
	
RUN mkdir -p /usr/share/dotnet \
	&& ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet 
RUN wget https://dot.net/v1/dotnet-install.sh && \
	chmod +x ./dotnet-install.sh && \
	./dotnet-install.sh --install-dir /usr/share/dotnet

COPY . ./app
WORKDIR /app

RUN dotnet restore
RUN dotnet build -c Release
#idk why Release was't work

RUN curl -L -0 https://github.com/ZeyoYT/Lavalink/releases/download/Fixed/Lavalink.jar -o ./Lavalink.jar && \
	curl https://pastebin.com/raw/2a1ZE9fp -o ./application.yml

EXPOSE 2334
RUN chmod +x ./init.sh

ENTRYPOINT ["./init.sh"]

