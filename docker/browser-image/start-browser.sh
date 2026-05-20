#!/usr/bin/env bash
set -euo pipefail
cat > /etc/krb5.conf <<KRB
[libdefaults]
 default_realm = TARGET.LOCAL
 dns_lookup_kdc = true
 dns_lookup_realm = true
KRB
export DISPLAY=:1
Xvfb :1 -screen 0 1920x1080x24 &
fluxbox &
if [[ -n "${KINIT_UPN:-}" ]] && [[ -n "${KINIT_PASSWORD:-}" ]]; then
  echo "$KINIT_PASSWORD" | kinit "$KINIT_UPN" || true
fi
chromium-browser --no-first-run --disable-sync --disable-dev-shm-usage --auth-server-whitelist=*.target.local --auth-negotiate-delegate-whitelist=*.target.local --user-data-dir="/profiles/${SESSION_ID:-default}" "${TARGET_URL:-https://app1.target.local}" &
x11vnc -display :1 -nopw -forever -shared -rfbport 5901 &
websockify --web=/usr/share/novnc/ 6901 localhost:5901
