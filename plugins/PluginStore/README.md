




## 使用


### 设置 `SecondsPeriod`

> `SecondsPeriod` 为备份周期 (秒数), 不要太短, 为数值, 不要加双引号, 默认为: 180 秒, 即 3分钟 备份一次       

> **注意:** 强烈建议 `Railway` / `Heroku` 用户 设置时间长一点, 备份时 消耗较大, 此时无法访问 或 出现其它异常为正常情况, 等待备份完成即可

```json
{
  "SecondsPeriod": 180,
  "Telegram": {
    "Enable": false,
    "Token": "",
    "ChatId": ""
  }
}
```


### 将 备份文件 发送到 Telegram

#### 设置 `Telegram`

> 将备份文件发送给你的目标 Telegram 机器人

> 1. 启用 Telegram, 即 设置 `Enable`: true  

> 2. `@BotFather` 通过此机器人创建你的 Telegram 机器人, 获取 机器人 `Token` , 填入设置 `Token`

> 3. `@userinfobot` 通过此机器人获取你的 `Id` , 填入设置 `ChatId`




### 使用 备份文件 恢复插件数据

#### 下载备份文件后, 解压

> 你将看到如下文件夹

```
App_Data
Plugins
Plugins_wwwroot
```

> 一般来说，你只需要 `Plugins` 文件夹

> 例如进入 `Plugins/QQStatPlugin` 文件夹, 可以看到一堆插件文件, 这时 `Ctrl+A` 全选文件, 右键菜单 点击 **添加到压缩文件**,     
> 选择 **zip** 格式, 然后在 `上传插件` 页面, 上传此插件 -> 安装 -> 启用 即可


> **注意**: 若 插件列表 已存在此插件, 需要先删除同名插件, 然后再利用备份文件恢复



### 自动删除本地备份文件

> 由于备份在服务器本地, 发送到 `Telegram` 并不会删除本地备份文件, 因此会不断累积占用   
> 后面可能会出下载 服务器本地备份文件 功能

设置 `LocalBackupsMaxNum` 本地备份文件最大保存份数, 当超过份数, 会自动删除较早的本地备份文件

<!-- Matomo Image Tracker-->
<img referrerpolicy="no-referrer-when-downgrade" src="https://matomo.moeci.com/matomo.php?idsite=2&amp;rec=1&amp;action_name=Plugins.BackupPlugin-v0.1.1.README" style="border:0" alt="" />
<!-- End Matomo -->
