


## 介绍

> 本项目为 Duplicati 辅助插件, 可通过依赖本插件开发相关插件



## 使用

### 1. 准备 Telegram

> 1. 打开你的 Telegram

> 2. `@BotFather` 通过此机器人创建你的 Telegram 机器人, 获取 机器人 `Token` , 即为 `BotToken`

> 3. `@userinfobot` 通过此机器人获取你的 `Id` , 即为 `ChatId`


### 2. 方式1: 全局设置 Duplicati

- [右键复制本链接, 然后再替换其中内容](/api/Duplicati/TgBotToken/replace-your-botToken/to/TgChatId/replace-your-chatId/apply)

```
/api/Duplicati/TgBotToken/替换为你的botToken/to/TgChatId/替换为你的chatId/apply
```

> Duplicati - 设置 - 默认选项 - 以文本形式编辑

```
--send-http-result-output-format=Duplicati
--send-http-url=替换为你的链接
```

### 2. 方式2: 全局设置 Duplicati (推荐)

- [右键复制本链接, 然后再替换其中内容](/api/Duplicati/replace-your-key/apply)

```
/api/Duplicati/替换为你的来源key,用作区分/apply
```

> Duplicati - 设置 - 默认选项 - 以文本形式编辑

```
--send-http-result-output-format=Duplicati
--send-http-message={"Name":"Duplicati","OperationName":"%OPERATIONNAME%","BackupName":"%backup-name%","ParsedResult":"%PARSEDRESULT%","LocalPath":"%LOCALPATH%","RemoteUrl":"%REMOTEURL%"}<--DuplicatiPlugin-->%RESULT%
--send-http-url=替换为你的链接
```



## 下载数据库

> 所有数据保存到数据库 **DuplicatiPlugin.sqlite** 中

- [下载数据库 (DuplicatiPlugin.sqlite)](/Plugins/DuplicatiPlugin/Download)




## 相关

> 项目地址: [https://github.com/yiyungent/KnifeHub/tree/main/plugins/DuplicatiPlugin](https://github.com/yiyungent/KnifeHub/tree/main/plugins/DuplicatiPlugin)             
> 本项目基于 [TelegramBots/Telegram.Bot: .NET Client for Telegram Bot API](https://github.com/TelegramBots/Telegram.Bot)

<!-- Matomo Image Tracker-->
<img referrerpolicy="no-referrer-when-downgrade" src="https://matomo.moeci.com/matomo.php?idsite=2&amp;rec=1&amp;action_name=PluginCore.Plugins..DuplicatiPlugin-v0.2.4.README" style="border:0" alt="" />
<!-- End Matomo -->
