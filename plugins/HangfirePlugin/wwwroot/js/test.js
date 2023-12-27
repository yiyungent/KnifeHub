(function () {
    window.memosPlusBaseUrl = "http://localhost:5193";
    var memosPlus = document.createElement("script");
    memosPlus.src = `${memosPlusBaseUrl}/plugins/MemosPlus/js/insert.js`;
    var s = document.getElementsByTagName("script")[0];
    s.parentNode.insertBefore(memosPlus, s);
})();