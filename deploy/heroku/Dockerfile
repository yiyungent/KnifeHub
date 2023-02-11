# Heroku Dockerfile

FROM yiyungent/knifehub:v1.2.0-amd-chrome

# 处于 /app 目录下
ADD heroku-entrypoint.sh ./heroku-entrypoint.sh
RUN chmod +x ./heroku-entrypoint.sh
ADD heroku-PluginCore.Config.json ./heroku-PluginCore.Config.json

ENTRYPOINT ["/bin/sh", "./heroku-entrypoint.sh"]
