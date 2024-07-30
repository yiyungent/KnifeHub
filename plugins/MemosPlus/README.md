

## 简介

> 提升 memos 使用体验, 更快捷的 memos 自动备份/导出



> 当前适配 [memos](https://usememos.com/) v0.11.0


## 功能


- [x] 注入 `memos` 默认前端
  - [ ] 具体功能待添加 
- [x] 定时同步 `memos` 内容到 `GitHub` 仓库 指定文件夹
  - [x] 纯文本内容 (包含 `markdown`)
    - [x] 自定义模版 (默认适配 `Hexo post` 风格, 配合 `GitHub Actions` 即可实现 `memos` 同步发布到 `Hexo`)  
  - [x] 资源库 (`二进制文件`)

## 使用

### 1. 嵌入 memos 默认前端

> `设置` -> `系统` -> `自定义脚本` -> `复制粘贴` -> `保存`

> **重要** : 将 `https://qqbothub.moeci.com` 替换为你的 url, 末尾不要带 `/`     
> `memosPlusBaseUrl` 为部署 MemosPlus 的 URL, 而不是部署 memos 的 URL

- [右键复制这个链接地址, 一般此为 memosPlusBaseUrl](/)

```javascript
(function () {
    window.memosPlusBaseUrl = "https://qqbothub.moeci.com";
    var memosPlus = document.createElement("script");
    memosPlus.src = `${memosPlusBaseUrl}/plugins/MemosPlus/js/insert.js`;
    var s = document.getElementsByTagName("script")[0];
    s.parentNode.insertBefore(memosPlus, s);
})();
```

> 将以上内容复制粘贴到 `自定义脚本` 中, 点击 `保存`


### 2. 设置 `GitHub`

> 前往此处获取 `GitHub Personal Access Token`, [点我前往](https://github.com/settings/tokens/new) ,       
> 注意: 一定要勾选 `repo Full control of private repositories`

> `RepoTargetDirPath` 为内容写入文件夹路径 (相对于此仓库根目录),      
> 例如: `source/_posts/分类-memos/`



### 其它

> `设置` 中 `SecondsPeriod` 为 `定时周期`: 单位为 `秒`




<!-- Matomo Image Tracker-->
<img referrerpolicy="no-referrer-when-downgrade" src="https://matomo.moeci.com/matomo.php?idsite=2&amp;rec=1&amp;action_name=Plugins.MemosPlus-v0.1.17.README" style="border:0" alt="" />
<!-- End Matomo -->








<!-- ## 临时 -->


<!-- https://memos.moeci.com/o/r/1/500440a7-04c2-4feb-a76b-e7fada60e7c7.jpg -->


<!-- https://memos.moeci.com/api/memo?rowStatus=NORMAL&offset=20&limit=20 -->



