


## 功能

- 备份 `memos` 内容到 `GitHub` 仓库 指定文件
  - `纯文本内容 (包括 markdown)`
  - `二进制文件`





## 使用


## 嵌入 memos 默认前端

> `系统` -> `自定义脚本` 

```javascript
var _hmt = _hmt || [];
(function () {
    var hm = document.createElement("script");
    hm.src = "https://cdn.jsdelivr.net/npm/@yiyungent/plugincore/dist/PluginCore.min.js";
    var s = document.getElementsByTagName("script")[0];
    s.parentNode.insertBefore(hm, s);
})();


function startPluginCore() {
  // let oldHtml = document.querySelector(".banner-wrapper").innerHTML;

  // document.querySelector(".banner-wrapper").innerHTML = oldHtml + "<!-- PluginCore.IPlugins.IWidgetPlugin.Widget(memos,0.11.0,banner-wrapper) -->"

  window.memosPluginCore = new PluginCore({
    baseUrl: "https://api-onetree.moeci.com"
  });

  memosPluginCore.start();
}

var global = global || window;
window.addEventListener("load", () => {
  startPluginCore();
});

window.onload = function () {

}
```


### 设置 `GitHub`

1. 前往此处获取 `GitHub Personal Access Token`, [点我前往](https://github.com/settings/tokens/new) , 注意: 一定要勾选 `repo Full control of private repositories`



> `RepoTargetFilePath` 为笔记写入文件路径 (相对于此仓库根目录), 此文件路径需先存在, 例如: `source/_posts/分类-杂记/杂记-来自QQ.md`




<!-- Matomo Image Tracker-->
<img referrerpolicy="no-referrer-when-downgrade" src="https://matomo.moeci.com/matomo.php?idsite=2&amp;rec=1&amp;action_name=Plugins.MemosPlusPlugin-v0.1.0.README" style="border:0" alt="" />
<!-- End Matomo -->

## 临时


https://memos.moeci.com/o/r/1/500440a7-04c2-4feb-a76b-e7fada60e7c7.jpg


https://memos.moeci.com/api/memo?rowStatus=NORMAL&offset=20&limit=20


