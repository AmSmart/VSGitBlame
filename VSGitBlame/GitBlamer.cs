using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using VSGitBlame.Core;

namespace VSGitBlame;

internal static class GitBlamer
{
    private static ConcurrentDictionary<string, FileBlameInfo> _gitBlameCache = new();
    public static EventHandler<string> OnBlameFinished;

    public static void InvalidateCache(string filePath)
    {
        _gitBlameCache.TryRemove(filePath, out _);
    }


    public static CommitInfo GetBlame(string filePath, int line)
    {
        if (_gitBlameCache.TryGetValue(filePath, out FileBlameInfo fileBlameInfo))
            return fileBlameInfo != null ? fileBlameInfo.GetAt(line) : CommitInfo.InProgress;

        var curContext = SynchronizationContext.Current;
        _ = InitialiseFileAsync(filePath);

        return CommitInfo.InProgress;
    }

    
    static async Task InitialiseFileAsync(string filePath)
    {
        _gitBlameCache[filePath] = null;

        string command = $"git blame {filePath} --porcelain";

        using Process process = new Process();
        process.StartInfo = new ProcessStartInfo("cmd", "/c " + command)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = System.Text.Encoding.UTF8,
            WorkingDirectory = Path.GetDirectoryName(filePath)
        };
        process.Start();

        string result = await process.StandardOutput.ReadToEndAsync();

        // We invalidated the file during the process, so we don't need the output anymore
        if (_gitBlameCache.ContainsKey(filePath) == false)
            return;

        var blameInfo = new FileBlameInfo();
        _gitBlameCache[filePath] = blameInfo;

        if (process.ExitCode == 0 && string.IsNullOrEmpty(result) == false)
            blameInfo.Parse(result);

        OnBlameFinished.Invoke(null, filePath);
    }
}
