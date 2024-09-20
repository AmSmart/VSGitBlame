using System;

namespace VSGitBlame.Core;

public static class ReadOnlySpanExtensions
{
    public static ReadOnlySpan<T> SliceTill<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> delimiter) where T : IEquatable<T>
    {
        int delimiterIndex = span.IndexOf(delimiter);
        if (delimiterIndex == -1)
            throw new IndexOutOfRangeException();

        span = span.Slice(0, delimiterIndex);

        return span;
    }

    public static ReadOnlySpan<T> CropTillNth<T>(this ref ReadOnlySpan<T> span, ReadOnlySpan<T> delimiter, int n = 1) where T : IEquatable<T>
    {
        for (int i = 0; i < n; i++)
        {
            int delimiterIndex = span.IndexOf(delimiter);
            if (delimiterIndex == -1)
                throw new IndexOutOfRangeException();

            span = span.Slice(delimiterIndex + delimiter.Length);
        }

        return span;
    }

    public static bool CanCropTillNth<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> delimiter, int n = 1) where T : IEquatable<T>
    {
        for (int i = 0; i < n; i++)
        {
            int delimiterIndex = span.IndexOf(delimiter);
            if (delimiterIndex == -1)
                return false;


            span = span.Slice(delimiterIndex + delimiter.Length);
        }

        return true;
    }
}