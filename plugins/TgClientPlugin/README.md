

## 使用

> 前往 [下方链接](https://my.telegram.org/apps) 登录 Telegram , 再创建一个 app, 复制 `App api_id` 与 `App api_hash` 填入 设置中对应项, 保存      
> 注意: `ApiId` 的值 不要加双引号 (`""`)
> 
> 打开你的 **已登录** 的 Telegram 客户端/Web , 方便接收验证码 (**Login code**), 提示登录成功后, 你的 **已登录** 的 Telegram 客户端/Web 也会收到提示 (来自 Telegram 官方联系人)

- https://my.telegram.org/apps

> 输入你的 手机号((中国大陆 +86)), 点击登录, 然后会提示输入验证码, 从 **已登录** 的 Telegram 客户端的 Telegram 官方联系人中获取, 然后输入验证码, 再次点击提交即可,   
> 
> PS: 目前没有已登录状态的显示, 代做中, **反复提示输入手机号**, 就点击一次退出, 然后再输入手机号, 点击登录

- [前往登录](/plugins/TgClientPlugin)


## 代理

> 若 你部署地 无法访问 Telegram API, 那么你可能需要设置 **Proxy**

> 例如 对于 **v2rayN** 的默认配置

```json
"Proxy": {
    "ProxyEnabled": true,
    "ProxyHost": "127.0.0.1",
    "ProxyPort": 10808
}
```


## 下载数据库

> 所有数据保存到数据库 **TgClientPlugin.sqlite** 中

- [下载数据库 (TgClientPlugin.sqlite)](/api/Plugins/TgClientPlugin/Download)




## 相关

> 项目地址: [https://github.com/yiyungent/KnifeHub/tree/main/plugins/TgClientPlugin](https://github.com/yiyungent/KnifeHub/tree/main/plugins/TgClientPlugin)             
> 本项目基于 [wiz0u/WTelegramClient: Telegram Client API (MTProto) library written 100% in C# and .NET Standard](https://github.com/wiz0u/WTelegramClient)

<!-- Matomo Image Tracker-->
<img referrerpolicy="no-referrer-when-downgrade" src="https://matomo.moeci.com/matomo.php?idsite=2&amp;rec=1&amp;action_name=Plugins.TgClientPlugin-v0.0.2.README" style="border:0" alt="" />
<!-- End Matomo -->
