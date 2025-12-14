# Design Justification Document

## Architecture Overview
The application follows a layered architecture with clear separation of concerns.

### Layers
Controllers -> Services -> Repositories

#### Controllers (API Layer)
- Handle Http requests
- Call application services
- Does not contain any business logic
- Must not reference DAL directly

#### Services (Applicatoin Layer)
- Contain business rules and validations
- Call Repositories for data persistance
- Transforms data to and from DAL using DTOs
- Manage complete procedures such as Create Appointment

### Repositories (Data Access layer)
- Data storage and retrieval
- Must not use DTOs
- No buisiness logic here

### Design principles
- Seperation of concerns
- Inversion of Control using DI
- Thin controllers
- Testable business logic

### Libraly and Folder structure

[src]
|- Appointments.Api
|  |- Controllers
|  |- ViewModels (used only in the Controllers for responses)
|  |- Http (Contains logically grouped test requests | Smoke tests)
|
|- [Libraries]
|  |- Dal
|  |  |- Entities (The Data Storage Entities, must match the database)
|  |  |- Repositories (each different Entity must have it's own repo)
|  |
|  |- Application
|  |  |- Dtos
|  |  |- Services (seperate the business requirements by locical groups, not by db tables)
|  |
|  |- DiConfig (Gathers the DI configuration from all the libraries)
|
|- [Tests]
|  |- Application.Tests

All libraries must contain Models, Interfaces, Concreate classes for their functionality.
The Interfaces are in a `Abstructions` folder. 
Each Libary is responsible to provide their DI config in a `static IServiceCollection Add...(this IServiceCollection services)` procedure.


## Design Choices

### Why seperate API and Domain models?
The application intentionally separates API models (DTOs) from domain models.  
This is a deliberate design choice to improve long-term maintainability and flexibility.

API contracts might contain more information that the domain model needs.
UI specific data or user configurable format (local datetime), versioning, UX information, etc

Domain might contain sensitive data that we don't want to expose to the UI.
DTO is lighter by not including irrelevant domain data.

They can evolve seperately without having to propage the changes to the oposite layer.
We can apply versioning to the DTOs without breaking existing consumers.

Unfortunately there is mapping overhead, but it's a small sacrifice for having all the above advantages.

### Why there are two seperate repositories for backend and frontend?
Frontend and backedn should stay independent because they usually evolve at a different speed.
we might see frontend iterate really fast while we need the backend to remain stable.
Different team don't want to step on each other. Independent deployment, see the next section.
Different tooling, CI pipelines won't have to deal with differnt technologies.
Team boundaries, backend team owns the API and frontent team owns the UI.
Better and faster code review, less merge conflicts.

### Independent Deployment for backend and frontend
Using separate deployments for backend and frontend is a cloud-native design choice that optimizes scalability, cost, security, and operational simplicity.

Backend has long running processes, requires CPU,Memory, and Storage for been stateful.
While frontend has prebuild static files, usually served over CDN wihtout needing any server runtime.

With seperate hosting you are paying only for backend computing, frontend is almost free since it's served through CND.

When the traffic hits, the scalling is completely different for frontend and backend.
If we serve the fronent from a docker image, we have to scale out to handle increased traffic while the backend might not need it.

Independent deployment can keep the server running while the frontend is updating a minor ui fix.
and the server updates doesn't invalidate the frontend caching.

Azure is build exactly for this setup, the App Services resource is packed with free tooling.
- Zero infrastructure management
  - no vm, no OS maintencance, no container orhestration required
  - slot-based deployments
- Build-in observability
  - Application insights
  - Live log streaming
  - Metrics alerts, dashboards
  - Distributed tracing
- Security
  - Https enforced
  - Managed identity
  - Key vault and Managed Configuration

Azure App Service is purpose-built for hosting APIs and provides extensive built-in tooling for deployment, monitoring, security, and scaling.

By deploying the backend as an App Service and the frontend as a Static Web App, the system leverages Azure’s strengths instead of recreating platform features inside a Docker image.

## Testing Philosophy
We prioterize Unit test over integration tests
### Unit tests
Unit tests can execute fast and frequent without the need to bring the system to a running state.
There is no need to have networks, http requests, time heavy executions or warm ups.
 
We can use them to create a test first coding aproach, and isolate the business logic and test throughfully.
When a unit test fails it points exactly to the problematic location and it's easy and fast to debug using breakpoints.

Maybe the biggest gain from attending the unit test approach is that if forces writting better code:
The unit testing needs a strong seperation of concerns, DI, thin controllers.

They are great to use in an automation action before commiting, mergin or releasing a new version.
We don't test DTO mapping, framework code, actual Service call and DB access.

### Integration testing
This is a valuable testing way to validate that the system works.
They verify end-to-end flows, while unit test validates each component works correctly, 
integration tests validate that these components have been correctly wired together.


## Trade-offs and Omissions
 
### Logging
I should have setup Serilog for log piping.

### Authentication
No authentication/authorization is implemented.

### Data Persistance
A Real database is missing, but the infrastructure is already there. 
Implement the file `/Libraries/Dal/Repositories/AppointmentsRepositor.cs` with actual db implementation 
and update the Dal's DependencyInjection to use the real implementation.

### Localization
The validation messages are not localized.

### Metrics
Imlement Application Insights to gather metrics about the application health.

### Integration testing
While unit-tests are ok, we need also some integration testing.

### Rate limiting, abuse pervation
If this is a pulbic tool, we might want to apply some rate limiting to avoid abuse.

### CI/CD 

## Deployment
For this project I wouldn't use containers. I propose to deploy it at Azure App Services.
 
- create a new App Service. Note the [app name] and download the publishing profile of the staging slot.
- Add the following action at the project's github workflows folder.
- Add a new secret at the github repository named AZURE_WEBAPP_PUBLISH_PROFILE  with the publishing project.


```
name: Build and Deploy to Azure App Service

on:
  push:
    branches:
      - main

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout source
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --configuration Release --no-restore

      - name: Run tests
        run: dotnet test --configuration Release --no-build

      - name: Publish
        run: dotnet publish -c Release -o ./publish

      - name: Deploy to Azure Web App
        uses: azure/webapps-deploy@v3
        with:
          slot-name: staging
          app-name: [app name]
          publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
          package: ./publish


```

### Requriments
Directly commiting to the main branch should be forbidden and applied with authorization restriction.
Every feature should be implemented using branches and merged to a Version branch.
Only when the Version is ready we can merge it to main. That will trigger publishing to the stage environment.
When Staging is tested, we can switch the staging to production.
