# Appointments API

Minimal .NET 8 Web API for managing appointments.

## Time tracking
Backend+Testing: 3h | Frontend: 1h | Documentation: 1.5h

## Deployment
The frontend and backend are deployed independently:
the backend API runs on Azure App Service, while the frontend is hosted on Azure Static Web Apps.

This approach enables independent scaling, faster deployments, lower cost, improved security, and better alignment with cloud-native Azure services.

A single Docker image was intentionally avoided to prevent unnecessary coupling and resource waste.
