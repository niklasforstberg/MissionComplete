#!/bin/bash

# Pull latest changes with error checking
echo "Fetching latest changes..."
git fetch origin main
git reset --hard origin/main

# Stop and remove existing container
echo "Stopping and removing existing container..."
docker stop missioncomplete || true
docker rm missioncomplete || true

# Build new image
echo "Building new image..."
docker build -t missioncomplete:latest .

# Run new container
echo "Starting new container..."
docker run -d \
  --name missioncomplete \
  --restart unless-stopped \
  --env-file env.docker \
  -p 8084:8080 \
  missioncomplete:latest

# Clean up old images
echo "Cleaning up old images..."
docker image prune -f

echo "Deployment complete!" 