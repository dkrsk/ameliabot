FROM alpine
RUN apk update && apk upgrade && \
	apk add dotnet6-sdk openjdk11 curl build-base gcompat
COPY . ./app
WORKDIR /app

RUN dotnet build -c Debug 
#idk why Release was't work

RUN curl -L -0 https://github.com/freyacodes/Lavalink/releases/download/3.7.4/Lavalink.jar -o ./Lavalink.jar && \
	curl https://pastebin.com/raw/b9Y8RYGq -o ./application.yml

EXPOSE 2334
RUN chmod +x ./init.sh

ENTRYPOINT ["./init.sh"]

