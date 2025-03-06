// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemosPlus.Utils;

public class FileUtil
{
    #region 安全的文件名
    public static string SoftFileName(string fileName, char replaceChar = '_')
    {
        HashSet<char> blocked = new(System.IO.Path.GetInvalidFileNameChars());
        char[] output = fileName.ToCharArray();
        for (int i = 0, ln = output.Length; i < ln; i++)
        {
            if (blocked.Contains(output[i]))
            {
                output[i] = replaceChar;
            }
        }
        return new String(output);
    }
    #endregion

    #region 安全的文件路径
    public static string SafeFilePath(string filePath, char replaceChar = '_')
    {
        if (filePath == null)
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        // 获取非法字符集合
        HashSet<char> blocked = new HashSet<char>(Path.GetInvalidFileNameChars());
        blocked.UnionWith(Path.GetInvalidPathChars());

        // Path.GetInvalidPathChars() 默认不包含 / 和 \
        // 下方其实不需要, 但显式排除
        // 显式排除路径分隔符（根据操作系统）
        blocked.Remove(Path.DirectorySeparatorChar);    // 例如 Windows 中的 '\'
        blocked.Remove(Path.AltDirectorySeparatorChar); // 例如 Windows 中的 '/'

        // 替换非法字符
        char[] output = filePath.ToCharArray();
        for (int i = 0; i < output.Length; i++)
        {
            if (blocked.Contains(output[i]))
            {
                output[i] = replaceChar;
            }
        }

        return new string(output);
    }
    #endregion
}
