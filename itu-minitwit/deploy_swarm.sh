#!/usr/bin/env bash

#turn docker engine into swarm mode
docker swarm init

if [[ $1 == "build" ]]; then
    docker compose build
fi

docker stack deploy minitwit -c ./infrastructure/docker_swarm/local_stack/minitwit_stack.yml --detach=false 
echo "go to MiniTwit: https://minitwit.localhost"
echo "go to Grafana: http://127.0.0.1:3000"
echo "go to Alloy UI: http://127.0.0.1:12345"