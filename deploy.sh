#!/bin/bash

# Stop and remove existing container
docker stop missioncomplete || true
docker rm missioncomplete || true

# Pull latest changes
git pull origin main

# Build new image
docker build -t missioncomplete:latest .

# Run new container
docker run -d \
  --name missioncomplete \
  --restart unless-stopped \
  --env-file env.docker \
  -p 8085:80 \
  missioncomplete:latest

# Clean up old images
docker image prune -f 