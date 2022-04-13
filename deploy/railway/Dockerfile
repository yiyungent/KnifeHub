# Railway Dockerfile

FROM yiyungent/qqbothub:v0.0.2

# 处于 /app 目录下
ADD railway-entrypoint.sh ./railway-entrypoint.sh
RUN chmod +x ./railway-entrypoint.sh
ADD railway-PluginCore.Config.json ./railway-PluginCore.Config.json

# TODO: 不知道为什么 settings.json 没有被发布出去
RUN ls /app
#RUN rm ./settings.json
ADD settings.json ./settings.json
RUN pwd
RUN cat ./settings.json

ENTRYPOINT ["/bin/sh", "./railway-entrypoint.sh"]