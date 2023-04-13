using Microsoft.AspNetCore.Components;

namespace Larp.Landing.Client.Shared;

public class PageInfo
{
    public RenderFragment? HeaderText { get; set; }
    public bool Print { get; set; }
}