# Policy and AD Group-Based Authorization

## Primary business objective
Allow OURLOTTO users to access LOTGITS-hosted IIS Windows Authentication applications without requiring OURLOTTO client machines to contact LOTGITS domain controllers.

## Authorization model

Policy evaluation order:
1. Explicit user override (optional)
2. AD group policy
3. Application policy
4. Global default policy

Security conflict rule:
- Deny wins over allow, unless an explicit configurable exception is set by implementation.

Application visibility rule:
- Each app declares `RequiredGroups` in `config/apps.json`.
- A user only sees apps where they are a member of at least one required group.
- Direct URL access must enforce the same authorization path and deny access when no allowed group is present.

## AD group design

Example groups:
- `APP_Users`
- `APP_Admins`
- `APP_FileUpload_Allowed`
- `APP_FileDownload_Allowed`
- `APP_Print_Allowed`
- `APP_ClipboardIn_Allowed`
- `APP_ClipboardOut_Allowed`
- `APP_DevTools_Allowed`
- `APP_SessionRecording_Allowed`
- `APP_PersistentProfile_Allowed`

Admin group principle:
- `APP_Admins` can enable selected admin capabilities (e.g., dev tools, persistent profile, broader catalog scope).
- `APP_Admins` must not implicitly grant download, clipboard-out, or printing unless separate groups/policies explicitly allow them.

## Recommended secure defaults

The baseline in `config/policies.json` enforces:
- Upload/download/printing disabled.
- Clipboard into session enabled, clipboard out disabled.
- Browser extensions and dev tools disabled.
- Webcam, microphone, and audio disabled by default.
- Ephemeral session profile by default.
- Session metadata recording enabled; full video recording disabled.
- Watermarking enabled by default.
- Kiosk mode enabled by default.
- HTTPS required in production; startup should warn/refuse if configured for HTTP.

## Identity mapping model

`config/identity-mapping.json` defines OURLOTTO-to-LOTGITS identity translation:
- Default mode: same username.
- Source domain: `OURLOTTO`.
- Target Kerberos realm: `AD.LOTGITS.COM`.
- Optional explicit per-user overrides for non-standard mappings.

## Kerberos flow inside the container

1. User authenticates to the portal as OURLOTTO identity.
2. Broker resolves effective app + policy + group authorization.
3. Broker maps OURLOTTO identity to LOTGITS principal.
4. Browser container runs `kinit` for mapped LOTGITS identity.
5. Browser accesses LOTGITS IIS endpoint using Negotiate/Kerberos.

Why client machines do not require LOTGITS DC reachability:
- Kerberos exchange for LOTGITS occurs from the broker/container side, not from the OURLOTTO endpoint device.

Credential handling requirements:
- Do not store LOTGITS passwords by default.
- If persistence is enabled, make it opt-in and use protected host storage (DPAPI or Windows Credential Manager).
- Never log passwords or Kerberos tickets.

## Enforceable vs best-effort controls

Expected implementation posture:
- Enforce controls at both streaming and browser/container layers when possible.
- If a requested control is unsupported by noVNC/KasmVNC or browser runtime, log startup warning and display on admin/status page.
- Never claim a control is enforced when it is only best-effort.

## Watermarking

When `EnableWatermark=true`, overlay must be visible and include:
- OURLOTTO username
- LOTGITS mapped username (if known)
- App name
- Session ID
- Timestamp

Note: screenshots cannot be fully prevented; watermarking is default mitigation.

## Logging and observability

Required logs:
- Effective policy at session creation (redacted).
- Group memberships used in decisions.
- App access denied events.
- Kerberos diagnostics: DNS resolution, KDC reachability, krb5.conf generation, kinit result, SPN/HTTP ticket request, browser negotiate configuration.

Event Viewer IDs:
- 1600 policy evaluated
- 1601 app access denied
- 1602 group lookup failed
- 1603 unsupported control requested
- 1604 watermark enabled
- 1605 file transfer blocked
- 1606 clipboard transfer blocked
- 1607 printing blocked
- 1608 dev tools enabled
- 1609 dev tools blocked

## Admin/status page requirements

Display:
- Loaded applications
- Loaded policy set
- Security control enforceability matrix
- Active sessions
- Recent warnings

Do not display:
- Secrets, raw credentials, tickets, or sensitive token material.

## Test coverage expectations

Add tests for:
- Group policy merge
- Deny-over-allow behavior
- App visibility by group
- Browser argument generation
- Kerberos config generation
- Logging redaction
- Unsupported control warning behavior

## ELK monitoring guidance

For Windows Event Viewer + ELK ingestion:
- Collect event IDs 1600–1609.
- Parse app id, session id, source user, mapped user, decision outcome, and control name.
- Alert on repeated 1602 or 1603 events (identity/group lookup issues or unsupported security requests).
- Dashboard policy denials (1601) and blocked transfers (1605/1606/1607) by app and group.
