# render Dockerfile

FROM yiyungent/qqbothub:v0.5.3

# 处于 /app 目录下
ADD render-entrypoint.sh ./render-entrypoint.sh
RUN chmod +x ./render-entrypoint.sh
ADD render-PluginCore.Config.json ./render-PluginCore.Config.json

ENTRYPOINT ["/bin/sh", "./render-entrypoint.sh"]