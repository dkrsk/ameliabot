#!/bin/sh

java -jar ./Lavalink/Lavalink.jar &
sleep 15; ./ameliabot/bin/Release/net6.0/ameliabot $DTOKEN

wait -n

exit $?

