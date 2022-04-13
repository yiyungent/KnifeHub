# Heroku Dockerfile

FROM yiyungent/qqbothub:v0.0.2

# 处于 /app 目录下
ADD heroku-entrypoint.sh ./heroku-entrypoint.sh
RUN chmod +x ./heroku-entrypoint.sh
ADD heroku-PluginCore.Config.json ./heroku-PluginCore.Config.json

# TODO: 不知道为什么 settings.json 没有被发布出去
RUN ls /app
#RUN rm ./settings.json
ADD settings.json ./settings.json
RUN pwd
RUN cat ./settings.json

ENTRYPOINT ["/bin/sh", "./heroku-entrypoint.sh"]