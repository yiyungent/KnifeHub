namespace MemosPlusPlugin.Utils
{
    public class MemosUtil
    {
        public string MemosBaseUrl { get; set; }

        public MemosUtil(string memosBaseUrl)
        {
            this.MemosBaseUrl = memosBaseUrl.Trim().TrimEnd('/');
        }

        public List<MemoItemModel> List(string memosSession, string rowStatus = "NORMAL", int offset = 0, int limit = 20)
        {
            List<MemoItemModel> rtn = new List<MemoItemModel>();
            try
            {
                string resJson = HttpUtil.HttpGet(url: $"{MemosBaseUrl}/api/memo?rowStatus={rowStatus}&offset={offset}0&limit={limit}",
                headers: new string[] { $"cookie: memos_session={memosSession}" });
                var resModel = System.Text.Json.JsonSerializer.Deserialize<MemosListResponseModel>(resJson);
                if (resModel != null && resModel.data.Count >= 1)
                {
                    rtn = resModel.data;
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }

            return rtn;
        }

        public string ResourceLink(string resourceId, string fileName)
        {
            return $"{MemosBaseUrl}/o/r/{resourceId}/{fileName}";
        }

        public byte[] Resource(string resourceId, string fileName, string memosSession)
        {
            List<byte> rtn = new List<byte>();
            string url = ResourceLink(resourceId, fileName);
            try
            {
                var temp = HttpUtil.HttpGetFile(url, cookie: $"memos_session={memosSession}");
                if (temp != null && temp.Length >= 1)
                {
                    return temp;
                }
            }
            catch (System.Exception ex)
            {

            }

            return rtn.ToArray();
        }
    }

    public class MemosListResponseModel
    {
        public List<MemoItemModel> data { get; set; }
    }

    public class MemoItemModel
    {
        /// <summary>
        /// 
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// NORMAL
        /// </summary>
        public string rowStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long creatorId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long createdTs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long updatedTs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string visibility { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool pinned { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string creatorName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ResourceItemModel> resourceList { get; set; }
    }

    public class ResourceItemModel
    {
        /// <summary>
        /// 
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long creatorId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long createdTs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long updatedTs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string filename { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string externalLink { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long size { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long linkedMemoAmount { get; set; }
    }



}