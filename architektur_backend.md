# Architektur – SAQS Kolla Backend

Stand: 2026-01-07

## 1. Zweck & fachlicher Zuschnitt
Dieses Backend stellt eine HTTP-API (Minimal API) für die Domäne **Kolla** bereit. Aus dem Code lassen sich folgende Kernobjekte ableiten:

- **Objective** (Ziel)
- **Assignment** (Aufgabe, optional verknüpft mit Objective, Actor, Role)
- **Actor** (Benutzer/Person, optional mit Role)
- **Role** (Rolle inkl. Admin-Flag)

Zusätzlich gibt es **Echtzeit-Benachrichtigungen** zu Assignment-Änderungen via **SignalR**.

## 2. Übergeordnete Architekturentscheidungen

### 2.1 Architektur-Stil: Schichtenmodell (Clean-ish / Onion-ish)
Der Code ist in **API**, **Application**, **Domain**, **Infrastructure** gegliedert:

- **API**: Transport-/Adapter-Schicht (HTTP Endpoints, DTOs, Error-Mapping, SignalR Hub)
- **Application**: Use-Case-/Service-Schicht (Business-Regeln, Validierung, Orchestrierung)
- **Domain**: Domänenmodelle (ValueObjects/Entities, Enums)
- **Infrastructure**: Persistenz & technische Adapter (Dapper-Repositories, SQLite Setup)

Wichtig: Die API gibt aktuell **Domain-Objekte direkt** als Response zurück (z.B. `Actor`, `Assignment`). Das ist eine bewusste Einfachheitsentscheidung, koppelt aber Transportmodell und Domänenmodell.

### 2.2 Technologie-/Framework-Entscheidungen
Aus `SAQS-kolla-backend.csproj` und `Program.cs`:

- **.NET**: `net10.0`
- **ASP.NET Core Minimal API** (keine MVC-Controller; statische Endpoint-Mapping-Klassen)
- **Dependency Injection** (built-in DI Container)
- **Dapper** als Micro-ORM (SQL-first, leichtgewichtig)
- **SQLite** (`Microsoft.Data.Sqlite`) als Datenbank
- **SignalR** für Echtzeit-Events
- **Options Pattern** (`DatabaseOptions`) für Konfiguration

### 2.3 Persistenzstrategie
- SQLite, Tabellen werden beim Start **imperativ erstellt** (kein Migrationsframework, keine Versionierung der DB-Schemata).
- IDs sind `Guid` und werden in SQLite als `TEXT` gespeichert.
- Zeiten (`StartDate`, `EndDate`, `DeadlineDate`) werden als `TEXT` gespeichert, im ISO-ähnlichen Format `yyyy-MM-ddTHH:mm:ssZ`.

### 2.4 Fehler-/Ergebnisstrategie
- Durchgehend wird ein **Result-Pattern** verwendet (`Application/Common/Result.cs`):
  - `Result` / `Result<T>` mit `IsSuccess`, `ResultError`, `Error`.
- In der API wird über `API/ErrorMapper.cs` auf HTTP-Status gemappt:
  - `NotFound → 404`, `ValidationError → 400`, `Conflict → 409`, `Unauthorized → 401`, Default → `500`.

### 2.5 Echtzeitstrategie
- Jede relevante Änderung an Assignments triggert ein Broadcast-Event:
  - `OnAssignmentUpdated` an `Clients.All` über SignalR (`AssignmentEndpoints`)
  - Hub: `API/Hubs/AssignmentHub.cs` (leerer Hub, nur Kanal)

## 3. Bausteinsicht (Komponenten)

### 3.1 Hosting / Einstiegspunkt
Datei: `Program.cs`

- Registrierung der Services (Application) und Repositories (Infrastructure) via DI.
- Konfiguration der DB über `DatabaseOptions` (`appsettings.json`: `DatabaseOptions:SqliteConnectionString`).
- Datenbank-Initialisierung beim Start (scoped create + `SqliteInitializer.InitializeDatabase()`).
- Mapping der Endpoints:
  - `ObjectiveEndpoints.Map(app)`
  - `RoleEndpoints.Map(app)`
  - `AssignmentEndpoints.Map(app)`
  - `ActorEndpoints.Map(app)`
- Mapping des SignalR Hubs: `app.MapHub<AssignmentHub>("/Assignment/Notify")`.

### 3.2 API-Schicht
Dateien: `API/*Endpoints.cs`, `API/DTOs/**`, `API/ErrorMapper.cs`, `API/Hubs/AssignmentHub.cs`

- Endpoints sind als **statische Klassen** mit `Map(WebApplication app)` implementiert.
- Routing ist **nicht REST-konventionell**, sondern nutzt Muster wie:
  - `Entity/GetAll`, `Entity/Get/{guid}`, `Entity/Create`, `Entity/SetX`, `Entity/Delete/{guid}`.
