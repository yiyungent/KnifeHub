# fly.io Dockerfile

# FROM yiyungent/knifehub:v1.4.3-amd-full
FROM yiyungent/knifehub:v1.6.3-amd-chrome

ADD flyio-entrypoint.sh ./flyio-entrypoint.sh
RUN chmod +x ./flyio-entrypoint.sh
ADD flyio-PluginCore.Config.json ./flyio-PluginCore.Config.json

ENTRYPOINT ["/bin/sh", "./flyio-entrypoint.sh"]
