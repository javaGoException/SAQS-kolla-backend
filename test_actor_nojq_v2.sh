#!/bin/zsh

# ==========================================
# KONFIGURATION
# ==========================================
BASE_URL="http://localhost:5007"
CONTENT_TYPE="Content-Type: application/json"

# Farben
GREEN=$'\e[0;32m'
RED=$'\e[0;31m'
CYAN=$'\e[0;36m'
YELLOW=$'\e[0;33m'
NC=$'\e[0m' # No Color

# Helper Funktion: JSON Parsen mit Python 3
json_extract() {
    python3 -c "import sys, json; data=json.load(sys.stdin); print(data$1 if data else 'null')" 2>/dev/null
}

# Helper Funktion für Array Länge
json_count() {
    python3 -c "import sys, json; print(len(json.load(sys.stdin)))" 2>/dev/null
}

print -P "${CYAN}🚀 Starte Actor-Tests (v2 - Robuster)...${NC}"

# Prüfen, ob Backend läuft
if ! curl -s "$BASE_URL/Objective/GetAll" > /dev/null; then
    print -P "${RED}Fehler: Backend ist nicht erreichbar.${NC}"
    print "Bitte starte das Backend mit 'dotnet run' in einem anderen Tab."
    exit 1
fi

# Gemeinsamer Zeitstempel für diesen Durchlauf
TIMESTAMP=$(date +%s)

# ==========================================
# 1. TEST-ROLLE ERSTELLEN
# ==========================================
print -P "\n${YELLOW}[1/6] Erstelle temporäre Rolle...${NC}"

# Dynamischer Name, damit es beim 2. Start nicht knallt
ROLE_NAME="Role_$TIMESTAMP"
ROLE_PAYLOAD="{\"displayName\": \"$ROLE_NAME\", \"description\": \"Auto-Test\", \"isAdmin\": false}"

ROLE_RESPONSE=$(curl -s -X POST "$BASE_URL/Role/Create" -H "$CONTENT_TYPE" -d "$ROLE_PAYLOAD")
ROLE_GUID=$(echo $ROLE_RESPONSE | json_extract "['guid']")

if [[ "$ROLE_GUID" != "null" && -n "$ROLE_GUID" ]]; then
    print -P "${GREEN}✅ Rolle erstellt: $ROLE_GUID ($ROLE_NAME)${NC}"
else
    # Fallback: Falls sie doch existiert (sehr unwahrscheinlich durch Timestamp), versuchen wir weiterzumachen
    print -P "${RED}⚠️ Warnung: Rolle konnte nicht erstellt werden. Response: $ROLE_RESPONSE${NC}"
    print -P "${YELLOW}Versuche, das Skript abzubrechen...${NC}"
    exit 1
fi

# ==========================================
# 2. ACTOR ERSTELLEN
# ==========================================
print -P "\n${YELLOW}[2/6] Erstelle Actor...${NC}"

ACTOR_NAME="User_$TIMESTAMP"
ACTOR_PAYLOAD="{\"nickname\": \"$ACTOR_NAME\", \"roleGuid\": null}"

ACTOR_RESPONSE=$(curl -s -X POST "$BASE_URL/Actor/Create" -H "$CONTENT_TYPE" -d "$ACTOR_PAYLOAD")
ACTOR_GUID=$(echo $ACTOR_RESPONSE | json_extract "['guid']")

if [[ "$ACTOR_GUID" != "null" && -n "$ACTOR_GUID" ]]; then
    print -P "${GREEN}✅ Actor erstellt: $ACTOR_GUID ($ACTOR_NAME)${NC}"
else
    print -P "${RED}❌ Fehler beim Erstellen des Actors. Response: $ACTOR_RESPONSE${NC}"
    exit 1
fi

# ==========================================
# 3. ACTOR ABRUFEN
# ==========================================
print -P "\n${YELLOW}[3/6] Prüfe Actor Daten (GET)...${NC}"
GET_RESPONSE=$(curl -s -X GET "$BASE_URL/Actor/Get/$ACTOR_GUID")
GET_NAME=$(echo $GET_RESPONSE | json_extract "['displayName']")

if [[ "$GET_NAME" == "$ACTOR_NAME" ]]; then
    print -P "${GREEN}✅ Name korrekt: $GET_NAME${NC}"
else
    print -P "${RED}❌ Name stimmt nicht! Erwartet: $ACTOR_NAME, Bekommen: $GET_NAME${NC}"
fi

# ==========================================
# 4. NICKNAME ÄNDERN
# ==========================================
print -P "\n${YELLOW}[4/6] Ändere Nickname...${NC}"
NEW_NAME="${ACTOR_NAME}_v2"
UPDATE_NAME_PAYLOAD="{\"guid\": \"$ACTOR_GUID\", \"nickname\": \"$NEW_NAME\"}"

curl -s -X PATCH "$BASE_URL/Actor/SetNickname" -H "$CONTENT_TYPE" -d "$UPDATE_NAME_PAYLOAD"

CHECK_RESPONSE=$(curl -s -X GET "$BASE_URL/Actor/Get/$ACTOR_GUID")
CHECK_NAME=$(echo $CHECK_RESPONSE | json_extract "['displayName']")

if [[ "$CHECK_NAME" == "$NEW_NAME" ]]; then
    print -P "${GREEN}✅ Nickname geändert auf: $CHECK_NAME${NC}"
else
    print -P "${RED}❌ Änderung fehlgeschlagen. Aktuell: $CHECK_NAME${NC}"
fi

# ==========================================
# 5. ROLLE ZUWEISEN
# ==========================================
print -P "\n${YELLOW}[5/6] Weise Rolle zu ($ROLE_GUID)...${NC}"
SET_ROLE_PAYLOAD="{\"guid\": \"$ACTOR_GUID\", \"roleGuid\": \"$ROLE_GUID\"}"

curl -s -X PATCH "$BASE_URL/Actor/SetRole" -H "$CONTENT_TYPE" -d "$SET_ROLE_PAYLOAD"

CHECK_ROLE_RESPONSE=$(curl -s -X GET "$BASE_URL/Actor/Get/$ACTOR_GUID")
CHECK_ROLE_ID=$(echo $CHECK_ROLE_RESPONSE | json_extract "['role']['guid']")

if [[ "$CHECK_ROLE_ID" == "$ROLE_GUID" ]]; then
    print -P "${GREEN}✅ Rolle erfolgreich verknüpft.${NC}"
else
    print -P "${RED}❌ Rollenverknüpfung fehlgeschlagen. DB Wert: $CHECK_ROLE_ID${NC}"
fi

# ==========================================
# 6. GET ALL ACTORS
# ==========================================
print -P "\n${YELLOW}[6/6] Liste alle Actors auf...${NC}"
ALL_RESPONSE=$(curl -s -X GET "$BASE_URL/Actor/GetAll")
COUNT=$(echo $ALL_RESPONSE | json_count)

if [[ "$COUNT" -gt 0 ]]; then
    print -P "${GREEN}✅ GetAll erfolgreich. Anzahl Actors: $COUNT${NC}"
else
    print -P "${RED}❌ Keine Actors gefunden oder Fehler beim Parsen.${NC}"
fi

print -P "\n${CYAN}🎉 Testdurchlauf abgeschlossen!${NC}"
