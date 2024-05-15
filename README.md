![header](https://capsule-render.vercel.app/api?type=waving&color=timeGradient&height=200&section=header&reversal=true&animation=twinkling&fontSize=70&fontAlignY=30&descAlignY=50&text=KnifeHub&desc=🧰%20简单易用的效率工具平台)

<!-- <h1 align="center">KnifeHub</h1> -->

<!-- > 🧰 简单易用的效率工具平台 -->

[![repo size](https://img.shields.io/github/repo-size/yiyungent/KnifeHub.svg?style=flat)]()
[![LICENSE](https://img.shields.io/github/license/yiyungent/KnifeHub.svg?style=flat)](https://github.com/yiyungent/KnifeHub/blob/master/LICENSE)
[![Telegram Group](https://img.shields.io/badge/Telegram-Group-blue)](https://t.me/xx_dev_group)
[![QQ Group](https://img.shields.io/badge/QQ%20Group-894031109-deepgreen)](https://jq.qq.com/?_wv=1027&k=q5R82fYN)
<!-- ![hits](https://api-onetree.moeci.com/hits.svg?id=KnifeHub) -->
[![docker pulls](https://img.shields.io/docker/pulls/yiyungent/knifehub)](https://hub.docker.com/r/yiyungent/knifehub)
[![docker version](https://img.shields.io/docker/v/yiyungent/knifehub/latest?label=docker%20image%20ver.)](https://hub.docker.com/r/yiyungent/knifehub)
[![Docker Image CI/CD - Release - knifehub](https://github.com/yiyungent/KnifeHub/actions/workflows/docker-push-knifehub-release.yml/badge.svg)](https://github.com/yiyungent/KnifeHub/actions/workflows/docker-push-knifehub-release.yml)
[![Github All Releases](https://img.shields.io/github/downloads/yiyungent/KnifeHub/total.svg)](https://hanadigital.github.io/grev/?user=yiyungent&repo=KnifeHub)
[![CLA assistant](https://cla-assistant.io/readme/badge/yiyungent/KnifeHub)](https://cla-assistant.io/yiyungent/KnifeHub)
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fyiyungent%2FKnifeHub.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2Fyiyungent%2FKnifeHub?ref=badge_shield)


## Introduce

> 🧰 简单易用的效率工具平台

- **简单易用** - 拒绝折腾, 专注于工作, 而非工具
- **WebUI 可视化** - 无需再在 Console 上操作, 轻松上手
- **插件化架构** - 轻松使用插件扩展
- **多平台多架构** - win,linux,osx,amd,arm 均可

> [!IMPORTANT]           
> **通知** : 请尽快更新到 最新版 (KnifeHub-**v1.1.0** +) ,        
> **v1.1.0** 开始将静默发送异常日志等到服务端, 使用本软件即代表同意此隐私授权

> [!NOTE]    
> **注意** : 本项目仅供学习使用, 所有第三方插件与本项目无关


## 技术栈

- 后端: 主服务: ASP.NET Core (.NET/C#) & Nginx
- 后端: 部分 API 服务: Hono (Node.js & TypeScript) & Flask (Python)
- 前端: Next.js & React.js & TypeScript & Material UI & pnpm

> 关联线上产品:         
> - [https://120365.xyz](https://120365.xyz)          
>   - 在线工具集   
>   - 数据分析可视化    
>   - 时间管理    
>   - 效率工具箱    


## 功能

- [x] **大部分功能由插件提供**

<details>
  <summary><strong>点我 打开/关闭 本仓库 维护的 官方插件</strong></summary>

> > **PUP (Public Universal Plugin)**            
> 
> **不依赖于特定主程序, 可在 `PluginCore` 环境通用**   
> 若有前置依赖插件, 安装启用后, 可独立运行   

请自行前往 [plugins](https://github.com/yiyungent/KnifeHub/tree/main/plugins) 查看插件列表,    
插件可在 [Releases](https://github.com/yiyungent/KnifeHub/releases) 搜索下载

</details>


## Online demo

- [https://knifehub.onrender.com](https://knifehub.onrender.com/)
  - 用户名: `admin` 密码: `ABC12345`
  - 不定时重置数据
  - 非最新版本


## Screenshots

<!-- ### PluginCore Admin -->

<!-- ![PluginCore Admin](./screenshots/PluginCore-Admin.png) -->



## Quick Start

### 部署

> `PluginCore Admin 用户名` 与 `PluginCore Admin 密码` 为你自己设置的后台登录用户名与密码, 随意设置即可, 自己记住就行

#### 方式1：原生部署 (适合小白)

> [Releases - KnifeHub, 点我下载](https://github.com/yiyungent/KnifeHub/releases?q=KnifeHub&expanded=true)         
> 找到并下载 **KnifeHub** 相应平台压缩包,       
> (Windows 系统 一般下载 Windows-64 版本即可, 即 **KnifeHub-win-x64.zip**)   
> 解压里面有 **KnifeHub.Web.exe** , 运行它即可 (无需再安装其它任何运行库)    
> 黑窗口中会显示一个 url 地址（Now listening on:后跟随的地址）, 复制到浏览器中打开即可 
> 保持此黑窗口在后台运行即可

#### 方式2: 使用 Zeabur 免费 一键部署

> - 点击下方按钮 一键部署        
> - 免费注册, 无需信用卡验证      
> - 每月 `$5.00` 免费额度
> - 需要一定天数后延长免费时长
> - 支持 **支付宝**

[![Deploy on Zeabur](https://zeabur.com/button.svg)](https://zeabur.com/templates/IW8G70?referralCode=yiyungent)

##### Zeabur 环境变量

| 环境变量名称                | 必填 | 备注                    |
| --------------------------- | ---- | ----------------------- |
| `PLUGINCORE_ADMIN_USERNAME` | √    | PluginCore Admin 用户名 |
| `PLUGINCORE_ADMIN_PASSWORD` | √    | PluginCore Admin 密码   |

> 注意:    
> 修改环境变量后, 需 `Redeploy` 才能生效

#### 方式3: 使用 Render 免费 一键部署 

> 注意: 此种方式 `KonataPlugin` 可能无法登录上线

> - 点击下方按钮 一键部署        
> - 免费注册, 无需信用卡验证      
> - Free Instance Hours: 750 hours/month
> - Free Bandwidth: 100 GB/month
> - Free Build Minutes: 500 min/month
> - **注意** : 免费实例类型上的 Web 服务在闲置 15 分钟后会自动停止运行, 当一个新的免费服务请求进来时, Render 会再次启动它, 以便它可以处理该请求, 因此为了保证存活, 请使用第三方监控保活, 例如: [UptimeRobot: 免费网站监控服务](https://uptimerobot.com/)   

[![Deploy to Render](http://render.com/images/deploy-to-render-button.svg)](https://render.com/deploy?repo=https://github.com/yiyungent/KnifeHub)
<!-- [![Deploy to Render](http://render.com/images/deploy-to-render-button.svg)](https://yourls.yiyungent.eu.org/knifehubdeployrender) -->

##### Render 环境变量

| 环境变量名称                | 必填 | 备注                    |
| --------------------------- | ---- | ----------------------- |
| `PLUGINCORE_ADMIN_USERNAME` | √    | PluginCore Admin 用户名 |
| `PLUGINCORE_ADMIN_PASSWORD` | √    | PluginCore Admin 密码   |

#### 方式4: 使用 Railway 免费 一键部署 

> - 点击下方按钮 一键部署        
> - 免费注册, 无需信用卡验证      
> - **一次性** `$5.00` 免费额度 / 500 小时免费执行时间
> - 无需保活, 在免费额度用完之前永不停止运行

[![Deploy on Railway](https://railway.app/button.svg)](https://railway.app/new/template/A3JY-J?referralCode=8eKBDA)


##### Railway 环境变量

| 环境变量名称                | 必填 | 备注                    |
| --------------------------- | ---- | ----------------------- |
| `PLUGINCORE_ADMIN_USERNAME` | √    | PluginCore Admin 用户名 |
| `PLUGINCORE_ADMIN_PASSWORD` | √    | PluginCore Admin 密码   |


> 注意:    
> - Railway 修改环境变量 会 触发 重新 Deploy   
> - Railway 重新 Deploy 后会删除数据, 你安装的所有插件及数据都将清空。

#### 方式5: 使用 Heroku 一键部署 

> 注意：此种方式 `KonataPlugin` 可能无法登录上线

> - 点击下方按钮 一键部署       
> - **注意** : Heroku 应用一段时间不访问会自动休眠, 因此为了保证存活, 请使用第三方监控保活, 例如: [UptimeRobot: 免费网站监控服务](https://uptimerobot.com/)   

[![Deploy on Heroku](https://www.herokucdn.com/deploy/button.svg)](https://heroku.com/deploy?template=https://github.com/yiyungent/KnifeHub)
<!-- [![Deploy on Heroku](https://www.herokucdn.com/deploy/button.svg)](https://yourls.yiyungent.eu.org/knifehubdeployheroku) -->

##### Heroku 环境变量

| 环境变量名称                | 必填 | 备注                    |
| --------------------------- | ---- | ----------------------- |
| `PLUGINCORE_ADMIN_USERNAME` | √    | PluginCore Admin 用户名 |
| `PLUGINCORE_ADMIN_PASSWORD` | √    | PluginCore Admin 密码   |

#### 方式6: 使用 Docker

> **注意** : 请不要映射/挂载 容器内 **/app/Plugins/** 目录, 否则可能导致 **插件无法正常上传** , 此问题解决中

```bash
docker run -d --restart=always -p 53213:80 -e ASPNETCORE_URLS="http://*:80" -e ASPNETCORE_ENVIRONMENT="Production" -e TZ="Asia/Shanghai"  --name knifehub yiyungent/knifehub
```

```bash
# 可选, 进入容器 管理, 例如修改 /app/App_Data/PluginCore.Config.json 中的 PluginCore Admin 用户名与密码
docker exec -it knifehub bash
```

> 现在访问: <http://your-domain/PluginCore/Admin>  your-domain 为黑窗口中显示的 url 地址


### 插件管理

访问: <https://your-domain/PluginCore/Admin>  进入 `PluginCore Admin`

> 插件:   
> 下载插件包, 
> > 插件包下载见 [Release](https://github.com/yiyungent/KnifeHub/releases) , 
> > 直接插件上传 下载的 `QQHelloWorldPlugin-net6.0.zip` 即可    
> 
> 然后直接 `上传 -> 安装 -> 文档 -> 设置 -> 启用 -> 文档` 即可

### 守护进程(daemon)

> 守护进程: 开机自启动, 程序异常退出时自动重启程序

#### Windows    

##### 方式1: 简易 PowerShell 实现

> PowerShell 实现简易 `KnifeHub.Web.exe` 异常退出时自动重启 `KnifeHub.Web.exe`       
> 下载 仓库根目录 [utils/start.ps1](https://raw.githubusercontent.com/yiyungent/KnifeHub/main/utils/start.ps1) [utils/corn.ps1](https://raw.githubusercontent.com/yiyungent/KnifeHub/main/utils/corn.ps1)            
> 将 `start.ps1` 与 `corn.ps1` 放入与 `KnifeHub.Web.exe` 同目录文件夹下,   
> 使用 `Windows Terminal` / `PowerShell` 运行 `start.ps1` 即可

```powershell
# 确保进入程序目录再执行
cd "KnifeHub-win-x64/win-x64"

./start.ps1
```
##### 方式2: [ProcessGuard](https://github.com/KamenRiderKuuga/ProcessGuard)

> 强烈推荐新人使用, 稳定强大

#### Linux

> 推荐使用 `Supervisor`

### 更新 KnifeHub

> 查看最新版 [Releases - KnifeHub](https://github.com/yiyungent/KnifeHub/releases?q=KnifeHub&expanded=true)


> 若你使用 `Railway` 一键部署,         
> 只需要修改 `Railway` 创建的仓库 (例如: `KnifeHub`) 里的 `Dockerfile` 文件里的 `yiyungent/knifehub:v0.5.2` , 更新最后的版本号 `v0.5.2` 到最新版即可


> **注意:**    
> 请更新前导出插件数据, `QQStatPlugin` 支持下载数据库到本地, 然后下载最新插件包, 解压, 将数据库文件替换为你导出的数据库文件, 然后在有 `QQStatPlugin.sqlite` 的路径下打包所有文件 为 zip, 上传插件即可           
> `插件设置` 可以通过保持打开插件设置页面的方式, 重新安装插件后, 再在此页面点击保存


> 一些更新 docker knifehub 可能需要用到的命令

```bash
cd /www/wwwroot/knifehub*
# docker-data 位置用于保存当前容器数据
mkdir -p docker-data/App_Data/
mkdir -p docker-data/Plugins/
mkdir -p docker-data/Plugins_wwwroot/
# 保存当前数据
docker cp knifehub:/app/appsettings.json docker-data/appsettings.json
docker cp knifehub:/app/App_Data/PluginCore.Config.json docker-data/App_Data/PluginCore.Config.json
docker cp knifehub:/app/App_Data/plugin.config.json docker-data/App_Data/plugin.config.json
docker cp knifehub:/app/Plugins/ docker-data/
docker cp knifehub:/app/Plugins_wwwroot/ docker-data/

# 移除当前
docker stop knifehub
docker rm knifehub
docker rmi yiyungent/knifehub

# 获取最新
# 建议不要使用 latest , 而是指定最新的版本号, 有可能你使用的 docker 仓储源还未同步, 而导致 latest 仍为旧版
#docker pull yiyungent/knifehub:v1.0.0
docker pull yiyungent/knifehub:latest
docker run -d --restart=always -p 53213:80 -e ASPNETCORE_URLS="http://*:80" -e ASPNETCORE_ENVIRONMENT="Production" -e TZ="Asia/Shanghai"  --name knifehub yiyungent/knifehub

# 这里我将原本备份的数据保存到了这个路径, 进入这个路径, 将备份数据覆盖到 docker 容器中
cd docker-data

docker cp appsettings.json knifehub:/app/appsettings.json
docker cp App_Data/PluginCore.Config.json knifehub:/app/App_Data/PluginCore.Config.json
docker cp App_Data/plugin.config.json knifehub:/app/App_Data/plugin.config.json
docker cp Plugins/ knifehub:/app/
docker cp Plugins_wwwroot/ knifehub:/app/

# 重启容器
docker restart knifehub
```

> 注意:   
> 使用 `yiyungent/knifehub:latest-amd-full` 完整版,  
> 每次升级, 内嵌的 `filebrowser` 都会被重置为默认用户名: `admin`, 默认密码: `admin`, 升级完需立即修改为自己的用户名及密码




## 插件开发

### 开发必读

> 注意:  
> 所有纯基于 `PluginCore.IPlugins` 开发的插件都通用,   
> 下载插件包, 然后 `上传 -> 安装 -> 设置 -> 启用` 即可


> 插件开发 可参考:   
> - [插件开发 | PluginCore](http://yiyungent.github.io/PluginCore/zh/PluginDev/Guide/)      
> - **建议** 参考: [./plugins/QQHelloWorldPlugin](https://github.com/yiyungent/KnifeHub/tree/main/plugins/QQHelloWorldPlugin)

> KnifeHub 插件开发包  
> 插件开发包中已包含:   
> - `PluginCore.IPlugins.AspNetCore`

> 注意: 如果你不需要 `KnifeHub.Sdk` 中的一些 `Utils` , 那么建议直接依赖 `PluginCore.IPlugins.AspNetCore` , 减少依赖项, 使之称为通用插件

```powershell
dotnet add package KnifeHub.Sdk
```

> **注意**:   
> - 本项目目前直接使用的 `PluginCore` 插件框架, 插件采用激发式, 插件工作完成后, 实例会立即销毁, 无法常驻后台
> - 若需要定时任务, 可以使用 `ITimeJobPlugin`, 可见 `PluginCore` 的文档    
> - 由于 QQBot 本身为常驻, 因此需额外注意 `IPluginFinder` 的服务的生命周期/范围, 这点和在 `ASP.NET Core` 的 `Controller` 中 直接使用不同

### 在线开发

<!-- https://yourls.yiyungent.eu.org/replit0knifehub0v1 -->

> > #### [使用 Replit 在线开发 | 点我前往](https://replit.com/@yiyungent/KnifeHub?v=1)
> - 预设 Replit 开发模板
> - 免费注册, 无需信用卡验证
> - 免费在线实验环境, 可启动项目
> - `Console/Shell` 支持
> - 未注册用户注册登录后, 再回到此处点击上方链接, `fork` 此 Replit 模板项目, 基于此开发更方便, 但此模板可能不是最新版, 建议在实验环境中 `git pull`
> - 注意: 此实验环境 `KonataPlugin` 无法登录上线, 你依然需要做本地测试



## 相关项目

- [KonataDev/Konata.Core](https://github.com/KonataDev/Konata.Core) - QQ Android 协议核心库
- [Chianne1025/QQChannelFramework](https://github.com/Chianne1025/QQChannelFramework) - QQ 频道 核心库
- [yiyungent/PluginCore](https://github.com/yiyungent/PluginCore) - 插件系统

### 社区插件

> 欢迎 Pull Request !

- [SweelLong/AntiRecall: 这是基于KnifeHub - KonataPlugin的开源插件，功能是防止消息撤回。](https://github.com/SweelLong/AntiRecall)       
- [SweelLong/RandomImage: 这是基于QQBotHub的开源插件，主要功能是发送随机图片。（短期内不会更新）](https://github.com/SweelLong/RandomImage)       


## 赞助者

本列表由 [afdian-action](https://github.com/yiyungent/afdian-action) 自动更新

感谢这些来自爱发电的赞助者：

<!-- AFDIAN-ACTION:START -->

<a href="https://afdian.net/u/e98feb9e12d511efa7b352540025c377">
    <img src="https://pic1.afdiancdn.com/default/avatar/avatar-purple.png?imageView2/1/?imageView2/1/w/120/h/120" width="40" height="40" alt="爱发电用户_e98fe" title="爱发电用户_e98fe"/>
</a>
<a href="https://afdian.net/u/1ec1819cde6c11ec95b252540025c377">
    <img src="https://pic1.afdiancdn.com/user/1ec1819cde6c11ec95b252540025c377/avatar/0bd8c71ba926c7649a8a2646d4946fb2_w639_h640_s85.jpeg?imageView2/1/w/120/h/120" width="40" height="40" alt="RemMai" title="RemMai"/>
</a>
<a href="https://afdian.net/u/98e9914c457911ee95eb52540025c377">
    <img src="https://pic1.afdiancdn.com/user/98e9914c457911ee95eb52540025c377/avatar/0c7e8ddbbf27c6614fabbb00d4e907a0_w684_h683_s27.jpeg?imageView2/1/w/120/h/120" width="40" height="40" alt="浮沉" title="浮沉"/>
</a>
<a href="https://afdian.net/u/c4a50eea706211ebb48352540025c377">
    <img src="https://pic1.afdiancdn.com/user/c4a50eea706211ebb48352540025c377/avatar/bd2c86c3f773240acf4b74ca8ec3eef8_w640_h640_s22.jpeg?imageView2/1/w/120/h/120" width="40" height="40" alt="WiMi" title="WiMi"/>
</a>
<a href="https://afdian.net/u/459023b8e07b11eb92af52540025c377">
    <img src="https://pic1.afdiancdn.com/user/459023b8e07b11eb92af52540025c377/avatar/9238a84b58fdc0aa6093340709d63fd4_w1500_h925_s935.jpg?imageView2/1/w/120/h/120" width="40" height="40" alt="Dr" title="Dr"/>
</a>
<a href="https://afdian.net/u/6c944aa0a55f11eabd5f52540025c377">
    <img src="https://pic1.afdiancdn.com/user/6c944aa0a55f11eabd5f52540025c377/avatar/e0b9977363fe0b475e0fb6300c43b4be_w480_h480_s13.jpg?imageView2/1/w/120/h/120" width="40" height="40" alt="MonoLogueChi" title="MonoLogueChi"/>
</a>

<details>
  <summary>点我 打开/关闭 赞助者列表</summary>

<a href="https://afdian.net/u/e98feb9e12d511efa7b352540025c377">
爱发电用户_e98fe
</a>
<span>( 1 次赞助, 共 ￥30 ) 留言: </span><br>
<a href="https://afdian.net/u/1ec1819cde6c11ec95b252540025c377">
RemMai
</a>
<span>( 1 次赞助, 共 ￥15 ) 留言: 感谢提供插件灵感。
...</span><br>
<a href="https://afdian.net/u/98e9914c457911ee95eb52540025c377">
浮沉
</a>
<span>( 1 次赞助, 共 ￥100 ) 留言: </span><br>
<a href="https://afdian.net/u/c4a50eea706211ebb48352540025c377">
WiMi
</a>
<span>( 1 次赞助, 共 ￥30 ) 留言: 感谢分享</span><br>
<a href="https://afdian.net/u/459023b8e07b11eb92af52540025c377">
Dr
</a>
<span>( 1 次赞助, 共 ￥10 ) 留言: 非常感谢</span><br>
<a href="https://afdian.net/u/6c944aa0a55f11eabd5f52540025c377">
MonoLogueChi
</a>
<span>( 1 次赞助, 共 ￥28.2 ) 留言: 感谢你的开源项目</span><br>

</details>
<!-- 注意: 尽量将标签前靠,否则经测试可能被 GitHub 解析为代码块 -->
<!-- AFDIAN-ACTION:END -->


## Donate

KnifeHub is an Apache-2.0 licensed open source project and completely free to use. However, the amount of effort needed to maintain and develop new features for the project is not sustainable without proper financial backing.

We accept donations through these channels:

- <a href="https://afdian.net/@yiyun" target="_blank">爱发电</a> (￥5.00 起)
- <a href="https://dun.mianbaoduo.com/@yiyun" target="_blank">面包多</a> (￥1.00 起)


## License
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fyiyungent%2FKnifeHub.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2Fyiyungent%2FKnifeHub?ref=badge_large)

## Author

- KnifeHub.Web `Apache-2.0`
- KnifeHub.Sdk `Apache-2.0`
- QQHelloWorldPlugin `GPL-3.0`
- MoLiPlugin `GPL-3.0`
- QQStatPlugin `GPL-3.0`
- BackupPlugin `MIT`
- AutoLoginPlugin `GPL-3.0`
- QQNotePlugin `GPL-3.0`
- ZhiDaoPlugin `GPL-3.0`
- WebMonitorPlugin `Apache-2.0`
- QQChannelPlugin `MIT`
- MoLi4QQChannelPlugin `MIT`

**KnifeHub** © [yiyun](https://github.com/yiyungent), Released under the [Apache-2.0](./LICENSE) License.<br>
Authored and maintained by yiyun with help from contributors ([list](https://github.com/yiyungent/KnifeHub/contributors)).

> GitHub [@yiyungent](https://github.com/yiyungent) Gitee [@yiyungent](https://gitee.com/yiyungent)

<!-- Matomo Image Tracker-->
<img referrerpolicy="no-referrer-when-downgrade" src="https://matomo.moeci.com/matomo.php?idsite=2&amp;rec=1&amp;action_name=GitHub.KnifeHub.README" style="border:0" alt="" />
<!-- End Matomo -->


