


## 介绍

> 本项目为 QQ 基础插件, 可通过依赖本插件开发相关插件

```json
"UseDemoModel": true
```

> 当开启 `UseDemoModel` 时, 可在机器人加入的群内 发送 `hello sora` , 它会自动回复, 此项功能用于测试

## 注意

本插件只支持 Universal 连接方式的 `array` 上报格式

请将 onebot 端的数据上报格式修改为 `array` 格式

> 下方配置使用 反向 Websocket 连接, 插件 `设置` 中 `Mode` 为 `server`, `ServerConfig.Port` 为 `8080` , `ServerConfig.Host` 为 `127.0.0.1`

> config.yml

```yml
message:
  # 上报数据类型
  # 可选: string,array
  post-format: array

# 连接服务列表
servers:
  # 反向WS设置
  - ws-reverse:
      # 反向WS Universal 地址
      # 注意 设置了此项地址后下面两项将会被忽略
      universal: ws://127.0.0.1:8080
```

> 注意: 更改 config.yml 后, 需要重新启动 go-cqhttp 才能生效,   
> 插件设置更改需要 再次访问启动 才能生效 

## 启动

> 请耐心等待启动

- [点击访问即可尝试启动, 注意查看控制台信息](/Plugins/SoraPlugin/Start)



## 下载数据库

> 所有数据保存到数据库 **SoraPlugin.sqlite** 中

- [下载数据库 (SoraPlugin.sqlite)](/Plugins/SoraPlugin/Download)




## 相关

> 项目地址: [https://github.com/yiyungent/QQBotHub/tree/main/plugins/SoraPlugin](https://github.com/yiyungent/QQBotHub/tree/main/plugins/SoraPlugin)             
> 本项目基于 [Hoshikawa-Kaguya/Sora: .Net 6异步机器人框架，跨平台，OneBot协议（原CQHTTP协议），在兼容协议的同时主要为Go-Cqhttp提供支持](https://github.com/Hoshikawa-Kaguya/Sora)

<!-- Matomo Image Tracker-->
<img referrerpolicy="no-referrer-when-downgrade" src="https://matomo.moeci.com/matomo.php?idsite=2&amp;rec=1&amp;action_name=Plugins.SoraPlugin-v0.1.7.README" style="border:0" alt="" />
<!-- End Matomo -->

