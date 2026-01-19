# Modal Service Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Implement a service-based modal system with simple dialogs (Info/Warning/Error/Confirm) and custom component modals with typed results.

**Architecture:** ModalService tracks active modals using TaskCompletionSource for async results. ModalProvider renders modals as `<dialog>` elements. Custom components receive ModalInstance via CascadingParameter to close themselves.

**Tech Stack:** Blazor WebAssembly, DaisyUI v5 modal classes, TailwindMerge

---

## Task 1: Create Modal Supporting Types

**Files:**
- Create: `src/BlazyUI/Components/Modal/ModalType.cs`
- Create: `src/BlazyUI/Components/Modal/ModalResult.cs`
- Create: `src/BlazyUI/Components/Modal/ModalOptions.cs`

**Step 1: Create the Modal folder and ModalType enum**

```csharp
// src/BlazyUI/Components/Modal/ModalType.cs
namespace BlazyUI;

public enum ModalType
{
    Info,
    Warning,
    Error,
    Confirm,
    Custom
}
```

**Step 2: Create ModalResult classes**

```csharp
// src/BlazyUI/Components/Modal/ModalResult.cs
namespace BlazyUI;

public class ModalResult
{
    public bool Confirmed { get; init; }
}

public class ModalResult<T> : ModalResult
{
    public T? Data { get; init; }
}
```

**Step 3: Create ModalOptions class**

```csharp
// src/BlazyUI/Components/Modal/ModalOptions.cs
namespace BlazyUI;

public class ModalOptions
{
    public bool CloseOnBackdropClick { get; set; } = false;
    public string? CssClass { get; set; }
}
```

**Step 4: Build to verify**

Run: `dotnet build`
Expected: Build succeeded

**Step 5: Commit**

```bash
git add src/BlazyUI/Components/Modal/
git commit -m "feat(modal): add ModalType, ModalResult, and ModalOptions types"
```

---

## Task 2: Create ModalInstance Class

**Files:**
- Create: `src/BlazyUI/Components/Modal/ModalInstance.cs`

**Step 1: Create ModalInstance class**

```csharp
// src/BlazyUI/Components/Modal/ModalInstance.cs
namespace BlazyUI;

public class ModalInstance
{
    private readonly TaskCompletionSource<ModalResult> _tcs;
    private readonly Action<ModalInstance> _onClose;

    internal ModalInstance(
        TaskCompletionSource<ModalResult> tcs,
        Action<ModalInstance> onClose)
    {
        _tcs = tcs;
        _onClose = onClose;
    }

    public void Close(bool confirmed = true)
    {
        _tcs.TrySetResult(new ModalResult { Confirmed = confirmed });
        _onClose(this);
    }

    public void Close<T>(T? data, bool confirmed = true)
    {
        _tcs.TrySetResult(new ModalResult<T>
        {
            Confirmed = confirmed,
            Data = data
        });
        _onClose(this);
    }
}
```

**Step 2: Build to verify**

Run: `dotnet build`
Expected: Build succeeded

**Step 3: Commit**

```bash
git add src/BlazyUI/Components/Modal/ModalInstance.cs
git commit -m "feat(modal): add ModalInstance for custom component close handling"
```

---

## Task 3: Create IModalService Interface

**Files:**
- Create: `src/BlazyUI/Components/Modal/IModalService.cs`

**Step 1: Create IModalService interface**

```csharp
// src/BlazyUI/Components/Modal/IModalService.cs
using Microsoft.AspNetCore.Components;

namespace BlazyUI;

public interface IModalService
{
    /// <summary>
    /// Event raised when modals collection changes.
    /// </summary>
    event Action? OnChange;

    /// <summary>
    /// Shows an info dialog and waits for it to close.
    /// </summary>
    Task Info(string title, string message, string? okText = "OK");

    /// <summary>
    /// Shows a warning dialog and waits for it to close.
    /// </summary>
    Task Warning(string title, string message, string? okText = "OK");

    /// <summary>
    /// Shows an error dialog and waits for it to close.
    /// </summary>
    Task Error(string title, string message, string? okText = "OK");

    /// <summary>
    /// Shows a confirmation dialog and returns true if confirmed.
    /// </summary>
    Task<bool> Confirm(
        string title,
        string message,
        string? confirmText = "Confirm",
        string? cancelText = "Cancel");

    /// <summary>
    /// Shows a custom component modal and returns the typed result.
    /// </summary>
    Task<ModalResult<TResult>> Show<TComponent, TResult>(
        Dictionary<string, object>? parameters = null,
        Action<ModalOptions>? configure = null)
        where TComponent : IComponent;

    /// <summary>
    /// Shows a custom component modal and waits for it to close.
    /// </summary>
    Task<ModalResult> Show<TComponent>(
        Dictionary<string, object>? parameters = null,
        Action<ModalOptions>? configure = null)
        where TComponent : IComponent;
}
```

