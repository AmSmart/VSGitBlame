using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Windows.Input;

namespace VSGitBlame;


[Export(typeof(IWpfTextViewCreationListener))]
[ContentType("text")]
[TextViewRole(PredefinedTextViewRoles.Document)]
internal sealed class CommitInfoAdornmentFactory : IWpfTextViewCreationListener
{
    [Export(typeof(AdornmentLayerDefinition))]
    [Name("CommitInfoAdornment")]
    [Order(After = PredefinedAdornmentLayers.Text)]
    private AdornmentLayerDefinition editorAdornmentLayer;

    public void TextViewCreated(IWpfTextView textView)
    {
        new CommitInfoAdornment(textView);
    }


    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Handle the click event here
        System.Windows.MessageBox.Show("Text editor clicked!");
    }
}

