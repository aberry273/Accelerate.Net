# NOT USED - CURRENTLY NOT IN USE
# Overview
This is a service to run locally to enable event bus capabilities through MassTransit
This should be replaced with RabbitMQ in product in a distributed setup
Based off https://learn.microsoft.com/en-us/dotnet/core/extensions/windows-service


# Build/Publish
1. Publish the app as an executable, right click the project and publish to folder

# Creating a windows service
1. Once the exectable is created, add it as a windows service with Powershell (as admin)
> sc.exe create "Accelerate.Services.InMemoryEventBus Service" binpath="C:\Projects\Accelerate.Net\src\Services\Accelerate.Services.InMemoryEventBus\bin\Release\net8.0\win-x64\publish\win-x64\Accelerate.Services.InMemoryEventBus.exe"
2. Run review the service configuration file run
> sc.exe qfailure "Accelerate.Services.InMemoryEventBus Service"
3. To set the recovery configuration use
> sc.exe failure "Accelerate.Services.InMemoryEventBus Service" reset=0 actions=restart/60000/restart/60000/run/1000
4. To start the services
> sc.exe start "Accelerate.Services.InMemoryEventBus Service"
5. To stop the services
> sc.exe stop "Accelerate.Services.InMemoryEventBus Service"
6. To remove the service
sc.exe delete "Accelerate.Services.InMemoryEventBus Service"