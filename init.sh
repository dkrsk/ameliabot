#!/bin/sh

java -jar ./Lavalink.jar &
sleep 20; ./ameliabot/bin/Debug/net6.0/ameliabot $DTOKEN

wait -n

exit $?

