using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;

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


}
