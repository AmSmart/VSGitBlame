using System;

namespace VSGitBlame.Core;

public class CommitInfo
{
    public string Hash { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public DateTimeOffset Time { get; set; }

    public static CommitInfo Uncommitted = new CommitInfo
    {
        Hash = "0000000000000000000000000000000000000000",
        AuthorName = "You",
        AuthorEmail = null,
        Summary = "Uncommitted Changes",
        Time = DateTimeOffset.MinValue,
    };
}