- Request DTOs sind als `record` mit **DataAnnotations** (`[Required]`) modelliert.
  - Hinweis: In `Program.cs` wird `builder.Services.AddValidation();` aufgerufen, aber eine Implementierung dieser Extension ist im aktuellen Repo-Snapshot nicht auffindbar. Dokumentation/Build sollten klären, ob dies aus einem (hier nicht referenzierten) Package kommt oder fehlt.

### 3.3 Application-Schicht (Use-Cases)
Dateien: `Application/Services/*.cs`, `Application/Interfaces/*.cs`, `Application/Common/*`

- Pro Aggregat existiert ein Service + Interface:
  - `IActorService`/`ActorService`
  - `IRoleService`/`RoleService`
  - `IObjectiveService`/`ObjectiveService`
  - `IAssignmentService`/`AssignmentService`
- Services enthalten fachliche Regeln, z.B. `AssignmentService`:
  - Name Pflicht, Duplikatprüfung
  - Start-/Deadline-Datum nicht in der Vergangenheit, Deadline nach Start
  - Priorität aus Zeitspanne (Short/Mid/Long Term)
  - Konsistenzregeln zwischen `AssigneeGuid` und `RequiredRoleGuid`
  - Status `Completed` setzt `EndDate` auf `Now`

### 3.4 Domain-Schicht
Dateien: `Domain/ValueObjects/*.cs`, `Domain/Enums/*.cs`

- Domain-Modelle sind **mutable Klassen** mit `required` Properties:
  - `Actor` enthält optional `Role` als Objekt
  - `Assignment` enthält GUID-Referenzen auf Actor/Role/Objective
  - `Objective`, `Role`
- Enums:
  - `Priority` (Short/Mid/Long)
  - `AssignmentStatus` (Planned/InProgress/Completed)

### 3.5 Infrastructure-Schicht
Dateien: `Infrastructure/Services/*.cs`, `Infrastructure/DTOs/*.cs`, `Infrastructure/Setup/*.cs`

- Datenzugriff via **Repository-Pattern** + Dapper.
- `IDatabaseConnector` kapselt Connection-Erzeugung (`SqliteConnector` nutzt `DatabaseOptions`).
- `SqliteInitializer` erstellt Tabellen:
  - `Objectives(Guid, DisplayName, Description)`
  - `Roles(Guid, DisplayName, Description, IsAdmin)`
  - `Actors(Guid, DisplayName, RoleGuid)`
  - `Assignments(Guid, DisplayName, Description, StartDate, EndDate, DeadlineDate, AssigneeGuid, RequiredRoleGuid, Priority, Status, ParentObjectiveGuid)`
- DTOs sind flache Persistenzmodelle (`*Dto`), die dann in Domain-Objekte gemappt werden.

## 4. Datenflüsse (Sequenzen)

### 4.1 Standard-Read (Beispiel: `GET Actor/Get/{guid}`)
1. HTTP Request → `ActorEndpoints`
2. Aufruf `IActorService.Get(guid)`
3. Service ruft `IActorRepository.QueryActor(guid)`
4. Repository öffnet DB-Connection (`IDatabaseConnector.OpenConnectionAsync()`)
5. Dapper Query → `ActorDto`
6. Optional: Role nachladen über `IRoleRepository.QueryRole(roleGuid)`
7. Domain `Actor` wird erstellt und zurückgegeben
8. API gibt `200 OK` mit Domain-Objekt zurück (oder `ErrorMapper` bei Fehlern)

### 4.2 Create (Beispiel: `POST Assignment/Create`)
1. HTTP Request → `AssignmentEndpoints`
2. DTO → Parameterweitergabe an `IAssignmentService.Create(...)`
3. Service führt Validierungen/Konsistenzprüfungen durch
4. Service schreibt via `IAssignmentRepository.InsertAssignment(assignment)`
5. API sendet SignalR Broadcast: `OnAssignmentUpdated` mit Assignment-Guid
6. API Response: `200 OK { guid = ... }`

### 4.3 Update mit Broadcast (Beispiel: `PATCH Assignment/SetStatus`)
1. HTTP Request → `AssignmentEndpoints`
2. (Teilweise) Vorvalidierung in API: `Enum.IsDefined(...)` für Status/Priority
3. Service prüft Existenz, setzt ggf. `EndDate`
4. Repository Update
5. SignalR `OnAssignmentUpdated(guid)` an alle Clients
6. Response: `204 NoContent`

### 4.4 Delete mit „Detach“ von Referenzen
In mehreren Repositories werden vor dem Delete Referenzen in Assignments auf `NULL` gesetzt:

- `ActorRepository.Delete`: `Assignments.AssigneeGuid = NULL` für betroffene Assignments
- `RoleRepository.DeleteRole`: `Assignments.RequiredRoleGuid = NULL`
- `ObjectiveRepository.DeleteObjective`: `Assignments.ParentObjectiveGuid = NULL`

