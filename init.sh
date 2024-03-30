#!/bin/sh

java -jar ./Lavalink.jar &
sleep 5; ./ameliabot/bin/Release/net8.0/ameliabot $DTOKEN

wait -n

exit $?

