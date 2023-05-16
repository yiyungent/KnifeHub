


## 介绍

> 本项目为 QQ 基础插件, 可通过依赖本插件开发相关插件,  
> 注意: 依赖本插件开发相关插件, 插件将感染 `GPL-3.0 license`

```json
"UseDemoModel": true
```

> 当开启 `UseDemoModel` 时, 可在机器人加入的群内 `@机器人` , 它会自动回复, 此项功能用于测试



## 登录

- [前往登录](/Plugins/KonataPlugin)


### KonataPlugin: QQ 登录

> 部署完成后
> > ⭐⭐⭐⭐ 一定要先登录 `PluginCore Admin`, 因为 `QQ登录页面` 与 `PluginCore Admin` 使用相同权限⭐⭐⭐⭐      
> > 访问 **401** 说明你没有登录 <https://your-domain/PluginCore/Admin>      
> 1. 访问: <https://your-domain/PluginCore/Admin>  进入 `PluginCore Admin` 在插件列表中添加[KonataPlugin插件](https://github.com/yiyungent/KnifeHub/releases?q=KonataPlugin&expanded=true)
> 2. 访问: <https://your-domain/Plugins/KonataPlugin> 进行 QQ机器人 登录

> 若为 `短信验证` , 则直接输入收到的验证码, 点击 `提交验证` 即可

> 若为 `滑动验证` , 则 `点击前往验证`, 浏览器在 `滑动验证` 页面 按 `F12`, 再选择 `Network`, 通过滑动验证后, 复制 `ticket` 如下 (不要包括双引号), 将 `ticket` 粘贴到输入框, 点击 `提交验证` 即可

![login_slide.png](/Plugins/KonataPlugin/images/login_slide.png)

> 注意验证不要等待太久, 否则尝试刷新登录页面 以重新登录及获取新验证


> **注意**    
> ⭐⭐⭐⭐当 **无法登录** 时⭐⭐⭐⭐
> 
> - 当 `滑动验证` `验证通过` 后, 等待一会 , 还是 `无法进入已登录状态`     
>   
> 可 下载 [Releases - KonataApp - Assets](https://github.com/yiyungent/KnifeHub/releases?q=KonataApp&expanded=true) 在本地登录成功后,    
> 获取 **BotKeyStore.json** 后, 在登录页面使用 **配置** 方式登录
> 
> > - 大部分人电脑为 `Windows 64 位`, 点击 **KonataApp-win-x64.zip** 下载即可, 下载到本地解压, 双击 **KonataApp.exe**
> > - 运行 `KonataApp.exe` 会自动给出输入提示, 按提示操作即可       
> > - 运行 `KonataApp.exe` 无需额外安装 `.NET SDK 或 Runtime`, 程序已打包





## 下载数据库

> 所有数据保存到数据库 **KonataPlugin.sqlite** 中

- [下载数据库 (KonataPlugin.sqlite)](/Plugins/KonataPlugin/Download)




## 相关

> 项目地址: [https://github.com/yiyungent/QQBotHub/tree/main/plugins/KonataPlugin](https://github.com/yiyungent/QQBotHub/tree/main/plugins/KonataPlugin)             
> 本项目基于 [KonataDev/Konata.Core: QQ(Android) protocol core implemented with pure C#.](https://github.com/KonataDev/Konata.Core)


<!-- Matomo Image Tracker-->
<img referrerpolicy="no-referrer-when-downgrade" src="https://matomo.moeci.com/matomo.php?idsite=2&amp;rec=1&amp;action_name=Plugins.KonataPlugin-v0.2.1.README" style="border:0" alt="" />
<!-- End Matomo -->
