namespace KonataPlugin.ResponseModels
{
    public class InfoResponseDataModel
    {
        public bool IsOnline { get; set; }

        public string CaptchaType { get; set; }

        /// <summary>
        /// 滑动验证: url
        /// 短信验证: 手机号
        /// </summary>
        public string CaptchaTip { get; set; }

        public string CaptchaUpdateTime { get; set; }
    }
}
