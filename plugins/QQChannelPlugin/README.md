


## 介绍

> 本项目为 QQ频道 基础插件, 可通过依赖本插件开发相关插件


## 设置

> 可配置多个机器人, 当然不需要那么多, 可删除, 只留一个

```json
"UseSandBoxMode": true,
"UsePrivateBot": true,
"EnableUserMessageTriggerCommand": true,
"UseDemoModel": true
```

> `UseSandBoxMode`: true    
> 指定Api通道模式为沙盒模式 (测试时使用), 不指定的情况下默认是正式模式  

> `UsePrivateBot`: true   
> 指定为私域机器人  
> 在想要使用一些私域机器人功能时，需要通过以下方法指定机器人为私域，否则无法正常使用。  
> 例如： 无需 @机器人 可收到频道内用户消息

> `EnableUserMessageTriggerCommand`: true    
> 启用无须@ 触发指令功能 (私域机器人可用)  
> 启用后，频道内触发机器人指令 无需 @机器人

> `UseDemoModel`: true    
> 当开启 `UseDemoModel` 时, 可在机器人加入的频道内 `@机器人` , 它会自动回复, 此项功能用于测试



## 登录

- [点击访问即可尝试登录, 注意查看控制台信息](/Plugins/QQChannelPlugin/Login)



## 下载数据库

> 所有数据保存到数据库 **QQChannelPlugin.sqlite** 中

- [下载数据库 (QQChannelPlugin.sqlite)](/Plugins/QQChannelPlugin/Download)




## 相关

> 项目地址: [https://github.com/yiyungent/QQBotHub/tree/main/plugins/QQChannelPlugin](https://github.com/yiyungent/QQBotHub/tree/main/plugins/QQChannelPlugin)             
> 本项目基于 [Chianne1025/QQChannelFramework: MyBot - QQ频道机器人开发框架(C#)](https://github.com/Chianne1025/QQChannelFramework)

<!-- Matomo Image Tracker-->
<img referrerpolicy="no-referrer-when-downgrade" src="https://matomo.moeci.com/matomo.php?idsite=2&amp;rec=1&amp;action_name=Plugins.QQChannelPlugin-v0.1.6.README" style="border:0" alt="" />
<!-- End Matomo -->
