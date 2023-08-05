//using Newtonsoft.Json;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace TgBotStatPlugin.Utils
{
    public class JsonUtil
    {

        #region JsonStr2Obj
        public static T JsonStr2Obj<T>(string jsonStr)
        {
            //return JsonConvert.DeserializeObject<T>(jsonStr);
            return JsonSerializer.Deserialize
                <T>(jsonStr);
        }
        #endregion

        #region Obj2JsonStr
        public static string Obj2JsonStr(object jsonObj)
        {
            //return JsonConvert.SerializeObject(jsonObj);
            return JsonSerializer.Serialize(jsonObj,
               new JsonSerializerOptions
               {
                   WriteIndented = true,
                   Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
               });
        }
        #endregion

    }
}
