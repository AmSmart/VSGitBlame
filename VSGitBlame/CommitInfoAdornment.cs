using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System.Windows.Controls;
using VSGitBlame.Core;
using System.Windows.Input;

namespace VSGitBlame;

// TODO: Validate that the last commit is still valid for data stored

public class CommitInfoAdornment
{
    readonly IWpfTextView _view;
    readonly IAdornmentLayer _adornmentLayer;


    public CommitInfoAdornment(IWpfTextView view)
    {
        _view = view;
        _adornmentLayer = view.GetAdornmentLayer("CommitInfoAdornment");
        _view.LayoutChanged += OnLayoutChanged;
        _view.VisualElement.MouseLeftButtonUp += VisualElement_MouseLeftButtonUp;
    }

    private void VisualElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _adornmentLayer.RemoveAllAdornments();

        var mousePosition = e.GetPosition(_view.VisualElement);
        
        // Get the scroll offset
        double verticalOffset = _view.ViewportTop;
        double horizontalOffset = _view.ViewportLeft;

        // Adjust the mouse position by the scroll offset
        double adjustedX = mousePosition.X + horizontalOffset;
        double adjustedY = mousePosition.Y + verticalOffset;

        var textViewLine = _view.TextViewLines.GetTextViewLineContainingYCoordinate(adjustedY);

        if (textViewLine == null)
            return;

        // If the mouse click was within the line bounds, return
        // We only want to show the commit info if click is after last character in line
        var bufferPosition = textViewLine.GetBufferPositionFromXCoordinate(adjustedX);
        if (bufferPosition.HasValue)
            return;
        
        var textView = _view.GetTextViewLineContainingBufferPosition(textViewLine.End);
        int lineNumber = textViewLine.Start.GetContainingLineNumber();

        string filePath = _view.TextBuffer.Properties.GetProperty<ITextDocument>(typeof(ITextDocument)).FilePath;
        var commitInfo = GitBlamer.GetBlame(filePath, lineNumber + 1);

        if (commitInfo == null)
            return;

        ShowCommitInfo(commitInfo, textView);
    }

    void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
    {
        foreach (var line in e.NewOrReformattedLines)
        {
            CreateVisuals(line);
        }
    }

    void CreateVisuals(ITextViewLine line)
    {
        // Clear previous adornments
        _adornmentLayer.RemoveAllAdornments();
    }

    void ShowCommitInfo(CommitInfo commitInfo, ITextViewLine line)
    {
        double top = line.Top - 50 < 0 ? 0 : line.Top - 50;
        var container = CommitInfoViewFactory.Get(commitInfo);

        Canvas.SetLeft(container, line.Right);
        Canvas.SetTop(container, top);

        _adornmentLayer.RemoveAllAdornments();
        SnapshotSpan span = new SnapshotSpan(_adornmentLayer.TextView.TextSnapshot, Span.FromBounds(line.Start, line.End));
        _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, container, null);
    }
}