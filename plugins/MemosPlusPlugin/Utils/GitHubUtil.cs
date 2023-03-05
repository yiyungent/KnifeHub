using Octokit;

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
        public void CreateFile(string repoOwner, string repoName, string repoBranch, string repoTargetFilePath, string fileContent, string accessToken,bool convertContentToBase64 = true)
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
                var createChangeSet = gitHubClient.Repository.Content.CreateFile(owner, repo, targetFilePath,
                   new CreateFileRequest(message: $"{nameof(MemosPlusPlugin)} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}",
                   content: fileContent, branch: branch, convertContentToBase64: convertContentToBase64))
                    .Result;
            }
            catch (Exception ex)
            {

            }
            #endregion

            #endregion
        }

        public void CreateFile(string repoOwner, string repoName, string repoBranch, string repoTargetFilePath, byte[] fileContent, string accessToken)
        {
            CreateFile(repoOwner: repoOwner, repoName: repoName, repoBranch: repoBranch, repoTargetFilePath: repoTargetFilePath,
            fileContent: Convert.ToBase64String(fileContent), accessToken: accessToken, convertContentToBase64: false);
        }

        /// <summary>
        /// 若不存在此文件, 则进行创建文件
        /// </summary>
        public void UpdateFile(string repoOwner, string repoName, string repoBranch, string repoTargetFilePath, string fileContent, string accessToken, bool convertContentToBase64 = true)
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
            // try to get the file (and with the file the last commit sha)
            bool existFile = false;
            try
            {
                var existingFile = gitHubClient.Repository.Content.GetAllContentsByRef(owner, repo, targetFilePath, branch).Result;
                if (existingFile != null && existingFile.Count >= 1)
                {
                    existFile = true;
                    // string oldFileContent = existingFile.First().Content;
                    // update the file
                    var updateChangeSet = gitHubClient.Repository.Content.UpdateFile(owner, repo, targetFilePath,
                   new UpdateFileRequest(message: $"{nameof(MemosPlusPlugin)} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}",
                   content: fileContent, sha: existingFile.First().Sha, branch: branch, convertContentToBase64: convertContentToBase64))
                    .Result;
                }
                else
                {
                    // if file is not found, create it
                    var createChangeSet = gitHubClient.Repository.Content.CreateFile(owner, repo, targetFilePath,
                    new CreateFileRequest(message: $"{nameof(MemosPlusPlugin)} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}",
                   content: fileContent, branch: branch, convertContentToBase64: convertContentToBase64)).Result;
                }
            }
            //catch (Octokit.NotFoundException)
            catch (Exception ex)
            {
                // if file is not found, create it
                var createChangeSet = gitHubClient.Repository.Content.CreateFile(owner, repo, targetFilePath,
                new CreateFileRequest(message: $"{nameof(MemosPlusPlugin)} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}",
               content: fileContent, branch: branch, convertContentToBase64: convertContentToBase64)).Result;
            }
            #endregion

            #endregion
        }

        public void UpdateFile(string repoOwner, string repoName, string repoBranch, string repoTargetFilePath, byte[] fileContent, string accessToken)
        {
            UpdateFile(repoOwner: repoOwner, repoName: repoName, repoBranch: repoBranch, repoTargetFilePath: repoTargetFilePath,
            fileContent: Convert.ToBase64String(fileContent), accessToken: accessToken, convertContentToBase64: false);
        }

        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <returns></returns>
        public string ReadFile()
        {
            string fileContent = "";

            return fileContent;
        }

    }
}