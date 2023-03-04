namespace MemosPlusPlugin.Utils
{
    public class GitHubUtil
    {
        /// <summary>
        /// 若文件已经存在, 则进行覆盖操作
        /// </summary>
        /// <param name="repoOwner"></param>
        /// <param name="repoName"></param>
        /// <param name="repoBranch"></param>
        /// <param name="repoTargetFilePath"></param>
        /// <param name="fileContent"></param>
        /// <param name="accessToken"></param>
        public void CreateFile(string repoOwner, string repoName, string repoBranch, string repoTargetFilePath, string fileContent, string accessToken)
        {
            #region GitHub
            GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue(nameof(MemosPlusPlugin)));
            gitHubClient.Credentials = new Credentials(accessToken);

            // github variables
            string owner = repoOwner;
            string repo = repoName;
            string branch = repoBranch;
            string targetFilePath = repoTargetFilePath;

            #region 写入文件
            try
            {
                var createFileSet = gitHubClient.Repository.Content.CreateFile(owner, repo, targetFilePath,
                   new CreateFileRequest(message: $"{nameof(MemosPlusPlugin)} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}",
                   content: fileContent, branch: branch, convertContentToBase64: false))
                    .Result;
            }
            //catch (Octokit.NotFoundException)
            catch (Exception ex)
            {
                // if file is not found, create it
                //var createChangeSet = gitHubClient.Repository.Content.CreateFile(owner, repo, targetFilePath, new CreateFileRequest("API File creation", "Hello Universe! " + DateTime.UtcNow, branch)).Result;

            }
            #endregion

            #endregion
        }

        public void CreateFile(string repoOwner, string repoName, string repoBranch, string repoTargetFilePath, byte[] fileContent, string accessToken)
        {
            return CreateFile(repoOwner: repoOwner, repoName: repoName, repoBranch: repoBranch, repoTargetFilePath: repoTargetFilePath,
            fileContent: Convert.ToBase64String(fileContent), accessToken: accessToken);
        }

        /// <summary>
        /// 若不存在此文件, 则进行创建文件
        /// </summary>
        public void UpdateFile() 
        {

        }

        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <returns></returns>
        public string ReadFile() 
        {
            
        }

    }
}