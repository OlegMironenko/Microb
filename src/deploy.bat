@echo Stopping containers
docker-compose down

@echo Build and published images
dotnet publish --os linux --arch x64 /t:PublishContainer -c Release

@echo Build containers
docker-compose build

@echo Start containers
docker-compose up -d