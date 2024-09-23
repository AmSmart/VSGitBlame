using System.Collections.Generic;
using System.Diagnostics;
using VSGitBlame.Core;

namespace VSGitBlame;

internal static class GitBlamer
{
    static Dictionary<string, FileBlameInfo> _gitBlameCache = new();

    public static CommitInfo GetBlame(string filePath, int line)
    {
        if (!_gitBlameCache.ContainsKey(filePath))
            InitialiseFile(filePath);

        return _gitBlameCache[filePath].GetAt(line);
    }

    static string GetBlameCommand(string filePath) =>
        $"git blame {filePath} --porcelain";

    static void InitialiseFile(string filePath)
    {
        string command = GetBlameCommand(filePath);

        ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd", "/c " + command)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        string result = string.Empty;
        using Process process = new Process();
        process.StartInfo = processStartInfo;
        process.Start();
        result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        _gitBlameCache[filePath] = new FileBlameInfo(result);
    }
}
