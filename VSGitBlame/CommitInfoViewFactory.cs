using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text.Editor;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using VSGitBlame.Core;
using System.Linq;

namespace VSGitBlame;

public static class CommitInfoViewFactory
{
    static Border _container;
    static TextBlock _summaryView;
    static TextBlock _commitDetailsView;
    static Image _profileIcon;

    static bool _firstMouseMoveFired = false;
    static bool _isDetailsVisible = false;


    static CommitInfoViewFactory()
    {
        #region Summary View
        var backgroundColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);
        _summaryView = new TextBlock
        {
            Opacity = 0.6,
            Background = Brushes.Transparent,
            Foreground = backgroundColor.GetBrightness() > 0.5 ? Brushes.DarkBlue : Brushes.LightGray,
            FontStyle = FontStyles.Italic,
            FontWeight = FontWeights.Bold,
        };
        #endregion

        #region Info View
        var infoView = new StackPanel
        {
            Background = new SolidColorBrush(Colors.DarkBlue),
            Orientation = Orientation.Horizontal,
        };

        _profileIcon = new Image
        {
            Width = 50,
            Height = 50,
            Margin = new Thickness(0, 0, 3, 0),
        };
        infoView.Children.Add(_profileIcon);

        _commitDetailsView = new TextBlock
        {
            Foreground = new SolidColorBrush(Colors.White),
            FontWeight = FontWeights.Bold,
        };
        infoView.Children.Add(_commitDetailsView);

        var infoViewContainer = new Border
        {
            BorderThickness = new Thickness(1),
            BorderBrush = Brushes.LightGreen,
            Visibility = Visibility.Hidden,
            Padding = new Thickness(2)
        };
        infoViewContainer.Child = infoView;
        #endregion

        #region Container
        var rootPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Background = Brushes.Transparent
        };
        rootPanel.Children.Add(infoViewContainer);
        rootPanel.Children.Add(_summaryView);

        rootPanel.MouseMove += (sender, e) =>
        {
            if (!_firstMouseMoveFired)
            {
                _firstMouseMoveFired = true;
                return;
            }

            if (_isDetailsVisible)
                return;

            infoViewContainer.Visibility = Visibility.Visible;
            _isDetailsVisible = true;
        };

        rootPanel.MouseLeave += (sender, e) =>
        {
            _firstMouseMoveFired = false;
            _isDetailsVisible = false;
            infoViewContainer.Visibility = Visibility.Hidden;
            _container.Visibility = Visibility.Hidden;
        };

        _container = new Border
        {
            Margin = new Thickness(30, 0, 0, 0),
        };
        _container.Child = rootPanel;
        #endregion
    }

    public static Border Get(CommitInfo commitInfo)
    {
        _summaryView.Text = $"{commitInfo.AuthorName}, {commitInfo.Time:yyyy/MM/dd HH:mm} • {commitInfo.Summary}";
        _profileIcon.Source = new BitmapImage(new Uri(GetGravatarUrl(commitInfo.AuthorEmail), UriKind.Absolute));
        _commitDetailsView.Text =
            $"""
            {commitInfo.AuthorName} | {commitInfo.Time:f}
            {commitInfo.Summary}
            Commit: {commitInfo.Hash.Substring(7)}
            """;
        _container.Visibility = Visibility.Visible;

        return _container;
    }

    static string GetGravatarUrl(string email)
    {
        string emailMD5 = System.Security.Cryptography.MD5.Create().ComputeHash(System.Text.Encoding.ASCII.GetBytes(email))
            .Select(b => b.ToString("x2")).Aggregate((s1, s2) => s1 + s2);
        return $"https://www.gravatar.com/avatar/{emailMD5}";
    }
}
