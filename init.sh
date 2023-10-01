#!/bin/sh

java -jar ./Lavalink.jar &
sleep 20; ./ameliabot/bin/Debug/net7.0/ameliabot $DTOKEN

wait -n

exit $?

