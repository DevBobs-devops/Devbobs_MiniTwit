#!/usr/bin/env bash

#turn docker engine into swarm mode
docker swarm init

if [[ $1 == "build" ]]; then
    docker compose build
fi

docker stack deploy minitwit -c ./infrastructure/docker_swarm/local_stack/minitwit_stack.yml --detach=false 
echo "go to: http://127.0.0.1:8080"
