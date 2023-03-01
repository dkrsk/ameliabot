FROM alpine
RUN apk update && apk upgrade && \
	apk add dotnet6-sdk openjdk11 curl
COPY . ./app
WORKDIR /app

RUN dotnet build -c Release

RUN mkdir Lavalink & \
	curl -L -0 https://github.com/freyacodes/Lavalink/releases/download/3.7.4/Lavalink.jar -o ./Lavalink/Lavalink.jar && \
	curl https://pastebin.com/raw/b9Y8RYGq -o ./Lavalink/application.yml

EXPOSE 2334
RUN chmod +x ./init.sh

ENTRYPOINT ["./init.sh"]
 
