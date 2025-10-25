# Inspection Visits — Backend (.NET 8)

## Quick Start (Docker)
```bash
cd deploy
docker compose up --build
# API at http://localhost:8080 (Swagger /swagger when Development)
```
Default SQL password in docker-compose: `Your_strong_password123`

## Local Run
1. Ensure SQL Server is running (local or Docker).
2. Update `appsettings.json` connection string.
3. Run:
```bash
dotnet restore
dotnet run --project src/InspectionVisits.Api
```

## Demo Users
- Admin: `admin@demo.local` / `P@ssw0rd!`
- Inspector: `inspector@demo.local` / `P@ssw0rd!`

## Endpoints (Summary)
- `POST /api/auth/login` → { token }
- **Entities (Admin)**:
  - `GET /api/entities?category=`
  - `POST /api/entities`
  - `PUT /api/entities/{id}`
  - `DELETE /api/entities/{id}`
- **Visits**:
  - `GET /api/visits?from=&to=&status=&inspectorId=&category=`
  - `POST /api/visits/schedule` (Admin)
  - `POST /api/visits/{id}/status` (Inspector)
  - `POST /api/visits/{id}/violations` (Inspector)
- **Dashboard**:
  - `GET /api/dashboard` (Any authenticated)

## Non-Functional
- JWT Auth, role-based
- FluentValidation in handlers (pipeline)
- Serilog (console + rolling file)
- Swagger/OpenAPI
- ProblemDetails (RFC7807)

## Postman
Import `postman/InspectionVisits.postman_collection.json`. Login first, then set `token` variable.
