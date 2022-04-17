# Heroku Dockerfile

FROM yiyungent/qqbothub:v0.5.0

# 处于 /app 目录下
ADD heroku-entrypoint.sh ./heroku-entrypoint.sh
RUN chmod +x ./heroku-entrypoint.sh
ADD heroku-PluginCore.Config.json ./heroku-PluginCore.Config.json

ENTRYPOINT ["/bin/sh", "./heroku-entrypoint.sh"]