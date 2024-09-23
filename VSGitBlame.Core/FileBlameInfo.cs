using System;
using System.Collections.Generic;

namespace VSGitBlame.Core;

public class FileBlameInfo
{
    Dictionary<int, string> _lineCommitCache;

    public FileBlameInfo(string porecelainBlameString)
    {
        _lineCommitCache = ParsePorcelainOutput(porecelainBlameString);
    }


    public CommitInfo GetAt(int line)
    {
        _lineCommitCache.TryGetValue(line, out string hash);

        if (string.IsNullOrEmpty(hash))
            return null;

        return CommitInfoCache.Get(hash);
    }

    Dictionary<int, string> ParsePorcelainOutput(string output)
    {
        var lineCommitCache = new Dictionary<int, string>();
        var commitHashes = new HashSet<string>();

        ReadOnlySpan<char> lines = output.AsSpan();
        ReadOnlySpan<char> space = " ".AsSpan();
        ReadOnlySpan<char> previous = "previous".AsSpan();
        ReadOnlySpan<char> newLine;
        bool scanComplete = false;

        // Get New line character(s) for the current fie
        int index1 = output.IndexOf('\n');
        int index2 = output.IndexOf("\r\n");
        
        if (index1 == -1 && index2 == -1)
            throw new Exception("Invalid string format");
        else if (index2 == -1)
            newLine = "\n".AsSpan();
        else
            newLine = "\r\n".AsSpan();

        while (scanComplete == false && lines.IsEmpty == false)
        {
            ReadOnlySpan<char> line = lines.SliceTill(newLine);

            if (line.Length == 0)
                break;

            lines.CropTillNth(newLine);

            string hash = line.SliceTill(space).ToString();
            line.CropTillNth(space, 2);
            int lineNumber = int.Parse(line.SliceTill(space).ToString());
            line.CropTillNth(space);
            int numberOfLines = int.Parse(line.ToString());
    
            if (commitHashes.Contains(hash) == false)
            {
                if (CommitInfoCache.Exists(hash))
                {
                    lines.CropTillNth(newLine, 9);
                }
                else
                {
                    line = lines.SliceTill(newLine);
                    lines.CropTillNth(newLine);
                    string authorName = line.CropTillNth(space).ToString();

                    line = lines.SliceTill(newLine);
                    lines.CropTillNth(newLine);
                    var authorEmailSlice = line.CropTillNth(space);
                    string authorEmail = authorEmailSlice.Slice(1, authorEmailSlice.Length - 2).ToString();

                    line = lines.SliceTill(newLine);
                    lines.CropTillNth(newLine);
                    long time = long.Parse(line.CropTillNth(space).ToString());
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(time);

                    line = lines.SliceTill(newLine);
                    lines.CropTillNth(newLine);
                
                    line.CropTillNth(space);
                    if (line[0] == '+')
                        line = line.Slice(1);
                    string timeZone = line.ToString();
                    TimeSpan timeZoneOffset = TimeSpan.ParseExact(timeZone, "hhmm", null);
                    dateTimeOffset = dateTimeOffset.ToOffset(timeZoneOffset);

                    lines.CropTillNth(newLine, 4);
                    line = lines.SliceTill(newLine);
                    lines.CropTillNth(newLine);
                    string summary = line.CropTillNth(space).ToString();

                    CommitInfo commitInfo = new CommitInfo
                    {
                        Hash = hash,
                        AuthorName = authorName,
                        AuthorEmail = authorEmail,
                        Summary = summary,
                        Time = dateTimeOffset
                    };

                    CommitInfoCache.Add(hash, commitInfo);
                    commitHashes.Add(hash);
                }

                int cropCount = lines.Slice(0, previous.Length).SequenceEqual(previous) ? 3 : 2;
                lines.CropTillNth(newLine, cropCount);
            }
            else
            {
                if (lines.CanCropTillNth(newLine, 1))
                    lines.CropTillNth(newLine, 1);
                else
                    scanComplete = true;
            }

            for (int i = lineNumber; i < lineNumber + numberOfLines; i++)
            {
                lineCommitCache[i] = hash;
            }


            if (scanComplete is not true)
            {
                int linesToCrop = (numberOfLines - 1) * 2;

                if (lines.CanCropTillNth(newLine, linesToCrop))
                    lines.CropTillNth(newLine, linesToCrop);
                else
                    scanComplete = true;
            }
        }

        return lineCommitCache;
    }
}