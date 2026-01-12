# Backend API Dokumentation (Kolla)

Diese Dokumentation beschreibt die REST-API und die SignalR-Schnittstelle für das "Kolla" Aufgabenmanagement-System.

**Base URL:** `http://http://localhost:5007/`

---

## 1. Datenmodelle & Enums

### Enums
Die API verwendet Integer-Werte für Status und Priorität.

**AssignmentStatus**
| Wert | Bedeutung |
| :--- | :--- |
| `0` | Planned |
| `1` | InProgress |
| `2` | Completed |

**Priority**
| Wert | Bedeutung |
| :--- | :--- |
| `0` | ShortTerm |
| `1` | MidTerm |
| `2` | LongTerm |

### TypeScript Interfaces (Referenz)

```typescript
interface Role {
  Guid: string;
  DisplayName: string;
  Description: string | null;
  IsAdmin: boolean;
}

interface Actor {
  Guid: string;
  DisplayName: string;
  Role: Role | null;
}

interface Objective {
  guid: string;
  displayName: string;
  description: string | null;
  deadlineDate: string; // ISO 8601
}

interface Assignment {
  guid: string;
  displayName: string;
  description: string | null;
  duration: number;
  sequenceNumber: number;
  assigneeGuid: string | null;
  requiredRoleGuid: string | null;
  priority: number; // 0..2 (Priority)
  status: number;   // 0..2 (AssignmentStatus)
  parentObjectiveGuid: string | null;
}
```

---

## 2. Endpoints

### Controller: Role (`/Role`)
Verwaltung von Benutzerrollen und Berechtigungen.

| Methode | URL | Beschreibung | Body / Return |
| :--- | :--- | :--- | :--- |
| **GET** | `/Role/GetAll` | Ruft alle Role-IDs ab. | Return: `string[]` (Liste von GUIDs) |
| **GET** | `/Role/Get/{guid}` | Ruft Details einer Rolle ab. | Return: `Role` |
| **POST** | `/Role/Create` | Erstellt neue Rolle. | Body: `{ "DisplayName": string, "Description": string?, "IsAdmin": boolean }` → Return: `string` (GUID) |
| **PATCH** | `/Role/SetDisplayName` | Ändert den Namen. | Body: `{ "Guid": string, "DisplayName": string }` |
| **PATCH** | `/Role/SetDescription` | Ändert die Beschreibung. | Body: `{ "Guid": string, "Description": string? }` |
| **PATCH** | `/Role/SetAdminFlag` | Setzt Admin-Rechte. | Body: `{ "Guid": string, "IsAdmin": boolean }` |
| **DELETE** | `/Role/Delete/{guid}` | Löscht eine Rolle. | Return: `void` |

---

### Controller: Actor (`/Actor`)
Verwaltung der Benutzer (Akteure).

| Methode | URL | Beschreibung | Body / Return |
| :--- | :--- | :--- | :--- |
| **GET** | `/Actor/GetAll` | Ruft alle Actor-IDs ab. | Return: `string[]` |
| **GET** | `/Actor/Get/{guid}` | Ruft Details eines Actors ab. | Return: `Actor` |
| **POST** | `/Actor/Create` | Erstellt neuen Actor. | Body: `{ "DisplayName": string, "RoleGuid": string? }` → Return: `string` (GUID) |
| **PATCH** | `/Actor/SetDisplayName` | Ändert den Namen. | Body: `{ "Guid": string, "DisplayName": string }` |
| **PATCH** | `/Actor/SetRole` | Weist eine Rolle zu (oder entfernt sie). | Body: `{ "Guid": string, "RoleGuid": string? }` |
| **GET** | `/Actor/GetAllAssignments/{guid}` | Alle Assignments eines Actors. | Return: `string[]` (Assignment GUIDs) |
| **DELETE** | `/Actor/Delete/{guid}` | Löscht einen Actor. | Return: `void` |

---

### Controller: Objective (`/Objective`)
Verwaltung der übergeordneten Aufgaben (Main Tasks).

