using System.Collections.Specialized;
using Octokit;
using PluginCore;

namespace MemosPlus.Utils
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
        public void CreateFile(string repoOwner, string repoName, string repoBranch, string repoTargetFilePath, string fileContent, string accessToken, bool convertContentToBase64 = true)
        {
            #region GitHub
            GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue(nameof(MemosPlus)));
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
                   new CreateFileRequest(message: $"{nameof(MemosPlus)} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}",
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
            GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue(nameof(MemosPlus)));
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
                    // 注意: Content 只有在为纯文本时(非二进制) 才有值, 否则为 null
                    string oldFileContent = null;
                    bool isBinaryFile = false;
                    try
                    {
                        oldFileContent = existingFile.First().Content;
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine("existingFile.First().Content:");
                        System.Console.WriteLine(ex.ToString());
                    }
                    // if (oldFileContent == null)
                    if (string.IsNullOrEmpty(oldFileContent))
                    {
                        // 二进制文件
                        System.Console.WriteLine("GitHubUtil.UpdateFile: 二进制文件");
                        // return;
                        isBinaryFile = true;
                    }
                    var settings = PluginSettingsModelFactory.Create<SettingsModel>(nameof(MemosPlus));
                    string fileExt = Path.GetExtension(settings.GitHub.MemoFileName);
                    if (!existingFile.First().Path.EndsWith(fileExt))
                    {
                        System.Console.WriteLine("GitHubUtil.UpdateFile: 二进制文件");
                        // return;
                        isBinaryFile = true;
                    }
                    if (isBinaryFile)
                    {
                        System.Console.WriteLine("GitHubUtil.UpdateFile: 二进制文件");
                        System.Console.WriteLine("GitHubUtil.UpdateFile: 获取 二进制文件 内容");
                        var temp = gitHubClient.Repository.Content.GetRawContentByRef(owner: owner, name: repo, path: targetFilePath, reference: branch).Result;
                        oldFileContent = Convert.ToBase64String(temp);
                    }
                    if (oldFileContent == fileContent)
                    {
                        System.Console.WriteLine("GitHubUtil.UpdateFile: 文件内容未变, 放弃提交");
                        return;
                    }
                    // update the file
                    var updateChangeSet = gitHubClient.Repository.Content.UpdateFile(owner, repo, targetFilePath,
                   new UpdateFileRequest(message: $"{nameof(MemosPlus)} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}",
                   content: fileContent, sha: existingFile.First().Sha, branch: branch, convertContentToBase64: convertContentToBase64))
                    .Result;
                }
                else
                {
                    // if file is not found, create it
                    var createChangeSet = gitHubClient.Repository.Content.CreateFile(owner, repo, targetFilePath,
                    new CreateFileRequest(message: $"{nameof(MemosPlus)} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}",
                   content: fileContent, branch: branch, convertContentToBase64: convertContentToBase64)).Result;
                }
            }
            //catch (Octokit.NotFoundException)
            catch (Exception ex)
            {
                // if file is not found, create it
                var createChangeSet = gitHubClient.Repository.Content.CreateFile(owner, repo, targetFilePath,
                new CreateFileRequest(message: $"{nameof(MemosPlus)} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}",
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

        public List<string> Files(string repoOwner, string repoName, string repoBranch, string repoTargetDirPath, string accessToken)
        {
            List<string> filePaths = new List<string>();
            GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue(nameof(MemosPlus)));
            gitHubClient.Credentials = new Credentials(accessToken);

            // github variables
            string owner = repoOwner;
            string repo = repoName;
            string branch = repoBranch;
            string targetDirPath = repoTargetDirPath;

            try
            {
                var result = gitHubClient.Repository.Content.GetAllContentsByRef(owner, repo, targetDirPath, branch).Result;
                if (result != null && result.Count >= 1)
                {
                    var list = result.ToList();
                    foreach (var item in list)
                    {
                        if (item.Type.Value == ContentType.Dir)
                        {
                            string dirPath = item.Path;
                            var tempFilePaths = Files(
                                repoOwner: repoOwner,
                                repoName: repoName,
                                repoBranch: repoBranch,
                                repoTargetDirPath: dirPath,
                                accessToken: accessToken
                            );
                            filePaths.AddRange(tempFilePaths);
                        }
                        else if (item.Type.Value == ContentType.File)
                        {
                            string filePath = item.Path;
                            filePaths.Add(filePath);
                        }
                    }
                }

            }
            catch (System.Exception ex)
            {

            }

            return filePaths;
        }

        public void Delete(string repoOwner, string repoName, string repoBranch, string repoTargetFilePath, string accessToken)
        {
            List<string> filePaths = new List<string>();
            GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue(nameof(MemosPlus)));
            gitHubClient.Credentials = new Credentials(accessToken);

            // github variables
            string owner = repoOwner;
            string repo = repoName;
            string branch = repoBranch;
            string targetFilePath = repoTargetFilePath;

            try
            {
                var existingFile = gitHubClient.Repository.Content.GetAllContentsByRef(owner, repo, targetFilePath, branch).Result;
                if (existingFile != null && existingFile.Count >= 1)
                {
                    string sha = existingFile.First().Sha;
                    if (!string.IsNullOrEmpty(sha))
                    {
                        gitHubClient.Repository.Content.DeleteFile(owner, repo, targetFilePath,
                            new DeleteFileRequest(message: $"{nameof(MemosPlus)} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}",
                            sha: sha, branch: branch));
                    }
                }

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

    }
}
