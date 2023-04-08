using System.Web;

namespace ITodaysPlus.Utils
{
    public class ITodaysUtil
    {
        public string BaseUrl { get; set; }

        public ITodaysUtil(string baseUrl = "https://api.timefriend.vip/ApiV2")
        {
            this.BaseUrl = baseUrl.Trim().TrimEnd('/');
        }

        public LoginResponseModel Login(string userName, string password, string tmp_token = "wSEjf_aUHWu89273")
        {
            LoginResponseModel responseModel = new LoginResponseModel();
            try
            {
                string platform = HttpUtility.UrlEncode("1", System.Text.Encoding.UTF8);
                string device_name = HttpUtility.UrlEncode("Samsung SM-G973N", System.Text.Encoding.UTF8);
                string device_id = HttpUtility.UrlEncode("351564315013821", System.Text.Encoding.UTF8);
                userName = HttpUtility.UrlEncode(userName, System.Text.Encoding.UTF8);
                password = HttpUtility.UrlEncode(password, System.Text.Encoding.UTF8);
                tmp_token = HttpUtility.UrlEncode(tmp_token, System.Text.Encoding.UTF8);
                string url = this.BaseUrl + "/login";
                string postDataStr = $"platform={platform}&device_name={device_name}&password={password}&userName={userName}&device_id={device_id}&tmp_token={tmp_token}";
                string jsonStr = HttpUtil.HttpPost(url: url,
                                          postDataStr: postDataStr,
                                          headers: new string[] {
                                              "Host: api.timefriend.vip",
                                              "Content-Type: application/x-www-form-urlencoded",
                                              "User-Agent: Dalvik/2.1.0 (Linux; U; Android 7.1.2; SM-G973N Build/PPR1.190810.011)",
                                              "Connection: Keep-Alive",
                                              "Accept-Encoding: gzip",
                                              $"Content-Length: {postDataStr.Length}"
                                          });
                responseModel = System.Text.Json.JsonSerializer.Deserialize<LoginResponseModel>(jsonStr);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return responseModel;
        }

        public (DateRecordResponseModel ResponseModel, DateRecordResponseModel.dataModel DataModel) GetDateRecord(string token, DateTime date)
        {
            (DateRecordResponseModel responseModel, DateRecordResponseModel.dataModel dataModel) resultModel = (new DateRecordResponseModel(), null);
            try
            {
                string data = "{\"activity_flag\":112207048,\"time\":\"{{0}}\"}".Replace("{{0}}", date.ToString("yyyy-MM-dd 00:00:00"));
                token = HttpUtility.UrlEncode(token, System.Text.Encoding.UTF8);
                data = HttpUtility.UrlEncode(data, System.Text.Encoding.UTF8);
                string url = this.BaseUrl + "/getDateRecord";
                string postDataStr = $"token={token}&data={data}";
                string jsonStr = HttpUtil.HttpPost(url: url,
                                          postDataStr: postDataStr,
                                          headers: new string[] {
                                              "Host: api.timefriend.vip",
                                              "Content-Type: application/x-www-form-urlencoded",
                                              "User-Agent: Dalvik/2.1.0 (Linux; U; Android 7.1.2; SM-G973N Build/PPR1.190810.011)",
                                              "Connection: Keep-Alive",
                                              "Accept-Encoding: gzip",
                                              $"Content-Length: {postDataStr.Length}"
                                          });
                resultModel.responseModel = System.Text.Json.JsonSerializer.Deserialize<DateRecordResponseModel>(jsonStr);
                if (resultModel.responseModel.status == 1)
                {
                    try
                    {
                        resultModel.dataModel = System.Text.Json.JsonSerializer.Deserialize<DateRecordResponseModel.dataModel>(resultModel.responseModel.data);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"resultModel.responseModel.data: \n {resultModel.responseModel.data}");
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultModel;
        }

        #region Models

        #region Login
        public class LoginResponseModel
        {
            public string status { get; set; }
            public string msg { get; set; }
            public int uid { get; set; }
            public object nick { get; set; }
            public string userName { get; set; }
            public string password { get; set; }
            public UserModel[] user { get; set; }
            public string token { get; set; }
            public GoalModel[] goals { get; set; }
            public GoalstaticModel[] GoalStatics { get; set; }
            public object[] items { get; set; }
            public LabelModel[] labels { get; set; }
            public object[] labelLinks { get; set; }
            public object[] allocations { get; set; }
            public class UserModel
            {
                public int id { get; set; }
                public string userName { get; set; }
                public string password { get; set; }
                public object nick { get; set; }
                public object authorization { get; set; }
                public object gender { get; set; }
                public object birthday { get; set; }
                public object profession { get; set; }
                public object integral { get; set; }
                public object phone { get; set; }
                public object qq { get; set; }
                public int investment { get; set; }
                public int property { get; set; }
                public object endUpdateTime { get; set; }
                public string createtime { get; set; }
                public int isDelete { get; set; }
                public int itemFrom { get; set; }
            }

            public class GoalModel
            {
                public int id { get; set; }
                public int userId { get; set; }
                public string image { get; set; }
                public string color { get; set; }
                public string goalName { get; set; }
                public int type { get; set; }
                public string startTime { get; set; }
                public string deadline { get; set; }
                public string level { get; set; }
                public int? timeOfEveryday { get; set; }
                public int expectSpend { get; set; }
                public int hadSpend { get; set; }
                public object hadWaste { get; set; }
                public int isFinish { get; set; }
                public int isDelete { get; set; }
                public string finishTime { get; set; }
                public string deleteTime { get; set; }
                public string intruction { get; set; }
                public int position { get; set; }
                public string endUpdateTime { get; set; }
                public int isSubGoal { get; set; }
                public int isManuscript { get; set; }
                public int isDefault { get; set; }
                public int isHided { get; set; }
                public string createTime { get; set; }
                public int resetCount { get; set; }
                public int frequency { get; set; }
                public string serverCreateTime { get; set; }
                public int itemFrom { get; set; }
            }

            public class GoalstaticModel
            {
                public int id { get; set; }
                public int userId { get; set; }
                public int goalId { get; set; }
                public object goalType { get; set; }
                public object expectInvest { get; set; }
                public int? hadInvest { get; set; }
                public object todayInvest { get; set; }
                public object sevenInvest { get; set; }
                public object createTime { get; set; }
                public object startTime { get; set; }
                public object deadline { get; set; }
                public object endUpdateTime { get; set; }
                public int staticsType { get; set; }
            }

            public class LabelModel
            {
                public int id { get; set; }
                public int userId { get; set; }
                public int goalType { get; set; }
                public int goalId { get; set; }
                public int labelType { get; set; }
                public string name { get; set; }
                public object describe { get; set; }
                public string lastUseTime { get; set; }
                public int isDelete { get; set; }
                public string createTime { get; set; }
                public int labelColor { get; set; }
                public int labelPosition { get; set; }
                public string endUpdateTime { get; set; }
                public string deleteTime { get; set; }
                public string serverCreateTime { get; set; }
                public int itemFrom { get; set; }
            }
        }
        #endregion

        #region DateRecord
        public class DateRecordResponseModel
        {
            public int status { get; set; }

            public string info { get; set; }

            /// <summary>
            /// 注意: 不能直接使用 dataModel
            /// 其中是 json 字符串 含转义符 \
            /// </summary>
            public string data { get; set; }

            public class dataModel
            {
                public ItemModel[] items { get; set; }

                public class ItemModel
                {
                    public int id { get; set; }
                    public int userId { get; set; }
                    public int goalId { get; set; }
                    public int goalType { get; set; }
                    public string startTime { get; set; }
                    public int take { get; set; }
                    public string stopTime { get; set; }
                    public int isEnd { get; set; }
                    public int isRecord { get; set; }
                    public string remarks { get; set; }
                    public int isDelete { get; set; }
                    public object deleteTime { get; set; }
                    public string endUpdateTime { get; set; }
                    public int isMaster { get; set; }
                    public object createTime { get; set; }
                    public string serverCreateTime { get; set; }
                    public int itemFrom { get; set; }
                }
            }
        }
        #endregion

        #endregion
    }
}
