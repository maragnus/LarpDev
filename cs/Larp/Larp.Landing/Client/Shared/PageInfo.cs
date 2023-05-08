using Microsoft.AspNetCore.Components;

namespace Larp.Landing.Client.Shared;

public class PageInfo
{
    private RenderFragment? _headerText;
    public event EventHandler? PageInfoChanged;  
    
    public RenderFragment? HeaderText
    {
        get => _headerText;
        set
        {
            _headerText = value;
            PageInfoChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}