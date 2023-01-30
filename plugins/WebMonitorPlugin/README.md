
# 介绍

> **注意**: **启用本插件较慢** , 请耐心等待提示 `启用成功` , 若出错, 禁用再启用即可

> **注意** : 本插件需要浏览器环境, 如: 此 Docker 镜像 `yiyungent/knifehub:latest-amd-chrome` 中就打包好了浏览器环境

# 基础

- [任务控制台](/plugins/WebMonitorPlugin)


- [设置](/plugincore/admin/index.html#/plugins/settings/WebMonitorPlugin) 里配置提醒通知


# 插件设置

`SecondsPeriod` : 一般保持默认 60 秒的监控频率, 当然若服务器压力大, 可以调高

`Selenium.ChromeDriverDirectory` : "" 一般保持为默认空字符串即可, 即使用 当前主程序所在目录 (/app/), 即使用 `yiyungent/knifehub:v1.0.5-amd-chrome` 等镜像 保持默认空字符串即可

`CommandTimeoutMinute.CommandTimeoutMinute` : 一般保持默认 5 分钟即可


# 通知

> PS: 通知属于 预定任务

## Telegram 通知

> Telegram 通知通过 Telegram bot 机器人实现

- `Token`: Telegram 机器人用于访问 HTTP API 的 token, 通过 [@BotFather](https://t.me/BotFather) 创建机器人获取，必填。
- `ChatId`: 接收消息对象的 `chat_id`, 可以是单一用户、频道、群组，通过 [@userinfobot](https://t.me/userinfobot) 获取，必填。
- `Enable`: 是否启用 Telegram 通知, 启用填 `true`, 禁用填 `false`, 必填

> PS: 目前在发送 Telegram 通知时, 还会附上一张目标 Url 网页截图

# JavaScript 条件 API

```javascript
// 注意: 此方法已废弃
// window.WebMonitorPlugin.JavaScriptConditionResult = false;

// 设置 JavaScript 条件 的结果, 若为 true, 则执行预定(通知)任务
localStorage.setItem("WebMonitorPlugin.JavaScriptConditionResult", false);

// 通过在 js 条件中设置 ForceWaitAfterJsConditionExecute 来改变预先设置的 js 条件 执行后强制等待, 默认为 预先设置的值
localStorage.setItem("WebMonitorPlugin.ForceWaitAfterJsConditionExecute", false);

// 仅当 预定(通知)任务 成功后, 才会判断此设置, 否则一律不禁用本任务
// 默认 false, 即 当预定(通知)任务 成功后, 本任务禁用
localStorage.setItem("WebMonitorPlugin.Enable", false);

// 永久存储, 可用于监控网页变化用（将之前的信息存储起来，然后下次访问时进行对比）
// 注意: 范围仅为当前任务
localStorage.setItem("WebMonitorPlugin.Storage.YourStorageKey", "your data");
// 上次 JavaScript 条件执行中设置后，下次即可通过下方使用
var yourStorageData = localStorage.getItem("WebMonitorPlugin.Storage.YourStorageKey");

// 添加额外 Telegram 通知消息
// 此消息 不受 JavaScriptConditionResult 影响, 只要不为空, 就会尝试发送, 默认为空数组 "[]"
// 注意: 每条消息只能使用 Telegram 支持的 HTML 格式标签,大多标签并不支持, 参考: https://core.telegram.org/bots/api#formatting-options
var tgMessage = ["消息1","消息2"];
localStorage.setItem("WebMonitorPlugin.TgMessageList", JSON.stringify(tgMessage));

// 更多 API 蓄力中...
```


# 执行流程

`浏览器打开 Url` -> `强制等待` -> `设置浏览器窗口大小` -> `初始化 JavaScript 条件 API` -> `浏览器在当前页面 执行 JavaScript 条件` 
-> `从 条件API 中获取结果` -> `强制等待` -> `JavaScript 条件 API 回调`  -> `从 JavaScriptConditionResult 获取是否执行 预定(通知)任务`

`若执行预定(通知)任务` -> `网页截图` -> `执行通知`

`若通知成功` -> `从 Enable 中获取是否禁用此任务 (防止条件后续依旧成立, 而导致重复提醒)`



# 例子


## 百度贴吧自动签到并通知 (使用 Cookie 免账号密码登录) ( 不建议, 目前 Cookie 还存在一些问题 )

> 首先在本机上登录百度，F12 获取 名为 `BDUSS` 的 Cookie, 这个就是用于维持百度登录状态的关键

> 设置 `Cookies` 如下

```
name=BDUSS;value=替换为你的BDUSS;domain=.baidu.com;path=/
```

> 注意: Cookie 如需添加多个, 则一行一个

> 设置目标 Url 为 `https://tieba.baidu.com/f?kw=c%23&ie=utf-8`

> 设置 强制等待为 5,     
> 因为我们需要在 JavaScript 条件中，设置 Cookie, 并刷新网页，需要的是登录后的网页，因此没有必要在这里耗费多余时间



> 设置 `JavaScript 条件` 如下:

```javascript
// 此时就处于登录状态了
// 贴吧签到
// 检测是否已经签到
var canSignDom = document.querySelector(".j_signbtn.sign_btn_bright.j_cansign");
if (canSignDom) {
    canSignDom.click();

    // 签到成功, 执行预定通知任务
    localStorage.setItem("WebMonitorPlugin.JavaScriptConditionResult", true);
} else {
    // 已经签到
    localStorage.setItem("WebMonitorPlugin.JavaScriptConditionResult", false);
}


// 标记为 true, 下次依旧执行
localStorage.setItem("WebMonitorPlugin.Enable", true);
```




> 设置 `执行 JavaScript 条件 后 强制等待` 为 5

> 保存

> 效果图

> 暂无


## B站 动态更新 提醒 (强烈推荐)

> 目标 Url:   
> https://space.bilibili.com/25057459/dynamic

> 浏览器-宽: 1280

> 强制等待: 5

> JavaScript 条件

```javascript
// localStorage.setItem("WebMonitorPlugin.JavaScriptConditionResult", false); 
// 通过此方法来确定本次任务 是否达成条件, 默认 false 
// 下面写你的业务逻辑

// 始终启用
localStorage.setItem("WebMonitorPlugin.Enable", true);

var coententDom = document.querySelector(".content");
var firstDynamicDom = null;

for (var i = 0; i < coententDom.childNodes.length; i++) {
    var node = coententDom.childNodes[i];
    if (node.nodeName == "DIV" && node.className != "feed-title" && node.className != "first-card-with-title") {
        // 除了置顶的第一条动态
        firstDynamicDom = node;
        break;
    }
}

var oldFirstDynamicText = localStorage.getItem("WebMonitorPlugin.Storage.FirstDynamicText");

var contentDom = firstDynamicDom.querySelector(".content-full") || firstDynamicDom.querySelector(".card-content .post-content .content");
// 注意: 当没有获取到时, 就使用 旧数据
// 有时候可能会由于加载原因, 而导致没有获取到数据
var newFirstDynamicText = contentDom?.innerText ?? oldFirstDynamicText;


if (oldFirstDynamicText != newFirstDynamicText) {
    // 动态有更新
    localStorage.setItem("WebMonitorPlugin.JavaScriptConditionResult", true);    
} else {
    localStorage.setItem("WebMonitorPlugin.JavaScriptConditionResult", false);
}


// 保存最新的动态数据
localStorage.setItem("WebMonitorPlugin.Storage.FirstDynamicText", newFirstDynamicText);
```


## 相关

> 项目地址: [https://github.com/yiyungent/KnifeHub/tree/main/plugins/WebMonitorPlugin](https://github.com/yiyungent/KnifeHub/tree/main/plugins/WebMonitorPlugin)             

<!-- Matomo Image Tracker-->
<img referrerpolicy="no-referrer-when-downgrade" src="https://matomo.moeci.com/matomo.php?idsite=2&amp;rec=1&amp;action_name=Plugins.WebMonitorPlugin-v0.4.5.README" style="border:0" alt="" />
<!-- End Matomo -->