# render Dockerfile

FROM yiyungent/knifehub:v1.1.1-amd-chrome

# 处于 /app 目录下
ADD render-entrypoint.sh ./render-entrypoint.sh
RUN chmod +x ./render-entrypoint.sh
ADD render-PluginCore.Config.json ./render-PluginCore.Config.json

ENTRYPOINT ["/bin/sh", "./render-entrypoint.sh"]