**Step 2: Build to verify**

Run: `dotnet build`
Expected: Build succeeded

**Step 3: Commit**

```bash
git add src/BlazyUI/Components/Modal/IModalService.cs
git commit -m "feat(modal): add IModalService interface"
```

---

## Task 4: Create Internal Modal Reference Class

**Files:**
- Create: `src/BlazyUI/Components/Modal/ModalReference.cs`

**Step 1: Create ModalReference class (internal tracking)**

```csharp
// src/BlazyUI/Components/Modal/ModalReference.cs
namespace BlazyUI;

internal class ModalReference
{
    public Guid Id { get; } = Guid.NewGuid();
    public ModalType Type { get; init; }
    public string? Title { get; init; }
    public string? Message { get; init; }
    public string OkText { get; init; } = "OK";
    public string CancelText { get; init; } = "Cancel";
    public bool CloseOnBackdropClick { get; init; }
    public Type? ComponentType { get; init; }
    public Dictionary<string, object>? Parameters { get; init; }
    public string? CssClass { get; init; }
    public TaskCompletionSource<ModalResult> TaskCompletionSource { get; init; } = default!;
    public ModalInstance? Instance { get; set; }
}
```

**Step 2: Build to verify**

Run: `dotnet build`
Expected: Build succeeded

**Step 3: Commit**

```bash
git add src/BlazyUI/Components/Modal/ModalReference.cs
git commit -m "feat(modal): add internal ModalReference for tracking active modals"
```

---

## Task 5: Create ModalService Implementation

**Files:**
- Create: `src/BlazyUI/Components/Modal/ModalService.cs`

**Step 1: Create ModalService class**

