using Microsoft.AspNetCore.Components;

namespace Larp.Landing.Client.Shared;

public class PageInfo
{
    private bool _container;
    private RenderFragment? _headerText;

    public RenderFragment? HeaderText
    {
        get => _headerText;
        set
        {
            _headerText = value;
            PageInfoChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool Container
    {
        get => _container;
        set
        {
            _container = value;
            PageInfoChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? PageInfoChanged;
}