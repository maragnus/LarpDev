using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Larp.Landing.Client.Pages;

public class LarpPage : ComponentBase
{
    [Inject] protected IDialogService DialogService { get; set; } = default!;
    [Inject] protected ILogger<LarpPage> Logger { get; set; } = default!;

    protected bool IsLoading { get; set; } = true;
    protected bool IsInitialized { get; set; }

    protected virtual Task OnSafeParametersSetAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnSafeInitializedAsync()
    {
        return Task.CompletedTask;
    }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        StateHasChanged();
        while (true)
        {
            try
            {
                await OnSafeInitializedAsync();
                IsInitialized = true;
            }
            catch (Exception ex)
            {
                if (await PromptForRetry(ex) == true) continue;
            }

            return;
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!IsInitialized) return;

        IsLoading = true;
        StateHasChanged();
        try
        {
            while (true)
            {
                try
                {
                    await OnSafeParametersSetAsync();
                }
                catch (Exception ex)
                {
                    if (await PromptForRetry(ex) == true) continue;
                }

                return;
            }
        }
        finally
        {
            IsLoading = false;
        }
    }


    protected async Task<bool> LoadingActionAsync(
        Func<Task> action)
    {
        IsLoading = true;
        StateHasChanged();
        try
        {
            while (true)
            {
                try
                {
                    await action();
                    return true;
                }
                catch (Exception ex)
                {
                    if (await PromptForRetry(ex) == true) continue;
                    return false;
                }
            }
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    protected async Task<bool> SafeActionAsync(
        Func<Task> action)
    {
        while (true)
        {
            try
            {
                await action();
                return true;
            }
            catch (Exception ex)
            {
                if (await PromptForRetry(ex) == true) continue;
                return false;
            }
        }
    }
    
    
    protected async Task<bool> SafeActionAsync(
        Func<Task> action, Func<Exception, Task> onFailure)
    {
        while (true)
        {
            try
            {
                await action();
                return true;
            }
            catch (Exception ex)
            {
                if (await PromptForRetry(ex) == true) continue;
                await onFailure(ex);
                return false;
            }
        }
    }
    
    
    protected async Task<bool> SafeActionAsync(
        Func<Task> action, Action<Exception> onFailure)
    {
        while (true)
        {
            try
            {
                await action();
                return true;
            }
            catch (Exception ex)
            {
                if (await PromptForRetry(ex) == true) continue;
                onFailure(ex);
                return false;
            }
        }
    }

    private async Task<bool?> PromptForRetry(Exception ex, [CallerMemberName] string? methodName = null)
    {
        if (ex is HttpRequestException httpRequestException)
        {
            Logger.LogError(ex, $"Exception during {nameof(SafeActionAsync)}: {{Message}}", ex.Message);

            if (httpRequestException.StatusCode == null)
                return await DialogService.ShowMessageBox("Server Error", "There was an issue contacting the server. Check your internet connection and try again.", "Retry", "Cancel");
            return await DialogService.ShowMessageBox("Server Error", ex.Message, "Retry", "Cancel");
        }

        Logger.LogError(ex, "Exception during {MethodName}: {Message}", methodName, ex.Message);
        return await DialogService.ShowMessageBox("Server Error", ex.Message, "Retry", "Cancel");
    }
}