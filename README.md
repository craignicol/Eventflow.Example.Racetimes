# Racetimes example for Eventflow

This project contains an example project that uses some basic and extended features.

Basic features:
- Command / CommandHandler
- Event
- Identity
- AggregateRoot
- ReadModel

Configuration:
- MSSQL
- EntityFramework
- EventFlowOptions
- Migration

Extended features:
- Entity (within AggregateRoot)
- ReadModel for an Entity
- Delete on ReadModel
- Snapshots

# Getting Started

## Requirements

* .Net Core SDK 
* SQL Server, with a database named `TimesEF`
* Run the 2 `Create*.sql` scripts in the root folder

# Racetimes (Domain)

The domain of this project is storing times from races within competitions. Therefore competitions can be created, renamed and deleted. Racetimes (Entries) can be added and changed. These actions are far from complete but I think they are sufficient for an example.

# Note

This is still a work in progress but I enjoy hearing from you (especially feedback on points I missed, got wrong or could do better).

This is a fork of [dennisfabri's example](https://github.com/craignicol/Eventflow.Example.Racetimes.git) adapted to clean-up the start-up and to work with Azure Functions

# Azure Event Grid configuration

Create an Event Grid Topic in your Azure subscription, then create a secrets.json file as follows :

```
{
  "EventGrid": {
    "Endpoint": "https://<TOPIC>.<REGION>.eventgrid.azure.net/api/events",
    "ApiKey": "<YOUR_API_KEY>",
    "TopicRoot": "/subscriptions/<YOUR_SUBSCRIPTION>/resourceGroups/<YOUR_RESOURCE_GROUP>/providers/Microsoft.EventGrid/topics/"
  }
}
```

# Sources

The code is based on the official documentation as well as code from the tests within the eventflow repository.
