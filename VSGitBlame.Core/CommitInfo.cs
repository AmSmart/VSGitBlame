using System;

namespace VSGitBlame.Core;

public class CommitInfo
{
    public static readonly CommitInfo InProgress = new CommitInfo() { ShowDetails = false, Summary = "Blame in progress.." };
    public static readonly CommitInfo Uncommitted = new CommitInfo() { ShowDetails = false, Summary = "Uncommitted changes"};

    public bool ShowDetails { get; set; } = true;
    public string Hash { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public DateTimeOffset Time { get; set; }
}