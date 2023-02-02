


# $pluginId = "QQHelloWorldPlugin"

# dotnet build QQHelloWorldPlugin 并 将 相关文件移动到 ./src/KnifeHub.Web/Plugins/QQHelloWorldPlugin 文件夹中

cd ..

cd "./plugins/${pluginId}"

dotnet build --configuration Debug
ls ./bin/Debug/
ls ./bin/Debug/net6.0

cd ./bin/Debug/net6.0

# 移除框架相关, 防止重复加载 导致 类型对不齐
# rm PluginCore.IPlugins*

# 返回仓库根目录
cd ../../../../../

# 移除原处 所有文件
rm -r "./src/KnifeHub.Web/Plugins/${pluginId}/*"

# 移动
mv "./plugins/${pluginId}/bin/Debug/net6.0/*" "./src/KnifeHub.Web/Plugins/${pluginId}/"

ls "./src/KnifeHub.Web/Plugins/${pluginId}/"

echo "${pluginId} build and move success !"

cd "./utils"
