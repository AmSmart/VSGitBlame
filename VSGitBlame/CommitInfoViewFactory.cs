using Microsoft.VisualStudio.PlatformUI;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using VSGitBlame.Core;
using System.Linq;
using Microsoft.VisualStudio.Text.Editor;
using Color = System.Drawing.Color;

namespace VSGitBlame;

public static class CommitInfoViewFactory
{
    static Border _container;
    static TextBlock _summaryView;
    static TextBlock _commitDetailsView;
    static Image _profileIcon;
    static StackPanel _detailsView;
    static Border _detailsViewContainer;

    static bool _firstMouseMoveFired = false;
    static bool _isDetailsVisible = false;
    static bool _showDetails = false;
    static IAdornmentLayer _adornmentLayer;
    
    private static VSGitBlamePackage _package;
    private static CommitInfoViewOptions _options;

    public static void InitializeSettings(VSGitBlamePackage package)
    {
        _package = package;
        LoadSettings();
    }

    private static void LoadSettings()
    {
        if (_package != null)
        {
            _options = _package.GetDialogPage(typeof(CommitInfoViewOptions)) as CommitInfoViewOptions;
        }
    }

    public static void RefreshSettings()
    {
        LoadSettings();
        if (_options != null)
        {
            ApplySettings();
        }
    }

    private static void ApplySettings()
    {
        if (_summaryView != null && _options != null)
        {
            // Apply summary view settings
            _summaryView.FontSize = _options.SummaryFontSize;

            // Override the foreground color if specified, otherwise use theme detection
            if (_options.SummaryFontColor != Color.Transparent)
            {
                _summaryView.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(
                    _options.SummaryFontColor.A, 
                    _options.SummaryFontColor.R, 
                    _options.SummaryFontColor.G, 
                    _options.SummaryFontColor.B));
            }
            else
            {
                // Use theme detection if no specific color is set
                var backgroundColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);
                _summaryView.Foreground = backgroundColor.GetBrightness() > 0.5 ? 
                    Brushes.DarkBlue : 
                    Brushes.LightGray;
            }
        }

        if (_commitDetailsView != null && _options != null)
        {
            // Apply details view settings
            _commitDetailsView.FontSize = _options.DetailsFontSize;
            _commitDetailsView.Foreground = _options.GetDetailsFontBrush();
        }

        if (_detailsView != null && _options != null)
        {
            // Apply background color setting
            _detailsViewContainer.Background = _options.GetDetailsBackgroundBrush();
        }
    }

    static CommitInfoViewFactory()
    {
        #region Summary View
        var backgroundColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);
        _summaryView = new TextBlock
        {
            Opacity = 0.5,
            Background = Brushes.Transparent,
            Foreground = backgroundColor.GetBrightness() > 0.5 ? Brushes.DarkBlue : Brushes.LightGray,
            FontStyle = FontStyles.Italic,
            FontWeight = FontWeights.Normal,
        };
        #endregion

        #region Details View
        _detailsView = new StackPanel
        {
            Orientation = Orientation.Horizontal,
        };

        _profileIcon = new Image
        {
            Width = 50,
            Height = 50,
            Margin = new Thickness(0, 0, 3, 0),
        };
        _detailsView.Children.Add(_profileIcon);

        _commitDetailsView = new TextBlock
        {
            Foreground = new SolidColorBrush(Colors.White),
            FontWeight = FontWeights.Bold,
        };
        _detailsView.Children.Add(_commitDetailsView);

        _detailsViewContainer = new Border
        {
            BorderThickness = new Thickness(1),
            Background = new SolidColorBrush(Colors.DarkBlue),
            BorderBrush = Brushes.Transparent,
            Visibility = Visibility.Hidden,
            Padding = new Thickness(5),
        };
        _detailsViewContainer.Child = _detailsView;
        #endregion

        #region Container
        var rootPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Background = Brushes.Transparent,
        };
        rootPanel.Children.Add(_summaryView);
        rootPanel.Children.Add(_detailsViewContainer);

        rootPanel.MouseMove += (sender, e) =>
        {
            if (!_firstMouseMoveFired)
            {
                _firstMouseMoveFired = true;
                return;
            }

            if (_isDetailsVisible || _showDetails == false)
                return;

            _detailsViewContainer.Visibility = Visibility.Visible;
            _isDetailsVisible = true;
        };

        rootPanel.MouseLeave += (sender, e) =>
        {
            _firstMouseMoveFired = false;
            _isDetailsVisible = false;
            _detailsViewContainer.Visibility = Visibility.Hidden;
        };

        _container = new Border
        {
            Margin = new Thickness(30, 0, 0, 0),
        };
        _container.Child = rootPanel;
        #endregion
    }

    public static Border Get(CommitInfo commitInfo, IAdornmentLayer adornmentLayer)
    {
        if (_adornmentLayer != null)
        {
            if (_adornmentLayer != adornmentLayer)
            {
                _adornmentLayer.RemoveAllAdornments();
                _adornmentLayer = adornmentLayer;
            }
        }
        else
        {
            _adornmentLayer = adornmentLayer;
        }

        if (commitInfo.ShowDetails == false)
        {
            _summaryView.Text = commitInfo.Summary;
            _profileIcon.Source = null;
            _commitDetailsView.Text = string.Empty;
            _detailsViewContainer.Visibility = Visibility.Hidden;
        }
        else
        {
            _summaryView.Text = $"{commitInfo.AuthorName}, {commitInfo.Time:yyyy/MM/dd HH:mm} • {commitInfo.Summary}";
            _profileIcon.Source = new BitmapImage(new Uri(GetGravatarUrl(commitInfo.AuthorEmail), UriKind.Absolute));
            _commitDetailsView.Text =
                $"""
            {commitInfo.AuthorName} | {commitInfo.Time:f}
            {commitInfo.Summary}
            Commit: {commitInfo.Hash.Substring(7)}
            """;
        }

        _showDetails = commitInfo.ShowDetails;
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
