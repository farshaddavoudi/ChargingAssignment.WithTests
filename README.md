# Charging Assignment with Tests

## Overview
This project is a comprehensive assignment featuring robust unit, integration, and end-to-end tests to showcase best testing practices and methodologies.

## Features
- Develop an API exposing a simplified smart charging domain.
- Manage Groups, Charge Stations, and Connectors with full CRUD operations.
- Ensure all operations adhere to strict functional requirements.
- Extensive testing coverage for all functionalities.

## Domain:
<img src="https://github.com/farshaddavoudi/ChargingAssignment.WithTests/blob/main/docs/domain-model.png" alt="domain-model">

## Functional Requirements
1. Create, update, and remove Group/Charge Station/Connector.
2. Removing a Group also removes all associated Charge Stations.
3. Add/remove only one Charge Station to/from a Group per call.
4. Charge Stations can only belong to one Group at a time.
5. Connectors can only exist within a Charge Station.
6. Update the Max current in Amps of existing Connectors.
7. Group Capacity in Amps must be greater than or equal to the sum of the Max current in Amps of all Connectors in the Group.
8. Reject operations/requests not meeting the above requirements.

## How to Run/Start the Project:
First, the database needs to be created and migrations to be applied. 

> Before creating the database and applying migrations, ensure that you have the necessary SQL Server instance accessible. If your SQL Server instance is hosted on 
the cloud or running within a Docker container, simply modify the connection string in the appsettings.json file to point 
to your SQL Server instance.

> The default one is set on *locally* installed SQL Server instance

It can be done following below steps:

### – Modify the connection string _(if necessary)_

```
// appsettings.json file
"AppDbConnStr": "Data Source=localhost;Initial Catalog=CharginAssignment;Trusted_Connection=True;TrustServerCertificate=True;"
```

### – Apply migrations 

This can be done in two ways:

1. **Visual Studio**: Open *Package Manager Console* of Visual Studio. Set *Default project* on `*.Infratructure` and *Startup project* on `*.API` one.
Then run the update database command: `Update-Database`

2. **.NET CLI**: In Windows Explorer, go to the `{project-path}/src/CharginAssignment.WithTests.Infrastructure` path, then open 
a command line there and run this command: `dotnet ef database update -s  "..\CharginAssignment.WithTests.Web.API"`

#### _Run the project by `dotnet run` or via Visual Studio, and the Swagger page will pop up as a sign that the application has started._

## Project Architecture:

*Clean Architecture* has been implemented with the following projects:

- API (WebAPI project)
- Application (Class Library)
- Infrastructure (Class Library)
- Domain (Class Library)

Also, the CQRS pattern is followed by using the `MediatR` library.

## Project Technical Description:

Use .NET 8, EF Core (ORM), SQL Server as the data store, AutoMapper for mappings, Unit/Integration
tests using `xUnit/Moq/FluentAssertions/Bogus`, In Memory DB for Integration Tests, Logging with Serilog configured the log file as target sink.
