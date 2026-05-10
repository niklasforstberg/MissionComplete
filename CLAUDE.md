# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

MissionComplete is a coaching platform where coaches assign challenges and goals to sports teams during the off-season. It's a full-stack app: ASP.NET Core 9 Minimal API backend + React 19 frontend (Vite + TypeScript).

The frontend is built into `clientapp/dist` and served as static files by the .NET backend. In production they run as a single container.

## Commands

### Backend (.NET)

```bash
dotnet run                          # Run the API (serves frontend from clientapp/dist if built)
dotnet build                        # Build
dotnet ef migrations add <Name>     # Add EF migration
dotnet ef database update           # Apply migrations
```

### Frontend (clientapp/)

```bash
npm run dev      # Vite dev server (separate from .NET, hits API via proxy)
npm run build    # Build to dist/ (needed before dotnet run serves it)
npm run lint     # ESLint
```

### Full stack local dev

Run both `dotnet run` (port 5192) and `cd clientapp && npm run dev` simultaneously. Vite proxies `/api` to the .NET backend.

## Secrets setup (first time)

```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "<32+ char secret>"
dotnet user-secrets set "Jwt:Issuer" "http://localhost"
dotnet user-secrets set "Jwt:Audience" "http://localhost"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<mssql connection string>"
dotnet user-secrets set "smtp:server" "<host>"
dotnet user-secrets set "smtp:sslport" "<port>"
dotnet user-secrets set "smtp:username" "<user>"
dotnet user-secrets set "smtp:password" "<pass>"
dotnet user-secrets set "smtp:enablessl" "true"
```

Frontend base URL in emails: `Frontend:BaseUrl` in `appsettings.json` (default `http://localhost:8085`).

## Architecture

### Backend

- **Minimal API** — all routes registered via extension methods in `Endpoints/`. No controllers.
- **EF Core + SQL Server** — `Data/ApplicationDbContext.cs` is the single DbContext.
- **JWT auth** — roles: `Coach`, `Admin`, `Player`. Claims-based checks inline in endpoint handlers.
- **Email** — `Integrations/SmtpEmailSender.cs` is the sole email integration.
- **Logging** — Serilog; rolling daily file in `logs/`, console output.
- **JSON** — PascalCase property names throughout (no camelCase conversion).

Domain entities: `Team`, `User`, `Challenge`, `ChallengeCompletion`, `Goal` (split into `TeamGoal` / `UserGoal`). Many-to-many join tables: `TeamUser`, `TeamCoach`.

### Frontend

- **React 19 + React Router 7** — SPA served from .NET in production.
- **Auth** — `contexts/AuthContext.tsx` holds JWT token and user state.
- **API calls** — `services/auth.ts` and `services/team.ts` via axios.
- **Pages** — role-based dashboards: `AdminDashboard`, `CoachDashboard`, `PlayerDashboard`. Also `TeamPage`, `ExercisesPage`, auth flows.

### Deployment

CI/CD via GitHub Actions to personal server `antec` (10.0.20.10) as Docker container. Environment variables stored in `.env.missioncomplete` on the server. App will be at `missioncomplete.forstberg.net` via Cloudflare + Caddy reverse proxy. The `Dockerfile` builds the .NET app only — frontend must be pre-built before `docker build` or built in a multi-stage step.
