// start: MemosPlusPlugin
window.plugincoreJsLoaded = false;
(function () {
    var plugincoreJs = document.createElement("script");
    plugincoreJs.src = "https://cdn.jsdelivr.net/npm/@yiyungent/plugincore/dist/PluginCore.min.js";
    var s = document.getElementsByTagName("script")[0];
    s.parentNode.insertBefore(plugincoreJs, s);
    plugincoreJs.onload = () => {
        window.plugincoreJsLoaded = true;
    }
})();

function startPluginCore() {
    window.memosPluginCore = new PluginCore({
        baseUrl: window.memosPlusBaseUrl
    });
    
    window.memosPluginCore.start();
}

let memosBannerNode = false;
let memosBannerNodeWidgetInserted = false;
let memosPluginCoreTimer = null;
function insertPluginCoreWidget() {
    memosBannerNode = document.querySelector(".banner-wrapper");
    if (memosBannerNode && !memosBannerNodeWidgetInserted) {
        memosBannerNode.innerHTML = memosBannerNode.innerHTML + "<!-- PluginCore.IPlugins.IWidgetPlugin.Widget(memos,0.11.0,banner-wrapper) -->";
        memosBannerNodeWidgetInserted = true;
    } else if (memosBannerNodeWidgetInserted && window.plugincoreJsLoaded) {
        clearInterval(memosPluginCoreTimer);
        startPluginCore();
    }
}

// window.memosNodes = []
// window.memosNodesWidgetInserted = []
// function findNodes(nodeStrArr, callback) {
//     nodeStrArr.forEach(nodeStr => {
//         memosNodes[nodeStr] = document.querySelector(nodeStr);
//         if (node && !memosNodesWidgetInserted[nodeStr]) {
//             memosNodesWidgetInserted[nodeStr] = true;
//         } else if (memosNodesWidgetInserted[nodeStr] && window.plugincoreJsLoaded) {
//             clearInterval(memosPluginCoreTimer);
//             callback(memosNodes[nodeStr]);
//         }
//     });
// }

memosPluginCoreTimer = setInterval(() => {
    insertPluginCoreWidget();
}, 300);

// end: MemosPlusPlugin