using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoLi4QQChannelPlugin.Utils
{
    public class MoLiApiUtil
    {
        public static MoLiApiResponseModel Reply(MoLiApiRequestModel requestModel)
        {
            MoLiApiResponseModel responseModel = new MoLiApiResponseModel();
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(MoLi4QQChannelPlugin));

            string url = "https://i.mly.app/reply";
            string reqJsonStr = JsonUtil.Obj2JsonStr(requestModel);
            string[] headers = new string[] {
                $"Api-Key: {settingsModel.ApiKey}",
                $"Api-Secret: {settingsModel.ApiSecret}",
                $"Content-Type: application/json"
            };
            string resJsonStr = "";
            try
            {
                resJsonStr = HttpUtil.HttpPost(url: url, postDataStr: reqJsonStr, headers: headers);
            }
            catch (Exception ex)
            {
                Console.WriteLine("调用 茉莉 API 失败:");
                Console.WriteLine(ex.ToString());
            }
            try
            {
                #region 还是失败
                // System.Text.Json.JsonException: The JSON value could not be converted to System.String. Path: $.data[0].typed | LineNumber: 0 | BytePositionInLine: 136.
                //--->System.InvalidOperationException: Cannot get the value of a token type 'Number' as a string.
                //at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.ReadJsonAndSetMember(Object obj, ReadStack & state, Utf8JsonReader & reader)
                //var options = new JsonSerializerOptions
                //{
                //    NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
                //};
                //responseModel = JsonSerializer.Deserialize<MoLiApiResponseModel>(resJsonStr, options); 
                #endregion

                //responseModel = JsonUtil.JsonStr2Obj<MoLiApiResponseModel>(resJsonStr);

                Console.WriteLine("茉莉机器人 API 响应:");
                Console.WriteLine(resJsonStr);

                responseModel = Newtonsoft.Json.JsonConvert.DeserializeObject<MoLiApiResponseModel>(resJsonStr);
            }
            catch (Exception ex)
            {
                Console.WriteLine("茉莉 API 响应转为实体 失败:");
                Console.WriteLine(ex.ToString());
            }

            return responseModel;
        }
    }

    public class MoLiApiRequestModel
    {
        public string content { get; set; }

        public string type { get; set; }

        public string from { get; set; }

        public string fromName { get; set; }

        public string to { get; set; }

        public string toName { get; set; }

    }


    public class MoLiApiResponseModel
    {
        public string code { get; set; }

        public string message { get; set; }

        public string plugin { get; set; }

        public List<DataItemModel> data { get; set; }

        public class DataItemModel
        {
            public string content { get; set; }

            public string typed { get; set; }

            public string remark { get; set; }
        }
    }

}
