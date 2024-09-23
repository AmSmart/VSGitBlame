using System;

namespace VSGitBlame.Core;

public class CommitInfo
{
    public string Hash { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public DateTimeOffset Time { get; set; }
}