using Microsoft.AspNetCore.Components;

namespace Larp.Landing.Client.Shared;

public class PageInfo
{
    private bool _container;
    private bool _darkMode;
    private RenderFragment? _headerText;

    public RenderFragment? HeaderText
    {
        get => _headerText;
        set
        {
            _headerText = value;
            NotifyPageInfoChanged();
        }
    }

    public bool Container
    {
        get => _container;
        set
        {
            _container = value;
            NotifyPageInfoChanged();
        }
    }

    public bool DarkMode
    {
        get => _darkMode;
        set
        {
            _darkMode = value;
            NotifyPageInfoChanged();
        }
    }

    private void NotifyPageInfoChanged()
    {
        PageInfoChanged?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? PageInfoChanged;
}