```csharp
// src/BlazyUI/Components/Modal/ModalService.cs
using Microsoft.AspNetCore.Components;

namespace BlazyUI;

public class ModalService : IModalService
{
    private readonly List<ModalReference> _modals = new();

    public event Action? OnChange;

    internal IReadOnlyList<ModalReference> Modals => _modals;

    public Task Info(string title, string message, string? okText = "OK")
    {
        return ShowSimpleDialog(ModalType.Info, title, message, okText ?? "OK", closeOnBackdropClick: true);
    }

    public Task Warning(string title, string message, string? okText = "OK")
    {
        return ShowSimpleDialog(ModalType.Warning, title, message, okText ?? "OK", closeOnBackdropClick: true);
    }

    public Task Error(string title, string message, string? okText = "OK")
    {
        return ShowSimpleDialog(ModalType.Error, title, message, okText ?? "OK", closeOnBackdropClick: true);
    }

    public async Task<bool> Confirm(
        string title,
        string message,
        string? confirmText = "Confirm",
        string? cancelText = "Cancel")
    {
        var tcs = new TaskCompletionSource<ModalResult>();

        var modalRef = new ModalReference
        {
            Type = ModalType.Confirm,
            Title = title,
            Message = message,
            OkText = confirmText ?? "Confirm",
            CancelText = cancelText ?? "Cancel",
            CloseOnBackdropClick = false,
            TaskCompletionSource = tcs
        };

        _modals.Add(modalRef);
        OnChange?.Invoke();

        var result = await tcs.Task;
        return result.Confirmed;
    }

    public Task<ModalResult<TResult>> Show<TComponent, TResult>(
        Dictionary<string, object>? parameters = null,
        Action<ModalOptions>? configure = null)
        where TComponent : IComponent
    {
        return ShowCustomModal<TComponent, TResult>(parameters, configure);
    }

    public async Task<ModalResult> Show<TComponent>(
        Dictionary<string, object>? parameters = null,
        Action<ModalOptions>? configure = null)
        where TComponent : IComponent
    {
        var result = await ShowCustomModal<TComponent, object>(parameters, configure);
        return new ModalResult { Confirmed = result.Confirmed };
    }

    private async Task ShowSimpleDialog(
        ModalType type,
        string title,
        string message,
        string okText,
        bool closeOnBackdropClick)
    {
        var tcs = new TaskCompletionSource<ModalResult>();

        var modalRef = new ModalReference
        {
            Type = type,
            Title = title,
            Message = message,
            OkText = okText,
            CloseOnBackdropClick = closeOnBackdropClick,
            TaskCompletionSource = tcs
        };

        _modals.Add(modalRef);
        OnChange?.Invoke();

        await tcs.Task;
    }

    private async Task<ModalResult<TResult>> ShowCustomModal<TComponent, TResult>(
        Dictionary<string, object>? parameters,
        Action<ModalOptions>? configure)
        where TComponent : IComponent
    {
        var options = new ModalOptions();
        configure?.Invoke(options);

        var tcs = new TaskCompletionSource<ModalResult>();

        var modalRef = new ModalReference
        {
            Type = ModalType.Custom,
            ComponentType = typeof(TComponent),
            Parameters = parameters,
            CloseOnBackdropClick = options.CloseOnBackdropClick,
            CssClass = options.CssClass,
            TaskCompletionSource = tcs
        };

        modalRef.Instance = new ModalInstance(tcs, CloseModal);

        _modals.Add(modalRef);
        OnChange?.Invoke();

        var result = await tcs.Task;

        if (result is ModalResult<TResult> typedResult)
        {
            return typedResult;
        }

        return new ModalResult<TResult> { Confirmed = result.Confirmed };
    }

    internal void CloseModal(ModalReference modal, bool confirmed)
    {
        modal.TaskCompletionSource.TrySetResult(new ModalResult { Confirmed = confirmed });
        CloseModal(modal.Instance!);
    }

    private void CloseModal(ModalInstance instance)
    {
        var modal = _modals.FirstOrDefault(m => m.Instance == instance);
        if (modal != null)
        {
            _modals.Remove(modal);
            OnChange?.Invoke();
        }
    }

    internal void CloseModalById(Guid id, bool confirmed)
    {
        var modal = _modals.FirstOrDefault(m => m.Id == id);
        if (modal != null)
        {
            modal.TaskCompletionSource.TrySetResult(new ModalResult { Confirmed = confirmed });
            _modals.Remove(modal);
            OnChange?.Invoke();
        }
    }
}
```

**Step 2: Build to verify**

Run: `dotnet build`
Expected: Build succeeded

**Step 3: Commit**

```bash
git add src/BlazyUI/Components/Modal/ModalService.cs
git commit -m "feat(modal): add ModalService implementation"
```

---

## Task 6: Create Icon Components

**Files:**
- Create: `src/BlazyUI/Components/Modal/Icons/InfoIcon.razor`
- Create: `src/BlazyUI/Components/Modal/Icons/WarningIcon.razor`
- Create: `src/BlazyUI/Components/Modal/Icons/ErrorIcon.razor`
- Create: `src/BlazyUI/Components/Modal/Icons/QuestionIcon.razor`

**Step 1: Create Icons folder and InfoIcon**

```razor
@* src/BlazyUI/Components/Modal/Icons/InfoIcon.razor *@
@namespace BlazyUI

<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="@Class" @attributes="AdditionalAttributes">
    <path stroke-linecap="round" stroke-linejoin="round" d="m11.25 11.25.041-.02a.75.75 0 0 1 1.063.852l-.708 2.836a.75.75 0 0 0 1.063.853l.041-.021M21 12a9 9 0 1 1-18 0 9 9 0 0 1 18 0Zm-9-3.75h.008v.008H12V8.25Z" />
</svg>

@code {
    [Parameter] public string? Class { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
```

**Step 2: Create WarningIcon**

```razor
@* src/BlazyUI/Components/Modal/Icons/WarningIcon.razor *@
@namespace BlazyUI

<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="@Class" @attributes="AdditionalAttributes">
    <path stroke-linecap="round" stroke-linejoin="round" d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126ZM12 15.75h.007v.008H12v-.008Z" />
</svg>

@code {
    [Parameter] public string? Class { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
```

