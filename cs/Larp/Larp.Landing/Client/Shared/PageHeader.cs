using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Larp.Landing.Client.Shared;

public sealed class PageHeader : ComponentBase
{
    /// <summary>
    /// Gets or sets the content to be rendered as the document title.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter] public bool Print { get; set; }

    [Parameter] public bool Container { get; set; }

    [CascadingParameter] public PageInfo? PageInfo { get; set; }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (PageInfo == null)
            return;

        PageInfo.HeaderText = ChildContent;
        PageInfo.Container = Container;
    }
}