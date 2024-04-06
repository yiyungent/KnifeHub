# Zeabur Dockerfile

# FROM yiyungent/knifehub:v1.4.3-amd-full
FROM yiyungent/knifehub:v1.6.3-amd-chrome

# 处于 /app 目录下
ADD zeabur-entrypoint.sh ./zeabur-entrypoint.sh
RUN chmod +x ./zeabur-entrypoint.sh
ADD zeabur-PluginCore.Config.json ./zeabur-PluginCore.Config.json

ENTRYPOINT ["/bin/sh", "./zeabur-entrypoint.sh"]
