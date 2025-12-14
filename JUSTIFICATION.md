# Design Justification Document

## Architecture Overview
The application follows a layered architecture with clear separation of concerns.

### Layers

Controllers -> Services -> Repositories

#### Controllers (API Layer)
- Handle Http requests
- Call application services
- Does not contain any business logic

#### Services (Applicatoin Layer)
- Contain business rules and validations
- Call Repositories for data persistance
- Transforms data to and from DAL using DTOs
- Manage complete procedures such as Create Appointment

### Repositories (Data Access layer)
- Data storage and retrieval

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

 ### Integration testing
 This is a valuable testing way to validate that the system works.
 They verify end-to-end flows, while unit test validates each component works correctly, 
 integration tests validate that these components have been correctly wired together.

 ## Trade-offs and Omissions
 
 ### Logging
 I should have setup Serilog for log piping and implement unhandle exception logging.
 The most import part of monitoring in th Azure ecosystme is Azure Insights.