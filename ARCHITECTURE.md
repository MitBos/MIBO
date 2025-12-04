# MIBO Architecture and Code Quality Overview

## High-level overview
MIBO is a Blazor Server application that lets users configure and run "integrations"—pipelines that fetch JSON from a source API, optionally map fields, and optionally push results to a target API. The UI is implemented with Razor components and talks to minimal API endpoints that persist integration definitions and execute runs via injected services (storage, API caller, mapping engine). Domain models now live in `src/MIBO.Domain/`, while sample configuration data sits under `src/MIBO.Web/Data/` and is copied to the web output.

## Solution layout
- `MIBO.sln`: solution composing the web UI, domain model, application abstractions, infrastructure, and tests.
- `src/MIBO.Domain`: domain DTOs such as `IntegrationConfig`, `FieldMapping`, `ApiCallRequest/Result`, and `RunIntegrationResponse`.
- `src/MIBO.Application`: service abstractions (`IApiCaller`, `IIntegrationConfigStorage`, `IMappingEngine`).
- `src/MIBO.Infrastructure`: concrete implementations of the abstractions (file-based storage, HTTP caller, mapping engine) and DI helpers.
- `src/MIBO.Web`: Razor components, routing, minimal API endpoints, UI-specific services, and seed data (`Data/Integrations/NewIntegration.json`).
- `tests/MIBO.Tests`: xUnit suite covering persistence, mapping, and editor state validation.

### Web project structure
- `Components/`: Blazor UI written in Razor.
  - `App.razor` and `Routes.razor` set up the root document and routing.
  - `Layout/`: shell elements such as `MainLayout`, `NavMenu`, and `ReconnectModal` plus their styling and JS helpers.
  - `Pages/`: top-level pages (`IntegrationsList`, `EditIntegration`, `Error`, `NotFound`).
  - `Integration/`: reusable panels for editing integration metadata, API settings, mappings, and showing run results.
- `Endpoints/`: minimal API registrations that expose CRUD-style endpoints and a "run" operation for integrations.

## Front-end architecture
- **Routing and layout**: `Components/App.razor` defines the HTML shell and loads Blazor scripts. `Components/Routes.razor` wires the router and a default `MainLayout` with header navigation and footer. `ReconnectModal` provides offline/connection recovery UX via a small JS module.
- **Pages**:
  - `IntegrationsList.razor` fetches integration names via `/api/integrations`, allows duplication via `/api/integrations/duplicate`, deletion via `DELETE /api/integrations/{name}`, and links to the editor.
  - `EditIntegration.razor` loads a specific integration, binds panel components to mutate the configuration, saves via `/api/integrations/save`, and runs via `/api/integrations/run` to show API results and mapped output.
- **Component composition**: `EditIntegration` aggregates `IntegrationConfigPanel`, `SourceApiPanel`, `TargetApiPanel`, `MappingPanel`, and `ApiResultPanel`. These panels bind directly to `IntegrationConfig` properties, mutate a shared `List<FieldMapping>`, and are intentionally simple (no state management library). Styling uses CSS co-located with layout components; other components rely on global `app.css` (not included in repo) and Bootstrap.

## Backend endpoints
- `IntegrationApiEndpoints.MapIntegrationApiEndpoints` registers `/api/integrations` routes:
  - `GET /api/integrations`: list all integration names via `IIntegrationConfigStorage`.
  - `GET /api/integrations/{name}`: load an integration or return a default template config when missing.
  - `POST /api/integrations/save`: validate name and persist via storage.
  - `POST /api/integrations/run`: save, execute source API via `IApiCaller`, map JSON via `IMappingEngine`, and return both raw and mapped results.
  - `DELETE /api/integrations/{name}`: remove a saved config.
  - `POST /api/integrations/duplicate`: clone one config into a new name.
- Service interfaces such as `IIntegrationConfigStorage`, `IApiCaller`, and `IMappingEngine` are referenced but not included in the repository; their implementations are assumed to reside elsewhere.

## Domain model
- `IntegrationConfig` captures integration metadata (type, API key requirements, rate limits), source/target API settings, and a list of `FieldMapping` entries.
- `FieldMapping` links a source JSON path to a target path.
- `ApiCallRequest`/`ApiCallResult` represent calling external APIs.
- `RunIntegrationResponse` wraps an `ApiCallResult` plus mapped JSON for the front end.
- `DuplicateIntegrationRequest` and `MappingConfig` serve endpoint payloads and mapping logic.

## Code quality observations
- **Strengths**:
  - Components include concise doc-comments explaining purpose and parameters, improving readability.
  - API endpoints validate required fields (e.g., integration name) before processing and use clear, small handlers.
  - Defensive null checks guard list manipulations and HTTP responses in components to avoid exceptions during async loading.
- **Gaps/risks**:
  - Service abstractions (`IIntegrationConfigStorage`, `IApiCaller`, `IMappingEngine`) are missing; repository consumers need to provide implementations for the app to run.
  - UI lacks centralized error handling/toast notifications—errors surface as inline messages only on the current page.
  - No unit/integration tests are present, and `app.css` or other global styles are not included, making visual fidelity unknown.
  - Navigation and layout assets include unused default Blazor navigation (e.g., `NavMenu` with counter/weather links) that may not match current routes.
- **Opportunities**:
  - Add form validation (required fields, URL validation) on the client side to prevent invalid payloads.
  - Introduce persistence/serialization tests for `IntegrationConfig` to ensure compatibility with stored JSON.
  - Replace inline string-based route building with shared route constants to reduce duplication between pages and endpoints.
  - Consider extracting mapping logic in the UI into a dedicated service/state container to simplify `EditIntegration` code-behind.
