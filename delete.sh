#!/bin/bash

docker stop game_db
docker stop game_backend
docker stop game_frontend
docker rm game_db
docker rm game_backend
docker rm game_frontend
docker rmi postgres
docker rmi tictactoewasm-game_frontend
docker rmi tictactoewasm-game_backend
docker volume rm tictactoewasm_game_data
docker network rm tictactoewasm_default