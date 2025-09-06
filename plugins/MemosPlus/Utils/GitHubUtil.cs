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

            // 检查新内容是否为空或无效
            if (string.IsNullOrEmpty(fileContent))
            {
                System.Console.WriteLine("GitHubUtil.CreateFile: 文件内容为空, 放弃提交");
                return;
            }

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
                System.Console.WriteLine("GitHubUtil.CreateFile: 创建文件时发生异常");
                System.Console.WriteLine($"异常信息: {ex.Message}");
                throw; // 重新抛出异常，让调用者处理
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

            // 检查新内容是否为空或无效
            if (string.IsNullOrEmpty(fileContent))
            {
                System.Console.WriteLine("GitHubUtil.UpdateFile: 文件内容为空, 放弃提交");
                return;
            }

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

                    // 比较文件内容，如果内容相同则跳过提交
                    if (oldFileContent == fileContent)
                    {
                        System.Console.WriteLine("GitHubUtil.UpdateFile: 文件内容未变, 放弃提交");
                        return;
                    }

                    // update the file
                    System.Console.WriteLine("GitHubUtil.UpdateFile: 发生改变, 更新文件");
                    var updateChangeSet = gitHubClient.Repository.Content.UpdateFile(owner, repo, targetFilePath,
                   new UpdateFileRequest(message: $"{nameof(MemosPlus)} UpdateFile {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} \n\n{repoTargetFilePath}",
                   content: fileContent, sha: existingFile.First().Sha, branch: branch, convertContentToBase64: convertContentToBase64))
                    .Result;
                }
                else
                {
                    // 文件不存在，创建新文件
                    System.Console.WriteLine("GitHubUtil.UpdateFile: 文件不存在，创建新文件");
                    var createChangeSet = gitHubClient.Repository.Content.CreateFile(owner, repo, targetFilePath,
                    new CreateFileRequest(message: $"{nameof(MemosPlus)} CreateFile {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} \n\n{repoTargetFilePath}",
                   content: fileContent, branch: branch, convertContentToBase64: convertContentToBase64)).Result;
                }
            }
            //catch (Octokit.NotFoundException)
            catch (Exception ex)
            {
                // 只有在文件确实不存在时才创建，避免重复创建
                if (ex is Octokit.NotFoundException
                    || ex is System.AggregateException ae
                    && ae != null && ae.InnerException != null
                    && ae.InnerException is Octokit.NotFoundException
                    )
                {
                    System.Console.WriteLine("GitHubUtil.UpdateFile: 文件不存在(NotFoundException)，创建新文件");
                    var createChangeSet = gitHubClient.Repository.Content.CreateFile(owner, repo, targetFilePath,
                    new CreateFileRequest(message: $"{nameof(MemosPlus)} CreateFile {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} \n\n{repoTargetFilePath}",
                   content: fileContent, branch: branch, convertContentToBase64: convertContentToBase64)).Result;
                }
                else
                {
                    System.Console.WriteLine("GitHubUtil.UpdateFile: 发生异常，放弃提交");
                    System.Console.WriteLine($"异常信息: {ex.Message}");
                    throw; // 重新抛出非NotFoundException的异常
                }
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
