# Dakar Rally Simulation

REST API in ASP.NET CORE

## Description

Implementation of Dakar Rally real-time simulation. Simulation has cars, trucks and motorbikes as participants.

## Usage

1. API is hosted on: https://localhost:5001/api/

2. Json POSTMAN file DakarRallyCollection.postman_collection.json is in the root folder of the project. This file contains all examples.

3. Swagger documentation: https://localhost:5001/swagger/index.html

## Notes

* 1 hour is considered as 1 second in simulation.
* Leaderboard can be presented only when race is in running state.
* In the simulation it is considered that the vehicles are always moving at maximum speed.