Diese Logik ersetzt Foreign-Key-Cascades (es sind keine FK-Constraints im Schema definiert).

## 5. Abhängigkeiten (Dependency View)

### 5.1 Projektinterne Abhängigkeiten
- **API** → `Application.Interfaces`, `Application.Common`, `Domain.*`
- **Application.Services** → `Application.Interfaces`, `Application.Common`, `Domain.*`
- **Infrastructure** → `Application.Interfaces`, `Domain.*`, `Options` + externe DB/ORM Libraries
- **Domain** → keine internen Abhängigkeiten

### 5.2 Externe Libraries
- `Dapper` (SQL Mapping)
- `Microsoft.Data.Sqlite` (SQLite ADO.NET Provider)
- `Microsoft.AspNetCore.SignalR` (Realtime Messaging)

## 6. Konfiguration & Betrieb

### 6.1 Konfiguration
- `appsettings.json`:
  - `DatabaseOptions:SqliteConnectionString` (Default: `Data Source=KollaDB;`)
- `DatabaseOptions` wird bei Start validiert (`ValidateOnStart`).

### 6.2 Docker
- `Dockerfile`:
  - Build: `dotnet restore` + `dotnet publish -o out`
  - Runtime: `mcr.microsoft.com/dotnet/aspnet:10.0`
- `docker-compose.yml`:
  - Port-Mapping `5007:5007`
  - `ASPNETCORE_URLS=http://+:5007`

## 7. Architekturrelevante Patterns & Design-Details

- **Minimal API** statt Controller/MVC
- **Dependency Injection** für Entkopplung und Testbarkeit
- **Repository Pattern** (Infrastructure) + **Service/Use-Case Layer** (Application)
- **DTO Pattern** in zwei Formen:
  - API Request DTOs (`API/DTOs/**`)
  - Persistenz DTOs (`Infrastructure/DTOs/**`)
- **Result Pattern** für Fehlerpropagierung (statt Exceptions als Flow-Control)
- **Eventing (SignalR)** für push-basierte Konsistenz bei Assignment-Änderungen

## 8. Wichtige Trade-offs / bekannte Lücken (aus Code ersichtlich)

- **Validation-Pipeline unklar**: `AddValidation()` wird aufgerufen, ist aber im Repo nicht auffindbar. Ohne zusätzliche Infrastruktur werden DataAnnotations in Minimal APIs nicht automatisch für alle Endpoints erzwungen.
- **Keine AuthN/AuthZ** in `Program.cs` (keine `UseAuthentication/UseAuthorization`, kein Policy-Konzept). `ResultError.Unauthorized` existiert, wird aber aktuell nicht sichtbar genutzt.
- **Schema ohne Constraints/Indizes**: keine Foreign Keys, keine Unique Constraints (Duplikatschutz erfolgt rein in Application via Query).
- **Zeitformatierung/Parsing**: Assignments parsen Dates strikt via `ParseExact("yyyy-MM-ddTHH:mm:ssZ")` → Formatänderungen in DB brechen Parsing.
- **Delete-Return-Values**: Einige Delete-Methoden geben `false` zurück, wenn beim „Detach“ 0 Zeilen betroffen sind (z.B. Actor ohne Assignments) – auch wenn der eigentliche Delete erfolgreich war. Das ist für API-Fehlerbilder relevant.
- **Testskript veraltet**: `test_actor_nojq_v2.sh` verwendet Endpoints/Feldnamen wie `SetNickname`/`nickname`, die im Code `SetDisplayName`/`displayName` heißen.

## 9. Erweiterungspunkte (wie man Features ergänzt)

- Neues Aggregat hinzufügen:
  - Domain: `Domain/ValueObjects/<X>.cs` (+ Enums)
  - Application: `Application/Interfaces/I<X>Service.cs`, `Application/Services/<X>Service.cs`
  - Infrastructure: `Application/Interfaces/I<X>Repository.cs`, `Infrastructure/Services/<X>Repository.cs`, `Infrastructure/DTOs/<X>Dto.cs`
  - API: `API/<X>Endpoints.cs`, `API/DTOs/<X>/*.cs`
  - Startup: DI in `Program.cs`, Table in `SqliteInitializer`

## 10. Kurzfazit
Das Backend ist bewusst **leichtgewichtig** gehalten: Minimal API + Dapper + SQLite, klare Schichten, Result-basiertes Fehlerhandling und SignalR für Realtime-Updates bei Assignments. Der Fokus liegt auf schneller Umsetzbarkeit; Themen wie Auth, Migrations, konventionelles REST, Swagger/OpenAPI und belastbare Validierung sind (Stand Code) nicht oder nur angedeutet vorhanden.
