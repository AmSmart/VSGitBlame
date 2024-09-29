using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System.Windows.Controls;
using VSGitBlame.Core;
using System.Windows.Input;

namespace VSGitBlame;

public class CommitInfoAdornment
{
    readonly IWpfTextView _view;
    readonly IAdornmentLayer _adornmentLayer;
    readonly ITextDocument _textDocument;

    public CommitInfoAdornment(IWpfTextView view)
    {
        _view = view;
        _adornmentLayer = view.GetAdornmentLayer("CommitInfoAdornment");
        _view.LayoutChanged += OnLayoutChanged;
        _view.VisualElement.MouseLeftButtonUp += VisualElement_MouseLeftButtonUp;
        _textDocument = _view.TextBuffer.Properties.GetProperty<ITextDocument>(typeof(ITextDocument));
        _textDocument.FileActionOccurred += TextDocument_FileActionOccurred;
    }

    private void TextDocument_FileActionOccurred(object sender, TextDocumentFileActionEventArgs e)
    {
        GitBlamer.InvalidateCache(_textDocument.FilePath);
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

        var commitInfo = GitBlamer.GetBlame(_textDocument.FilePath, lineNumber + 1);

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
        double top = line.Top - 52 < 0 ? 0 : line.Top - 52;
        var container = CommitInfoViewFactory.Get(commitInfo, _adornmentLayer);

        Canvas.SetLeft(container, line.Right);
        Canvas.SetTop(container, top);

        _adornmentLayer.RemoveAllAdornments();
        SnapshotSpan span = new SnapshotSpan(_adornmentLayer.TextView.TextSnapshot, Span.FromBounds(line.Start, line.End));
        _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, container, null);
    }
}