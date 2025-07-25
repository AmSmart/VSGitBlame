using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace VSGitBlame;

public class CommitInfoViewOptions : DialogPage
{
    private const string CategoryDisplay = "Display";

    #region Summary View Settings
    [Category(CategoryDisplay)]
    [DisplayName("Summary Font Size")]
    [Description("Font size for the summary view")]
    [DefaultValue(12.0)]
    public double SummaryFontSize { get; set; } = 12.0;

    [Category(CategoryDisplay)]
    [DisplayName("Summary Font Color")]
    [Description("Font color for the summary view")]
    public Color SummaryFontColor { get; set; } = Color.Transparent;
    #endregion

    #region Details View Settings
    [Category(CategoryDisplay)]
    [DisplayName("Details Visibility")]
    [Description("Enable details view")]
    [DefaultValue(false)]
    public bool DetailsVisibility { get; set; } = false;

    [Category(CategoryDisplay)]
    [DisplayName("Details Font Size")]
    [Description("Font size for the details view")]
    [DefaultValue(12.0)]
    public double DetailsFontSize { get; set; } = 12.0;

    [Category(CategoryDisplay)]
    [DisplayName("Details Font Color")]
    [Description("Font color for the details view")]
    public Color DetailsFontColor { get; set; } = Color.White;

    [Category(CategoryDisplay)]
    [DisplayName("Details Background Color")]
    [Description("Background color for the details view")]
    public Color DetailsBackgroundColor { get; set; } = Color.DarkBlue;
    #endregion

    public System.Windows.Media.Brush GetSummaryFontBrush()
    {
        return new SolidColorBrush(System.Windows.Media.Color.FromArgb(
            SummaryFontColor.A, SummaryFontColor.R, SummaryFontColor.G, SummaryFontColor.B));
    }

    public System.Windows.Media.Brush GetDetailsFontBrush()
    {
        return new SolidColorBrush(System.Windows.Media.Color.FromArgb(
            DetailsFontColor.A, DetailsFontColor.R, DetailsFontColor.G, DetailsFontColor.B));
    }

    public System.Windows.Media.Brush GetDetailsBackgroundBrush()
    {
        return new SolidColorBrush(System.Windows.Media.Color.FromArgb(
            DetailsBackgroundColor.A, DetailsBackgroundColor.R, DetailsBackgroundColor.G, DetailsBackgroundColor.B));
    }

    private FontWeight ParseFontWeight(string weightName)
    {
        try
        {
            return (FontWeight)new FontWeightConverter().ConvertFromString(weightName);
        }
        catch
        {
            return FontWeights.Normal;
        }
    }

    public override void SaveSettingsToStorage()
    {
        base.SaveSettingsToStorage();
        CommitInfoViewFactory.RefreshSettings();
    }
}