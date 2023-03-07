
## 功能


- [x] 注入 `memos` 默认前端
  - [ ] 具体功能待添加 
- [x] 备份 `memos` 内容到 `GitHub` 仓库 指定文件夹
  - [x] 纯文本内容 (包括 `markdown`)
  - [ ] 资源库 `二进制文件`

## 使用

## 1. 嵌入 memos 默认前端

> `系统` -> `自定义脚本` 

> **重要** : 将 `https://qqbothub.moeci.com` 替换为你的 url, 末尾不要带 `/`

```javascript
(function () {
    window.memosPlusBaseUrl = "https://qqbothub.moeci.com";
    var memosPlus = document.createElement("script");
    memosPlus.src = `${memosPlusBaseUrl}/plugins/MemosPlusPlugin/js/insert.js`;
    var s = document.getElementsByTagName("script")[0];
    s.parentNode.insertBefore(memosPlus, s);
})();
```


### 2. 设置 `GitHub`

> 前往此处获取 `GitHub Personal Access Token`, [点我前往](https://github.com/settings/tokens/new) ,       
> 注意: 一定要勾选 `repo Full control of private repositories`

> `RepoTargetDirPath` 为内容写入文件夹路径 (相对于此仓库根目录),      
> 例如: `source/_posts/分类-memos/`




<!-- Matomo Image Tracker-->
<img referrerpolicy="no-referrer-when-downgrade" src="https://matomo.moeci.com/matomo.php?idsite=2&amp;rec=1&amp;action_name=Plugins.MemosPlus-v0.1.1.README" style="border:0" alt="" />
<!-- End Matomo -->



<!-- ## 临时 -->


<!-- https://memos.moeci.com/o/r/1/500440a7-04c2-4feb-a76b-e7fada60e7c7.jpg -->


<!-- https://memos.moeci.com/api/memo?rowStatus=NORMAL&offset=20&limit=20 -->