**Step 3: Create ErrorIcon**

```razor
@* src/BlazyUI/Components/Modal/Icons/ErrorIcon.razor *@
@namespace BlazyUI

<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="@Class" @attributes="AdditionalAttributes">
    <path stroke-linecap="round" stroke-linejoin="round" d="m9.75 9.75 4.5 4.5m0-4.5-4.5 4.5M21 12a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z" />
</svg>

@code {
    [Parameter] public string? Class { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
```

**Step 4: Create QuestionIcon**

```razor
@* src/BlazyUI/Components/Modal/Icons/QuestionIcon.razor *@
@namespace BlazyUI

<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="@Class" @attributes="AdditionalAttributes">
    <path stroke-linecap="round" stroke-linejoin="round" d="M9.879 7.519c1.171-1.025 3.071-1.025 4.242 0 1.172 1.025 1.172 2.687 0 3.712-.203.179-.43.326-.67.442-.745.361-1.45.999-1.45 1.827v.75M21 12a9 9 0 1 1-18 0 9 9 0 0 1 18 0Zm-9 5.25h.008v.008H12v-.008Z" />
</svg>

@code {
    [Parameter] public string? Class { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
```

**Step 5: Build to verify**

Run: `dotnet build`
Expected: Build succeeded

**Step 6: Commit**

```bash
git add src/BlazyUI/Components/Modal/Icons/
git commit -m "feat(modal): add Info, Warning, Error, and Question icons"
```

---

## Task 7: Create ModalProvider Component

**Files:**
- Create: `src/BlazyUI/Components/Modal/ModalProvider.razor`

**Step 1: Create ModalProvider component**

```razor
@* src/BlazyUI/Components/Modal/ModalProvider.razor *@
@namespace BlazyUI
@implements IDisposable

@foreach (var modal in _modals)
{
    <dialog class="modal modal-open">
        <div class="modal-box @modal.CssClass">
            @if (modal.Type == ModalType.Custom)
            {
                <CascadingValue Value="modal.Instance">
                    <DynamicComponent Type="@modal.ComponentType" Parameters="@modal.Parameters" />
                </CascadingValue>
            }
            else
            {
                <div class="flex gap-4">
                    <div class="@GetIconColorClass(modal.Type)">
                        @switch (modal.Type)
                        {
                            case ModalType.Info:
                                <InfoIcon Class="w-6 h-6" />
                                break;
                            case ModalType.Warning:
                                <WarningIcon Class="w-6 h-6" />
                                break;
                            case ModalType.Error:
                                <ErrorIcon Class="w-6 h-6" />
                                break;
                            case ModalType.Confirm:
                                <QuestionIcon Class="w-6 h-6" />
                                break;
                        }
                    </div>
                    <div class="flex-1">
                        <h3 class="font-bold text-lg">@modal.Title</h3>
                        <p class="py-4">@modal.Message</p>
                    </div>
                </div>

                <div class="modal-action">
                    @if (modal.Type == ModalType.Confirm)
                    {
                        <button class="btn" @onclick="() => Close(modal, false)">
                            @modal.CancelText
                        </button>
                    }
                    <button class="btn btn-primary" @onclick="() => Close(modal, true)">
                        @modal.OkText
                    </button>
                </div>
            }
        </div>

        @if (modal.CloseOnBackdropClick)
        {
            <div class="modal-backdrop" @onclick="() => Close(modal, false)"></div>
        }
        else
        {
            <div class="modal-backdrop"></div>
        }
    </dialog>
}

@code {
    [Inject] private IModalService ModalService { get; set; } = default!;

    private ModalService? _service;
    private IReadOnlyList<ModalReference> _modals = Array.Empty<ModalReference>();

    protected override void OnInitialized()
    {
        _service = ModalService as ModalService;
        if (_service != null)
        {
            _modals = _service.Modals;
            _service.OnChange += HandleChange;
        }
    }

    private void HandleChange()
    {
        if (_service != null)
        {
            _modals = _service.Modals;
        }
        InvokeAsync(StateHasChanged);
    }

    private void Close(ModalReference modal, bool confirmed)
    {
        _service?.CloseModalById(modal.Id, confirmed);
    }

    private static string GetIconColorClass(ModalType type) => type switch
    {
        ModalType.Info => "text-info",
        ModalType.Warning => "text-warning",
        ModalType.Error => "text-error",
        ModalType.Confirm => "text-base-content",
        _ => ""
    };

    public void Dispose()
    {
        if (_service != null)
        {
            _service.OnChange -= HandleChange;
        }
    }
}
```

