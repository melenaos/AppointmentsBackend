# Appointments API

Minimal .NET 8 Web API for managing appointments.

UI Repo: https://github.com/melenaos/AppointmentsUi

## Time tracking
Backend+Testing: 3h | Frontend: 4h | Documentation: 1.5h

## Developing + Project evaluation (Run instructions)
This project is splitted into two repositories, one for UI and one for API.
Both need to be downloaded and run seperatelly.

### API
- Open the project in Visual studio 
- Run

The api will start on:
`https://localhost:5001`

The project includes an .http file that can be used to manually test the API endpoints at 
`/Http/Appointments.http`


### UI
- cd AppointmentsUi
- npm install
- npm run dev

The app will be available at:
`http://localhost:55001`


## Deployment
The frontend and backend are deployed independently:
the backend API runs on Azure App Service, while the frontend is hosted on Azure Static Web Apps.

This approach enables independent scaling, faster deployments, lower cost, improved security, and better alignment with cloud-native Azure services.

A single Docker image was intentionally avoided to prevent unnecessary coupling and resource waste.
