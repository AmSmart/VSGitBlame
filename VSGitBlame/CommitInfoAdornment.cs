using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System.Windows.Controls;
using System.Windows.Media;
using VSGitBlame.Core;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using Microsoft.VisualStudio.PlatformUI;

namespace VSGitBlame;

// TODO: Make Details UI nicer
// TODO: Include all planned details in both views
// TODO: Prevent UIElement multiple allocations by reusing a single instance
// TODO: Cache author information and speed up parsing
// TODO: Implement a cache eviction policy (LRU) for files
// TODO: Validate that the last commit is still valid for data stored

public class CommitInfoAdornment
{
    private readonly IWpfTextView _view;
    private readonly IAdornmentLayer _adornmentLayer;

    bool _firstMouseMoveFired = false;
    bool _isDetailsVisible = false;

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
        var textViewLine = _view.TextViewLines.GetTextViewLineContainingYCoordinate(mousePosition.Y);

        if (textViewLine == null)
            return;

        // If the mouse click was within the line bounds, return
        // We only want to show the commit info if click is after last character in line
        var bufferPosition = textViewLine.GetBufferPositionFromXCoordinate(mousePosition.X);
        if (bufferPosition.HasValue)
            return;
        
        var textView = _view.GetTextViewLineContainingBufferPosition(textViewLine.End);
        int lineNumber = textViewLine.Start.GetContainingLineNumber();

        string filePath = _view.TextBuffer.Properties.GetProperty<ITextDocument>(typeof(ITextDocument)).FilePath;
        var commitInfo = GitBlamer.GetBlame(filePath, lineNumber + 1);
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
        var backgroundColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);
        var summaryView = new TextBlock
        {
            Text = $"{commitInfo.AuthorName}, some time ago • {commitInfo.Summary}",
            Opacity = 0.6,
            Background = Brushes.Transparent,
            Foreground = backgroundColor.GetBrightness() > 0.5 ? Brushes.DarkBlue : Brushes.LightGray,
            FontStyle = FontStyles.Italic,
            FontWeight = FontWeights.Bold
        };

        var container = new Border
        {
            Margin = new Thickness(30, 0, 0, 0),
            BorderBrush = Brushes.Transparent,
        };
        var rootPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Background = Brushes.Transparent
        };
        rootPanel.Children.Add(summaryView);
        
        rootPanel.MouseMove += (sender, e) =>
        {
            if (!_firstMouseMoveFired)
            {
                _firstMouseMoveFired = true;
                return;
            }

            if (_isDetailsVisible)
                return;

            var detailsView = CreateInfoUI(commitInfo);
            rootPanel.Children.Insert(0, detailsView);
            _isDetailsVisible = true;
        };

        rootPanel.MouseLeave += (sender, e) =>
        {
            _adornmentLayer.RemoveAllAdornments();
            _firstMouseMoveFired = false;
            _isDetailsVisible = false;
        };
        container.Child = rootPanel;

        Canvas.SetLeft(container, line.Right);
        Canvas.SetTop(container, line.Top);

        SnapshotSpan span = new SnapshotSpan(_adornmentLayer.TextView.TextSnapshot, Span.FromBounds(line.Start, line.End));
        _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, container, null);
    }

    static UIElement CreateInfoUI(CommitInfo commitInfo)
    {
        StackPanel rootPanel = new StackPanel
        {
            Background = new SolidColorBrush(Colors.DarkBlue),
            Orientation = Orientation.Horizontal
        };

        // Profile icon
        Image profileIcon = new Image
        {
            Width = 50,
            Height = 50,
            Source = new BitmapImage(new Uri(GetGravatarUrl(commitInfo.AuthorEmail), UriKind.Absolute))
        };
        rootPanel.Children.Add(profileIcon);

        // Commit details
        //TextBlock commitDetails = new TextBlock
        //{
        //    Text = "You 3 weeks ago (September 3rd, 2024 at 8:27 AM)",
        //    Foreground = new SolidColorBrush(Colors.LightGray),
        //    Margin = new Thickness(0, 10, 0, 0)
        //};
        //rootPanel.Children.Add(commitDetails);

        // Commit message
        TextBlock commitMessage = new TextBlock
        {
            Text = commitInfo.Summary,
            Foreground = new SolidColorBrush(Colors.White),
            FontWeight = FontWeights.Bold,
        };
        rootPanel.Children.Add(commitMessage);

        return rootPanel;
    }

    static string GetGravatarUrl(string email)
    {
        string emailMD5 = System.Security.Cryptography.MD5.Create().ComputeHash(System.Text.Encoding.ASCII.GetBytes(email))
            .Select(b => b.ToString("x2")).Aggregate((s1, s2) => s1 + s2);
        return $"https://www.gravatar.com/avatar/{emailMD5}";
    }
}

