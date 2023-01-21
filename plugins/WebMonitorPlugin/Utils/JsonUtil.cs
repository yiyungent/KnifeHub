using Newtonsoft.Json;

namespace WebMonitorPlugin.Utils
{
    public class JsonUtil
    {

        #region JsonStr2Obj
        public static T JsonStr2Obj<T>(string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }
        #endregion

        #region Obj2JsonStr
        public static string Obj2JsonStr(object jsonObj)
        {
            return JsonConvert.SerializeObject(jsonObj);
        }
        #endregion

        #region 格式化JSON字符串
        public static string ConvertJsonString(string str)
        {
            //格式化json字符串
            #region 使用Newtonsoft.Json格式化 JSON字符串
            //Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            //TextReader tr = new StringReader(str);
            //Newtonsoft.Json.JsonTextReader jtr = new Newtonsoft.Json.JsonTextReader(tr);
            //object obj = serializer.Deserialize(jtr);
            //if (obj != null)
            //{
            //    StringWriter textWriter = new StringWriter();
            //    Newtonsoft.Json.JsonTextWriter jsonWriter = new Newtonsoft.Json.JsonTextWriter(textWriter)
            //    {
            //        Formatting = Newtonsoft.Json.Formatting.Indented,
            //        Indentation = 4,
            //        IndentChar = ' '
            //    };
            //    serializer.Serialize(jsonWriter, obj);
            //    return textWriter.ToString();
            //}
            //else
            //{
            //    return str;
            //} 
            #endregion

            #region 使用 System.Text.Json 格式化 JSON字符串
            // https://blog.csdn.net/essity/article/details/84644510
            System.Text.Json.JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions();
            // 设置支持中文的unicode编码: 这样就不会自动转码，而是原样展现
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            // 启用缩进设置
            options.WriteIndented = true;

            // 注意: object 不会丢失json数据, 但不能使用 dynamic, 会报编译错误
            object jsonObj = System.Text.Json.JsonSerializer.Deserialize<object>(str);

            // Error CS0656  Missing compiler required member 'Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create' 
            // dynamic jsonObj = System.Text.Json.JsonSerializer.Deserialize<dynamic>(str);

            string rtnStr = System.Text.Json.JsonSerializer.Serialize(jsonObj, options);

            return rtnStr;
            #endregion
        }
        #endregion

    }
}
