#!/bin/sh

# region env
export ASPNETCORE_URLS="http://+:$PORT"
export ASPNETCORE_ENVIRONMENT="Production"
export TZ="Asia/Shanghai"
# endregion env

# region PluginCore
echo ${PLUGINCORE_ADMIN_USERNAME}
echo ${PLUGINCORE_ADMIN_PASSWORD}

mkdir App_Data

touch /app/App_Data/PluginCore.Config.json

cat '/app/heroku-PluginCore.Config.json' | sed "s/PLUGINCORE_ADMIN_USERNAME/${PLUGINCORE_ADMIN_USERNAME}/g" | tee '/app/App_Data/PluginCore.Config.json'
cat '/app/App_Data/PluginCore.Config.json' | sed "s/PLUGINCORE_ADMIN_PASSWORD/${PLUGINCORE_ADMIN_PASSWORD}/g" | tee '/app/App_Data/PluginCore.Config.json'
# endregion PluginCore

# cat '/etc/nginx/sites-enabled/default' | sed "s/80/${PORT}/g" | tee '/etc/nginx/sites-enabled/default'
# /usr/sbin/nginx -s reload

dotnet KnifeHub.Web.dll
# /usr/bin/supervisord -c /etc/supervisord.conf