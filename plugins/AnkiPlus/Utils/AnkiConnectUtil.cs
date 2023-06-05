// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Markdig;
using Markdig.Parsers;
using Markdig.Syntax;
using System.Text.RegularExpressions;
using Markdig.Syntax.Inlines;
using Microsoft.AspNetCore.StaticFiles;

namespace AnkiPlus.Utils
{
    /// <summary>
    /// https://github.com/FooSoft/anki-connect#addnote
    /// </summary>
    public class AnkiConnectUtil
    {
        public SettingsModel Settings
        {
            get
            {
                return PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(AnkiPlus));
            }
        }

        public async Task AddNoteAsync(string deckName, string modelName, string front, string back)
        {
            var settings = this.Settings;

            var client = new HttpClient();
            client.BaseAddress = new Uri(settings.AnkiConnect.BaseUrl);

            var note = new JObject();
            note["deckName"] = deckName;
            note["modelName"] = modelName;
            note["fields"] = new JObject();
            note["fields"][settings.AnkiConnect.Note.Fields.Front] = front;
            note["fields"][settings.AnkiConnect.Note.Fields.Back] = back;

            #region options
            note["options"] = new JObject();
            note["options"]["allowDuplicate"] = false;
            note["options"]["duplicateScope"] = "deck";
            note["options"]["duplicateScopeOptions"] = new JObject();
            //note["options"]["duplicateScopeOptions"]["deckName"] = "Default";
            note["options"]["duplicateScopeOptions"]["checkChildren"] = false;
            note["options"]["duplicateScopeOptions"]["checkAllModels"] = false;
            #endregion

            //note["tags"] = new JArray("tag1", "tag2");
            var addNoteRequest = new JObject();
            addNoteRequest["action"] = "addNote";
            // TODO: 目前固定版本更合适, 毕竟版本改变, 可能导致代码也需要更新
            addNoteRequest["version"] = 6;
            addNoteRequest["params"] = note;

            var response = await client.PostAsync("", new StringContent(addNoteRequest.ToString()));
            var responseJson = JArray.Parse(response.Content.ReadAsStringAsync().Result);
            Console.WriteLine(responseJson.ToString());
        }

        public async Task ConvertMd2Notes(string mdFilePath)
        {
            if (File.Exists(mdFilePath))
            {
                var settings = this.Settings;

                string content = await File.ReadAllTextAsync(mdFilePath);

                // Split the YAML and Markdown content.
                #region Temp
                //var parts = content.Split(new string[] { "---" }, StringSplitOptions.RemoveEmptyEntries);
                //content.IndexOf();
                //if (parts.Length < 2)
                //{
                //    throw new Exception("Invalid Markdown file format.");
                //}
                //string yamlContent = parts[0].Trim();
                //string mdContent = "";
                //for (int i = 1; i < parts.Length; i++)
                //{
                //    mdContent = mdContent + parts[i];
                //}
                //mdContent = mdContent.Trim();
                //var yamlInput = new StringReader(yamlContent);
                //var yamlDeserializer = new YamlDotNet.Serialization.DeserializerBuilder().Build();
                //var yamlMetadata = yamlDeserializer.Deserialize<dynamic>(yamlInput);
                //string mdTitle = yamlMetadata["title"].ToString(); 
                #endregion

                string mdContent = content;

                List<MarkdownNode> nodes = new List<MarkdownNode>();
                MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                                            .UseAdvancedExtensions()
                                            .EnableTrackTrivia()
                                            //.DisableHeadings()
                                            .Build();
                var document = MarkdownParser.Parse(mdContent, pipeline);

                // 将其中 ![]() 图片路径 解析, 并替换为 base64 格式
                foreach (LinkInline link in document.Descendants<LinkInline>())
                {
                    if (link.IsImage)
                    {
                        link.Url = await GetBase64ImageAsync(link.Url, mdFilePath: mdFilePath);
                    }
                }

                ParseNodes(document, nodes, 1);

                //nodes.Reverse();
                //int level = -1;
                //Queue<string> deckNameQueue = new Queue<string>();
                foreach (var node in nodes)
                {
                    #region Temp
                    //// TODO: 层级顺序
                    //if (node.Level > level)
                    //{
                    //    // 本次 level > 上次 level
                    //    deckNameQueue.Enqueue(node.Title);
                    //}
                    //else if (node.Level == level)
                    //{
                    //    // 本次 level = 上次 level
                    //    deckNameQueue.Dequeue();
                    //    deckNameQueue.Enqueue(node.Title);
                    //}
                    //else
                    //{
                    //    // 本次 level < 上次 level
                    //    for (int i = 0; node.Level < level; i++)
                    //    {

                    //    }
                    //    deckNameQueue.Dequeue();
                    //    deckNameQueue.Enqueue(node.Title);
                    //}
                    //level = node.Level; 
                    #endregion

                    Console.WriteLine($"Level: {node.Level}");
                    Console.WriteLine($"Title: {node.Title}");
                    Console.WriteLine($"FullTitle: {node.FullTitle}");
                    if (!string.IsNullOrEmpty(node.Content))
                    {
                        Console.WriteLine(node.Content);

                        try
                        {
                            string fullDeckName = settings.AnkiConnect.Note.DeckName + "::" + node.FullTitle;
                            await AddNoteAsync(deckName: fullDeckName, modelName: settings.AnkiConnect.Note.ModelName, front: node.Title, back: node.Content);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            }
        }

        private async Task<string> GetBase64ImageAsync(string imageUrl, string mdFilePath)
        {
            if (!imageUrl.StartsWith("http"))
            {
                imageUrl = Path.Combine(Directory.GetParent(mdFilePath).FullName, imageUrl);
                imageUrl = imageUrl.Replace("\\", "/");
            }
            var imageBytes = await File.ReadAllBytesAsync(imageUrl);

            string base64String = Convert.ToBase64String(imageBytes);

            if (!new FileExtensionContentTypeProvider().TryGetContentType(imageUrl, out string contentType))
                throw new NotSupportedException($"Image file type '{Path.GetExtension(imageUrl)}' is not supported.");

            return $"data:{contentType};base64,{base64String}";
        }

        public static void ParseNodes(MarkdownObject obj, List<MarkdownNode> nodes, int level)
        {
            if (obj is HeadingBlock)
            {
                HeadingBlock heading = (HeadingBlock)obj;
                var node = new MarkdownNode();
                node.Title = heading.Inline.FirstChild.ToString();
                node.Level = level;
                node.FullTitle = (string.IsNullOrEmpty(node.FullTitle) ? "" : "::") + heading.Inline.FirstChild.ToString();

                StringBuilder builder = new StringBuilder();
                // TODO: 无法获取标题下方普通内容
                foreach (var child in heading.Descendants())
                {
                    if (child is HeadingBlock)
                    {
                        break;
                    }
                    else
                    {
                        builder.AppendLine(child.ToString());
                    }
                }

                node.Content = builder.ToString().Trim();
                nodes.Add(node);

                level++;
            }

            if (obj is ContainerBlock container)
            {
                foreach (var child in container)
                {
                    ParseNodes(child, nodes, level);
                }
            }
        }

        public class MarkdownNode
        {
            public string Title { get; set; }
            public string Content { get; set; }
            public int Level { get; set; }
            public string FullTitle { get; set; }
        }
    }
}
