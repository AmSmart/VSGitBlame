using System.Collections.Generic;
using System.Diagnostics;
using VSGitBlame.Core;

namespace VSGitBlame;

internal static class GitBlamer
{
    static Dictionary<string, FileBlameInfo> _gitBlameCache = new();

    public static void InvalidateCache(string filePath) =>
        _gitBlameCache.Remove(filePath);

    public static CommitInfo GetBlame(string filePath, int line)
    {
        if (_gitBlameCache.TryGetValue(filePath, out var fileBlameInfo) == false)
            fileBlameInfo = InitialiseFile(filePath);

        if (fileBlameInfo == null)
            return null;

        return fileBlameInfo.GetAt(line);
    }

    
    static FileBlameInfo InitialiseFile(string filePath)
    {
        string command = $"git blame {filePath} --porcelain";

        using Process process = new Process();
        process.StartInfo = new ProcessStartInfo("cmd", "/c " + command)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        if (string.IsNullOrEmpty(result) || process.ExitCode != 0)
        {
            _gitBlameCache[filePath] = null;
            return null;
        }

        var blameInfo = new FileBlameInfo(result);
        _gitBlameCache[filePath] = blameInfo;

        return blameInfo;
    }
}
