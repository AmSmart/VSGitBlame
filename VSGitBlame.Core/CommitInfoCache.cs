using System.Collections.Generic;

namespace VSGitBlame.Core;

public static class CommitInfoCache
{
    static Dictionary<string, CommitInfo> _commitInfoCache = new();

    public static bool Exists(string hash)
    {
        return _commitInfoCache.ContainsKey(hash);
    }
    
    public static void Add(string hash, CommitInfo commitInfo)
    {
        _commitInfoCache[hash] = commitInfo;
    }

    public static CommitInfo Get(string hash)
    {
        _commitInfoCache.TryGetValue(hash, out CommitInfo commitInfo);
        return commitInfo;
    }
}