| Methode | URL | Beschreibung | Body / Return |
| :--- | :--- | :--- | :--- |
| **GET** | `/Objective/GetAll` | Ruft alle Objective-IDs ab. | Return: `string[]` |
| **GET** | `/Objective/Get/{guid}` | Ruft Details eines Objectives ab. | Return: `Objective` |
| **POST** | `/Objective/Create` | Erstellt Objective. | Body: `{ "DisplayName": string, "Description": string?, "DeadlineDate": string }` → Return: `string` (GUID) |
| **PATCH** | `/Objective/SetDisplayName` | Ändert den Namen. | Body: `{ "Guid": string, "DisplayName": string }` |
| **PATCH** | `/Objective/SetDescription` | Ändert Beschreibung (oder entfernt sie). | Body: `{ "Guid": string, "Description": string? }` |
| **GET** | `/Objective/GetAllAssignments/{guid}` | Alle Assignments in einem Objective. | Return: `string[]` (Assignment GUIDs) |
| **DELETE** | `/Objective/Delete/{guid}` | Löscht Objective. | Return: `void` |

---

### Controller: Assignment (`/Assignment`)
Verwaltung der einzelnen Arbeitsschritte (Subtasks).

**Validierung:** Wenn `AssigneeGuid` und `RequiredRole` gesetzt sind, prüft das Backend:
1) ob der Assignee überhaupt eine Rolle hat  
2) ob Assignee.Role zur RequiredRole passt  
Wenn nicht, darf der Actor nicht zugewiesen werden.

| Methode | URL | Beschreibung | Body / Return |
| :--- | :--- | :--- | :--- |
| **GET** | `/Assignment/GetAll` | Ruft alle Assignment-IDs ab. | Return: `string[]` |
| **GET** | `/Assignment/Get/{guid}` | Ruft Details eines Assignments ab. | Return: `Assignment` |
| **POST** | `/Assignment/Create` | Erstellt Assignment. | Body: `{ "DisplayName": string, "Description": string?, "Duration": number, "AssigneeGuid": string?, "RequiredRole": string?, "ParentObjectiveGuid": string? }` → Return: `string` (GUID) |
| **PATCH** | `/Assignment/SetDisplayName` | Ändert Namen. | Body: `{ "Guid": string, "DisplayName": string }` |
| **PATCH** | `/Assignment/SetDescription` | Ändert Beschreibung (oder entfernt sie). | Body: `{ "Guid": string, "Description": string? }` |
| **PATCH** | `/Assignment/SetDuration` | Ändert Dauer. | Body: `{ "Guid": string, "Duration": number }` |
| **PATCH** | `/Assignment/SetAssignee` | Setzt/entfernt Assignee. | Body: `{ "Guid": string, "AssigneeGuid": string? }` |
| **PATCH** | `/Assignment/SetRequiredRole` | Setzt/entfernt RequiredRole. | Body: `{ "Guid": string, "RequiredRoleGuid": string? }` |
| **PATCH** | `/Assignment/SetPriority` | Setzt Priorität (Enum als int). | Body: `{ "Guid": string, "priority": 0 \| 1 \| 2 }` |
| **PATCH** | `/Assignment/SetStatus` | Setzt Status (Enum als int). | Body: `{ "Guid": string, "assignmentStatus": 0 \| 1 \| 2 }` |
| **PATCH** | `/Assignment/SetParentObjective` | Setzt/entfernt ParentObjective. | Body: `{ "Guid": string, "ParentObjectiveGuid": string? }` |
| **DELETE** | `/Assignment/DeleteAssignment/{guid}` | Löscht Assignment. | Return: `void` |

---

## 3. Real-Time (SignalR)

Das Backend sendet bei Updates eines Assignments eine Echtzeit-Benachrichtigung an alle verbundenen Clients.

- **Hub URL:** `/Assignment/Notify`
- **Client Event Name:** `OnAssignmentUpdated`
- **Payload:** `assignmentId` (string GUID)

**Beispiel (Frontend JS):**
```javascript
connection.on("OnAssignmentUpdated", (assignmentId) => {
  console.log("Assignment updated:", assignmentId);
  // Daten neu laden oder nur dieses Assignment refreshen
});
```
