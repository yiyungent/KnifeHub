# fly.io Dockerfile

FROM yiyungent/dragonfly:v0.1.3

ADD flyio-entrypoint.sh ./flyio-entrypoint.sh
RUN chmod +x ./flyio-entrypoint.sh
ADD flyio-PluginCore.Config.json ./flyio-PluginCore.Config.json

ENTRYPOINT ["/bin/sh", "./flyio-entrypoint.sh"]