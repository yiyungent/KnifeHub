
<h1 align="center">QQBotHub</h1>

> QQ 机器人 | 基于 Konata.Core | 一键免费部署

[![repo size](https://img.shields.io/github/repo-size/yiyungent/QQBotHub.svg?style=flat)]()
[![LICENSE](https://img.shields.io/github/license/yiyungent/QQBotHub.svg?style=flat)](https://github.com/yiyungent/QQBotHub/blob/master/LICENSE)
[![QQ Group](https://img.shields.io/badge/QQ%20Group-894031109-deepgreen)](https://jq.qq.com/?_wv=1027&k=q5R82fYN)
![hits](https://api-onetree.moeci.com/hits.svg?id=QQBotHub)

## Introduce


QQ 机器人 | 基于 Konata.Core | 一键免费部署

- **Web 可视化** 无需再在 Console 上操作
- **插件化架构** 轻松使用插件扩展

> 本项目基于以下:    
> - [KonataDev/Konata.Core](https://github.com/KonataDev/Konata.Core)
> - [yiyungent/PluginCore](https://github.com/yiyungent/PluginCore)


## 功能

- [x] 在线登录
- [x] 其它大部分功能由 插件提供

## Quick Start

> 部署完成后
> > 一定要先登录 `PluginCore Admin`   
> 1. 访问: <https://yourdomain/PluginCore/Admin>  进入 `PluginCore Admin`
> 2. 访问: <https://yourdomain/login> 进行 QQ机器人 登录

> 插件:   
> 下载插件包, 
> > 插件包下载见 [Release](https://github.com/yiyungent/QQBotHub/releases) , 
> > 直接插件上传 下载的 `QQHelloWorldPlugin-net6.0.zip` 即可    
> 
> 然后直接 `上传 -> 安装 -> 文档 -> 设置 -> 启用 -> 文档` 即可

### 方式1: 使用 Railway 免费 一键部署 

[![Deploy on Railway](https://railway.app/button.svg)](https://railway.app/new/template/A3JY-J?referralCode=8eKBDA)


#### Railway 环境变量

| 环境变量名称                | 必填 | 备注                    |
| --------------------------- | ---- | ----------------------- |
| `PLUGINCORE_ADMIN_USERNAME` | √    | PluginCore Admin 用户名 |
| `PLUGINCORE_ADMIN_PASSWORD` | √    | PluginCore Admin 密码   |
| `BOT_QQ` | √    | 机器人 QQ   |
| `BOT_PASSWORD` | √    | 机器人 QQ 密码   |


> 注意:    
> - Railway 修改环境变量 会 触发 重新 Deploy   
> - Railway 重新 Deploy 后会删除数据, 你安装的所有插件及数据都将清空。

## Screenshots



## Donate

QQBotHub is an GPL-3.0 licensed open source project and completely free to use. However, the amount of effort needed to maintain and develop new features for the project is not sustainable without proper financial backing.

We accept donations through these channels:

- <a href="https://afdian.net/@yiyun" target="_blank">爱发电</a> (￥5.00 起)
- <a href="https://dun.mianbaoduo.com/@yiyun" target="_blank">面包多</a> (￥1.00 起)

## Author

**QQBotHub** © [yiyun](https://github.com/yiyungent), Released under the [GPL-3.0](./LICENSE) License.<br>
Authored and maintained by yiyun with help from contributors ([list](https://github.com/yiyungent/QQBotHub/contributors)).

> GitHub [@yiyungent](https://github.com/yiyungent) Gitee [@yiyungent](https://gitee.com/yiyungent)