**Step 2: Build to verify**

Run: `dotnet build`
Expected: Build succeeded

**Step 3: Commit**

```bash
git add src/BlazyUI/Components/Modal/ModalProvider.razor
git commit -m "feat(modal): add ModalProvider component for rendering modals"
```

---

## Task 8: Create ServiceCollectionExtensions

**Files:**
- Create: `src/BlazyUI/Extensions/ServiceCollectionExtensions.cs`

**Step 1: Create Extensions folder and ServiceCollectionExtensions**

```csharp
// src/BlazyUI/Extensions/ServiceCollectionExtensions.cs
using Microsoft.Extensions.DependencyInjection;
using TailwindMerge.Extensions;

namespace BlazyUI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazyUI(this IServiceCollection services)
    {
        services.AddTailwindMerge();
        services.AddScoped<IModalService, ModalService>();
        return services;
    }
}
```

**Step 2: Build to verify**

Run: `dotnet build`
Expected: Build succeeded

**Step 3: Commit**

```bash
git add src/BlazyUI/Extensions/
git commit -m "feat: add AddBlazyUI extension method for service registration"
```

---

## Task 9: Update Demo App to Use Modal Service

**Files:**
- Modify: `src/BlazyUI.Demo/Program.cs`
- Modify: `src/BlazyUI.Demo/Layout/MainLayout.razor`

**Step 1: Update Program.cs to use AddBlazyUI**

Replace:
```csharp
builder.Services.AddTailwindMerge();
```

With:
```csharp
builder.Services.AddBlazyUI();
```

Full file:
```csharp
// src/BlazyUI.Demo/Program.cs
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazyUI;
using BlazyUI.Demo;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazyUI();

await builder.Build().RunAsync();
```

**Step 2: Update MainLayout.razor to include ModalProvider**

Add `<ModalProvider />` at the top of MainLayout.razor:

```razor
@* src/BlazyUI.Demo/Layout/MainLayout.razor *@
@inherits LayoutComponentBase

<ModalProvider />

<div class="drawer lg:drawer-open">
    <input id="drawer" type="checkbox" class="drawer-toggle" />

    <div class="drawer-content flex flex-col">
        <!-- Navbar -->
        <div class="navbar bg-base-200 lg:hidden">
            <div class="flex-none">
                <label for="drawer" class="btn btn-square btn-ghost drawer-button">
                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" class="inline-block w-5 h-5 stroke-current">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"></path>
                    </svg>
                </label>
            </div>
            <div class="flex-1">
                <a class="btn btn-ghost text-xl" href="">BlazyUI</a>
            </div>
        </div>

        <!-- Page content -->
        <main class="flex-1 p-6">
            @Body
        </main>
    </div>

    <div class="drawer-side">
        <label for="drawer" aria-label="close sidebar" class="drawer-overlay"></label>
        <aside class="bg-base-200 min-h-full w-64">
            <div class="p-4">
                <a class="btn btn-ghost text-xl" href="">BlazyUI</a>
            </div>
            <NavMenu />
        </aside>
    </div>
</div>

<div id="blazor-error-ui" class="hidden fixed bottom-0 left-0 right-0 bg-error text-error-content p-4 text-center">
    An unhandled error has occurred.
    <a href="." class="link">Reload</a>
    <span class="dismiss cursor-pointer ml-4">X</span>
</div>
```

**Step 3: Build to verify**

Run: `dotnet build`
Expected: Build succeeded

**Step 4: Commit**

```bash
git add src/BlazyUI.Demo/Program.cs src/BlazyUI.Demo/Layout/MainLayout.razor
git commit -m "feat(demo): integrate Modal service and ModalProvider"
```

---

## Task 10: Create Modal Demo Page

