@echo off
dotnet build . --configuration Release
dotnet publish . -p:PublishProfile=TourismSmartTransportation.API\Properties\PublishProfiles\tourism-smart-transportation-api-WebDeploy.pubxml
if %errorlevel%==0 (echo Done!) else (echo Failed!)
pause