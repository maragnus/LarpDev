using Microsoft.AspNetCore.Components;

namespace Larp.Landing.Client.Shared;

using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;

public sealed class PageHeader : ComponentBase
{
    /// <summary>
    /// Gets or sets the content to be rendered as the document title.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    
    [Parameter]
    public bool Print { get; set; }

    [CascadingParameter]
    public PageInfo? PageInfo { get; set; }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (PageInfo == null)
            return;

        PageInfo.HeaderText = ChildContent;
    }
}