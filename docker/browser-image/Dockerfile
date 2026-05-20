FROM ubuntu:24.04
RUN apt-get update && DEBIAN_FRONTEND=noninteractive apt-get install -y chromium-browser krb5-user xvfb x11vnc novnc websockify fluxbox curl && rm -rf /var/lib/apt/lists/*
COPY start-browser.sh /usr/local/bin/start-browser.sh
RUN chmod +x /usr/local/bin/start-browser.sh
EXPOSE 6901
HEALTHCHECK CMD curl -f http://localhost:6901 || exit 1
ENTRYPOINT ["/usr/local/bin/start-browser.sh"]
