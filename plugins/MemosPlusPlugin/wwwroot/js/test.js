(function () {
    window.memosPlusBaseUrl = "https://api-onetree.moeci.com";
    var memosPlus = document.createElement("script");
    memosPlus.src = `${memosPlusBaseUrl}/plugins/MemosPlusPlugin/js/insert.js`;
    var s = document.getElementsByTagName("script")[0];
    s.parentNode.insertBefore(memosPlus, s);
})();