**Files:**
- Create: `src/BlazyUI.Demo/Pages/Modals.razor`
- Modify: `src/BlazyUI.Demo/Layout/NavMenu.razor`

**Step 1: Create Modals demo page**

```razor
@* src/BlazyUI.Demo/Pages/Modals.razor *@
@page "/modals"
@inject IModalService ModalService

<PageTitle>Modal - BlazyUI</PageTitle>

<h1 class="text-3xl font-bold mb-8">Modal</h1>

<section class="mb-12">
    <h2 class="text-xl font-semibold mb-4">Simple Dialogs</h2>
    <CodeExample Code="@simpleCode">
        <div class="flex flex-wrap gap-4">
            <Button Color="ButtonColor.Info" OnClick="ShowInfo">Show Info</Button>
            <Button Color="ButtonColor.Warning" OnClick="ShowWarning">Show Warning</Button>
            <Button Color="ButtonColor.Error" OnClick="ShowError">Show Error</Button>
        </div>
    </CodeExample>
</section>

<section class="mb-12">
    <h2 class="text-xl font-semibold mb-4">Confirmation Dialog</h2>
    <CodeExample Code="@confirmCode">
        <div class="flex flex-col gap-4 max-w-md">
            <Button Color="ButtonColor.Primary" OnClick="ShowConfirm">Delete Item</Button>
            @if (!string.IsNullOrEmpty(confirmResult))
            {
                <Alert Color="@(confirmResult == "Confirmed" ? AlertColor.Success : AlertColor.Info)">
                    Result: @confirmResult
                </Alert>
            }
        </div>
    </CodeExample>
</section>

<section class="mb-12">
    <h2 class="text-xl font-semibold mb-4">Custom Button Text</h2>
    <CodeExample Code="@customButtonCode">
        <div class="flex flex-wrap gap-4">
            <Button OnClick="ShowCustomConfirm">Discard Changes?</Button>
        </div>
    </CodeExample>
</section>

<section class="mb-12">
    <h2 class="text-xl font-semibold mb-4">Custom Component Modal</h2>
    <CodeExample Code="@customComponentCode">
        <div class="flex flex-col gap-4 max-w-md">
            <Button Color="ButtonColor.Primary" OnClick="ShowCustomModal">Edit Profile</Button>
            @if (editedUser != null)
            {
                <Alert Color="AlertColor.Success">
                    Saved: @editedUser.Name (@editedUser.Email)
                </Alert>
            }
        </div>
    </CodeExample>
</section>

@code {
    private string? confirmResult;
    private UserModel? editedUser;

    private async Task ShowInfo()
    {
        await ModalService.Info("Information", "This is an informational message.");
    }

    private async Task ShowWarning()
    {
        await ModalService.Warning("Warning", "This action may have consequences.");
    }

    private async Task ShowError()
    {
        await ModalService.Error("Error", "Something went wrong. Please try again.");
    }

    private async Task ShowConfirm()
    {
        var confirmed = await ModalService.Confirm(
            "Delete Item?",
            "This action cannot be undone. Are you sure you want to delete this item?");

        confirmResult = confirmed ? "Confirmed" : "Cancelled";
    }

    private async Task ShowCustomConfirm()
    {
        var confirmed = await ModalService.Confirm(
            "Discard Changes?",
            "You have unsaved changes that will be lost.",
            confirmText: "Yes, discard",
            cancelText: "Keep editing");
    }

    private async Task ShowCustomModal()
    {
        var result = await ModalService.Show<EditUserModal, UserModel>(
            new Dictionary<string, object>
            {
                ["InitialName"] = "John Doe",
                ["InitialEmail"] = "john@example.com"
            });

        if (result.Confirmed && result.Data != null)
        {
            editedUser = result.Data;
        }
    }

    public class UserModel
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
    }

    private string simpleCode = @"@inject IModalService ModalService

<Button Color=""ButtonColor.Info"" OnClick=""ShowInfo"">Show Info</Button>
<Button Color=""ButtonColor.Warning"" OnClick=""ShowWarning"">Show Warning</Button>
<Button Color=""ButtonColor.Error"" OnClick=""ShowError"">Show Error</Button>

@code {
    async Task ShowInfo() => await ModalService.Info(""Information"", ""This is an informational message."");
    async Task ShowWarning() => await ModalService.Warning(""Warning"", ""This action may have consequences."");
    async Task ShowError() => await ModalService.Error(""Error"", ""Something went wrong."");
}";

    private string confirmCode = @"var confirmed = await ModalService.Confirm(
    ""Delete Item?"",
    ""This action cannot be undone."");

if (confirmed)
{
    // Proceed with deletion
}";

    private string customButtonCode = @"var confirmed = await ModalService.Confirm(
    ""Discard Changes?"",
    ""You have unsaved changes."",
    confirmText: ""Yes, discard"",
    cancelText: ""Keep editing"");";

    private string customComponentCode = @"var result = await ModalService.Show<EditUserModal, UserModel>(
    new Dictionary<string, object>
    {
        [""InitialName""] = ""John Doe"",
        [""InitialEmail""] = ""john@example.com""
    });

if (result.Confirmed && result.Data != null)
{
    await SaveUser(result.Data);
}";
}
```

