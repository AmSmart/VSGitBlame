using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System.Windows.Controls;
using VSGitBlame.Core;
using System.Windows.Input;
using System;

namespace VSGitBlame;

public class CommitInfoAdornment
{
    readonly IWpfTextView _view;
    readonly IAdornmentLayer _adornmentLayer;
    readonly ITextDocument _textDocument;
    int _lastCaretLine = -1;

    public CommitInfoAdornment(IWpfTextView view)
    {
        _view = view;
        _adornmentLayer = view.GetAdornmentLayer("CommitInfoAdornment");
        _textDocument = _view.TextBuffer.Properties.GetProperty<ITextDocument>(typeof(ITextDocument));

        // Event Subscriptions
        _view.GotAggregateFocus += (sender, args) => RefreshBlameOnCurrentLine();
        _view.LayoutChanged += (sender, args) => RefreshBlameOnCurrentLine();
        _view.Closed += (sender, args) => GitBlamer.OnBlameFinished -= OnBlameFinished;

        _textDocument.FileActionOccurred += TextDocument_FileActionOccurred;
        _view.Caret.PositionChanged += Caret_PositionChanged;
        //_view.VisualElement.MouseLeftButtonUp += VisualElement_MouseLeftButtonUp;

        GitBlamer.OnBlameFinished += OnBlameFinished;

        RefreshBlameOnCurrentLine();
    }

    private void RefreshBlameOnCurrentLine()
    {
        OnCaretLineChanged(_lastCaretLine, new CaretPositionChangedEventArgs(_view, _view.Caret.Position, _view.Caret.Position));
    }

    private void Caret_PositionChanged(object sender, CaretPositionChangedEventArgs e)
    {
        int currentLine = e.NewPosition.BufferPosition.GetContainingLine().LineNumber;
        if (currentLine != _lastCaretLine)
        {
            _lastCaretLine = currentLine;
            OnCaretLineChanged(currentLine, e);
        }
    }

    private void TextDocument_FileActionOccurred(object sender, TextDocumentFileActionEventArgs e)
    {
        GitBlamer.InvalidateCache(_textDocument.FilePath);
        RefreshBlameOnCurrentLine();
    }

    private void OnBlameFinished(object sender, string filePath)
    {
        if (filePath != _textDocument.FilePath)
            return;

        RefreshBlameOnCurrentLine();
    }

    private void OnCaretLineChanged(int lineNumber, CaretPositionChangedEventArgs e)
    {
        // Only show commit info if the document is not dirty (no unsaved changes)
        if (_textDocument.IsDirty)
        {
            _adornmentLayer.RemoveAllAdornments();
            return;
        }

        // Get the caret position in the view
        var caretPosition = e.NewPosition.BufferPosition;
        var textViewLine = _view.GetTextViewLineContainingBufferPosition(caretPosition);

        if (textViewLine == null)
        {
            _adornmentLayer.RemoveAllAdornments();
            return;
        }

        var commitInfo = GitBlamer.GetBlame(_textDocument.FilePath, Math.Max(0, lineNumber) + 1);

        if (commitInfo == null)
        {
            _adornmentLayer.RemoveAllAdornments();
            return;
        }

        ShowCommitInfo(commitInfo, textViewLine);
    }

    private void VisualElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _adornmentLayer.RemoveAllAdornments();

        // Only show commit info if the document is not dirty (no unsaved changes)
        if (_textDocument.IsDirty)
            return;

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

    void ShowCommitInfo(CommitInfo commitInfo, ITextViewLine line)
    {
        var container = CommitInfoViewFactory.Get(commitInfo, _adornmentLayer);

        Canvas.SetLeft(container, line.Right);
        Canvas.SetTop(container, line.Top);

        SnapshotSpan span = new SnapshotSpan(_adornmentLayer.TextView.TextSnapshot, Span.FromBounds(line.Start, line.End));
        _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, container, null);
    }
}