#!/bin/bash

# Stop and remove existing container
sudo docker stop missioncomplete || true
sudo docker rm missioncomplete || true

# Pull latest changes
sudo git pull origin main

# Build new image
sudo docker build -t missioncomplete:latest .

# Run new container
sudo docker run -d \
  --name missioncomplete \
  --restart unless-stopped \
  --env-file env.docker \
  -p 8084:8080 \
  missioncomplete:latest

# Clean up old images
sudo docker image prune -f 