**Step 2: Create EditUserModal component for the demo**

```razor
@* src/BlazyUI.Demo/Pages/EditUserModal.razor *@
<h3 class="font-bold text-lg">Edit Profile</h3>

<div class="py-4 flex flex-col gap-4">
    <Label Variant="LabelVariant.Input" Text="Name">
        <input type="text" class="grow" @bind="name" />
    </Label>

    <Label Variant="LabelVariant.Input" Text="Email">
        <input type="email" class="grow" @bind="email" />
    </Label>
</div>

<div class="modal-action">
    <Button OnClick="Cancel">Cancel</Button>
    <Button Color="ButtonColor.Primary" OnClick="Save">Save</Button>
</div>

@code {
    [CascadingParameter] public ModalInstance Modal { get; set; } = default!;
    [Parameter] public string InitialName { get; set; } = "";
    [Parameter] public string InitialEmail { get; set; } = "";

    private string name = "";
    private string email = "";

    protected override void OnInitialized()
    {
        name = InitialName;
        email = InitialEmail;
    }

    private void Cancel() => Modal.Close(confirmed: false);

    private void Save()
    {
        var user = new Modals.UserModel
        {
            Name = name,
            Email = email
        };
        Modal.Close(user, confirmed: true);
    }
}
```

**Step 3: Add Modal to NavMenu**

Add after the Fieldset link in NavMenu.razor:

```razor
<li>
    <NavLink class="flex items-center gap-3" href="modals">
        <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z" />
        </svg>
        Modal
    </NavLink>
</li>
```

**Step 4: Build to verify**

Run: `dotnet build`
Expected: Build succeeded

**Step 5: Run and manually test**

Run: `cd src/BlazyUI.Demo && dotnet run`

Test checklist:
- [ ] Navigate to /modals
- [ ] Click "Show Info" - info modal appears with blue icon
- [ ] Click "Show Warning" - warning modal appears with yellow icon
- [ ] Click "Show Error" - error modal appears with red icon
- [ ] Click "Delete Item" - confirm modal appears, test both Cancel and Confirm
- [ ] Click "Discard Changes?" - confirm modal with custom button text
- [ ] Click "Edit Profile" - custom component modal appears, edit and save
- [ ] Verify backdrop click closes Info/Warning/Error but not Confirm

**Step 6: Commit**

```bash
git add src/BlazyUI.Demo/Pages/Modals.razor src/BlazyUI.Demo/Pages/EditUserModal.razor src/BlazyUI.Demo/Layout/NavMenu.razor
git commit -m "feat(demo): add Modal demo page with examples"
```

---

## Summary

After completing all tasks, the Modal system will have:

1. **Supporting types:** `ModalType`, `ModalResult`, `ModalResult<T>`, `ModalOptions`
2. **ModalInstance:** For custom components to close themselves with data
3. **IModalService:** Public interface for modal operations
4. **ModalService:** Implementation tracking active modals
5. **Icon components:** InfoIcon, WarningIcon, ErrorIcon, QuestionIcon
6. **ModalProvider:** Renders active modals using `<dialog>` elements
7. **ServiceCollectionExtensions:** `AddBlazyUI()` for easy registration
8. **Demo page:** Showcasing all modal types and features
