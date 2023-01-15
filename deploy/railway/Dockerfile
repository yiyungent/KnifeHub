# Railway Dockerfile

FROM yiyungent/knifehub:v1.0.5

# 处于 /app 目录下
ADD railway-entrypoint.sh ./railway-entrypoint.sh
RUN chmod +x ./railway-entrypoint.sh
ADD railway-PluginCore.Config.json ./railway-PluginCore.Config.json

ENTRYPOINT ["/bin/sh", "./railway-entrypoint.sh"]