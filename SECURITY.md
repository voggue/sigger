# Sicherheitshinweise zu Sigger

## Meldung von Schwachstellen

Bitte meldet Sicherheitsprobleme vertraulich über die Kontaktmöglichkeiten des GitHub-Repositorys (Private Security Advisory), nicht als öffentliches Issue, wenn ihr sensible Details teilen müsst.

## Bekannte Risiken und empfohlene Gegenmaßnahmen

### Schema-Endpunkt (`/sigger/sigger.json` o. ä.)

Das JSON-Schema beschreibt eure SignalR-Hubs (Methoden, DTOs, ggf. CLR-Typnamen). Ein öffentlich erreichbarer Endpunkt entspricht fachlich einer offenen API-Dokumentation.

- In Produktion: `WithSchemaEndpointMode(SiggerSchemaEndpointMode.DevelopmentOnly)` oder `Disabled`, oder zusätzlich Authentifizierung/Netzwerkrestriktionen.
- Optional: In Produktion `IncludeClrMetadataInSchema = false` setzen, um weniger Implementierungsdetails preiszugeben (kann Codegeneratoren beeinflussen).

### Sigger UI

Die eingebettete Test-UI ist für Entwicklung gedacht. Standardmäßig wird sie nur in der Umgebung **Development** registriert (`SiggerUiVisibility.DevelopmentOnly`). Für Tests außerhalb von Development bewusst `UseSiggerUi(o => o.WithVisibility(SiggerUiVisibility.Always))` setzen.

### CORS und SignalR

`AllowAnyOrigin()` zusammen mit authentifizierten Cookies oder `Credentials` ist nicht geeignet. Verwendet benannte Policies, konkrete Origins und bei Bedarf `AllowCredentials()`.

### CLI `sigger-gen`

- Standard: TLS-Zertifikate werden wie vom System vorgegeben geprüft.
- Nur bei Bedarf und niemals in CI/Produktion: `--insecure-tls` (deaktiviert die Zertifikatsprüfung und ist anfällig für Man-in-the-Middle-Angriffe).

## Abhängigkeiten

Überwacht transitive NuGet- und npm-Pakete (z. B. `dotnet list package --vulnerable`, `npm audit`) und haltet ASP.NET Core, EF Core und den Node-Client aktuell.
