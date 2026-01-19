# Modal Component Design

## Overview

A service-based modal system for BlazyUI providing simple dialogs (Info/Warning/Error/Confirm) and custom component modals with typed results.

## Decisions

| Decision | Choice |
|----------|--------|
| Implementation | HTML `<dialog>` element (recommended by DaisyUI) |
| API Style | Service-based (`IModalService`) |
| Simple dialogs | `Info()`, `Warning()`, `Error()` - await until closed |
| Confirmation | `Confirm()` - returns `bool` |
| Custom components | `Show<TComponent, TResult>()` - returns `ModalResult<TResult>` |
| Button text | Customizable with sensible defaults |
| Positioning | Always centered (YAGNI) |
| Backdrop click | Configurable; Info/Warning/Error close by default, Confirm/Custom don't |
| Visual styling | Color-coded with icons per modal type |
| Registration | `AddBlazyUI()` extension method |

## Architecture

```
┌─────────────────────────────────────────────┐
│ MainLayout.razor                            │
│  ┌───────────────────────────────────────┐  │
│  │ ModalProvider                         │  │
│  │  - Subscribes to ModalService         │  │
│  │  - Renders <dialog> elements          │  │
│  │  - Handles open/close lifecycle       │  │
│  └───────────────────────────────────────┘  │
│                                             │
│  @Body (page content)                       │
│    │                                        │
│    └─> ModalService.Info("Title", "Msg")    │
│                                             │
└─────────────────────────────────────────────┘
```

### Components

**IModalService / ModalService** - Scoped service providing modal methods, tracks active modals using `TaskCompletionSource` for async results.

**ModalProvider** - Placed once in MainLayout, subscribes to service, renders active modals as `<dialog>` elements.

**ModalInstance** - Cascading parameter passed to custom components, provides `Close()` methods.

**ModalResult<T>** - Return type for custom components containing `Confirmed` and `Data` properties.

## File Structure

```
src/BlazyUI/
├── Components/Modal/
│   ├── IModalService.cs
│   ├── ModalService.cs
│   ├── ModalProvider.razor
│   ├── ModalInstance.cs
│   ├── ModalResult.cs
│   ├── ModalOptions.cs
│   ├── ModalType.cs
│   └── Icons/
│       ├── InfoIcon.razor
│       ├── WarningIcon.razor
│       ├── ErrorIcon.razor
│       └── QuestionIcon.razor
├── Extensions/
│   └── ServiceCollectionExtensions.cs
```

## Service API

```csharp
public interface IModalService
{
    // Simple dialogs - fire and forget, await until closed
    Task Info(string title, string message, string? okText = "OK");

    Task Warning(string title, string message, string? okText = "OK");

    Task Error(string title, string message, string? okText = "OK");

    // Confirmation - returns true if confirmed, false if cancelled
    Task<bool> Confirm(
        string title,
        string message,
        string? confirmText = "Confirm",
        string? cancelText = "Cancel");

    // Custom component - returns typed result
    Task<ModalResult<TResult>> Show<TComponent, TResult>(
        Dictionary<string, object>? parameters = null,
        Action<ModalOptions>? configure = null)
        where TComponent : IComponent;

    // Custom component - no return value (just await close)
    Task<ModalResult> Show<TComponent>(
        Dictionary<string, object>? parameters = null,
        Action<ModalOptions>? configure = null)
        where TComponent : IComponent;
}
```

## Supporting Types

```csharp
public class ModalOptions
{
    public bool CloseOnBackdropClick { get; set; } = false;
    public string? CssClass { get; set; } // extra classes for modal-box
}

public class ModalResult
{
    public bool Confirmed { get; set; }
}

public class ModalResult<T> : ModalResult
{
    public T? Data { get; set; }
}

public enum ModalType
{
    Info,
    Warning,
    Error,
    Confirm,
    Custom
}
```

## ModalInstance

Passed to custom components via `CascadingParameter`:

```csharp
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

## Built-in Dialog Template

Rendered by ModalProvider for Info/Warning/Error/Confirm:

```razor
<dialog class="modal modal-open">
    <div class="modal-box">
        <div class="flex gap-4">
            <div class="@IconColorClass">
                @switch (modal.Type)
                {
                    case ModalType.Info:
                        <InfoIcon class="w-6 h-6" />      @* text-info *@
                    case ModalType.Warning:
                        <WarningIcon class="w-6 h-6" />   @* text-warning *@
                    case ModalType.Error:
                        <ErrorIcon class="w-6 h-6" />     @* text-error *@
                    case ModalType.Confirm:
                        <QuestionIcon class="w-6 h-6" />  @* text-neutral *@
                }
            </div>

            <div>
                <h3 class="font-bold text-lg">@modal.Title</h3>
                <p class="py-4">@modal.Message</p>
            </div>
        </div>

        <div class="modal-action">
            @if (modal.Type == ModalType.Confirm)
            {
                <button class="btn" @onclick="() => Close(false)">
                    @modal.CancelText
                </button>
            }
            <button class="btn btn-primary" @onclick="() => Close(true)">
                @modal.OkText
            </button>
        </div>
    </div>

    @if (modal.CloseOnBackdropClick)
    {
        <form method="dialog" class="modal-backdrop">
            <button @onclick="() => Close(false)">close</button>
        </form>
    }
    else
    {
        <div class="modal-backdrop"></div>
    }
</dialog>
```

## Custom Component Template

```razor
<dialog class="modal modal-open">
    <div class="modal-box @modal.Options.CssClass">
        <CascadingValue Value="modalInstance">
            <DynamicComponent Type="@modal.ComponentType"
                              Parameters="@modal.Parameters" />
        </CascadingValue>
    </div>

    @if (modal.Options.CloseOnBackdropClick)
    {
        <form method="dialog" class="modal-backdrop">
            <button @onclick="() => Close(false)">close</button>
        </form>
    }
    else
    {
        <div class="modal-backdrop"></div>
    }
</dialog>
```

## Visual Styling

| Type | Icon | Color Class |
|------|------|-------------|
| Info | Circle with "i" | `text-info` |
| Warning | Triangle with "!" | `text-warning` |
| Error | Circle with "x" | `text-error` |
| Confirm | Circle with "?" | `text-base-content` |

## Setup

### Service Registration

```csharp
// Program.cs
builder.Services.AddBlazyUI();
```

Extension method:

```csharp
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

### Layout Setup

```razor
@* MainLayout.razor *@
@inherits LayoutComponentBase

<ModalProvider />

<div class="drawer lg:drawer-open">
    @* existing layout content *@
</div>
```

## Usage Examples

### Simple Dialogs

```csharp
@inject IModalService ModalService

// Information
await ModalService.Info("Success", "Record saved successfully");

// Warning
await ModalService.Warning("Caution", "This operation may take a while");

// Error
await ModalService.Error("Failed", "Could not connect to server");

// Custom button text
await ModalService.Error("Delete Failed", "File is in use", okText: "Got it");
```

### Confirmation

```csharp
var confirmed = await ModalService.Confirm(
    "Delete Record?",
    "This action cannot be undone.");

if (confirmed)
{
    await DeleteRecord();
}

// Custom button text
var confirmed = await ModalService.Confirm(
    "Discard Changes?",
    "You have unsaved changes.",
    confirmText: "Yes, discard",
    cancelText: "Keep editing");
```

### Custom Component

```razor
@* EditUserModal.razor *@
<h3 class="font-bold text-lg">Edit User</h3>

<div class="py-4">
    <Label Variant="LabelVariant.Input" Text="Name">
        <input type="text" class="grow" @bind="user.Name" />
    </Label>

    <Label Variant="LabelVariant.Input" Text="Email">
        <input type="email" class="grow" @bind="user.Email" />
    </Label>
</div>

<div class="modal-action">
    <Button OnClick="Cancel">Cancel</Button>
    <Button Color="ButtonColor.Primary" OnClick="Save">Save</Button>
</div>

@code {
    [CascadingParameter] public ModalInstance Modal { get; set; } = default!;
    [Parameter] public int UserId { get; set; }

    private UserModel user = new();

    protected override async Task OnInitializedAsync()
    {
        user = await UserService.GetUser(UserId);
    }

    void Cancel() => Modal.Close(confirmed: false);
    void Save() => Modal.Close(user, confirmed: true);
}
```

Calling the custom component:

```csharp
var result = await ModalService.Show<EditUserModal, UserModel>(
    new Dictionary<string, object> { ["UserId"] = 123 });

if (result.Confirmed && result.Data != null)
{
    await UserService.SaveUser(result.Data);
    await ModalService.Info("Saved", "User updated successfully");
}
```

### With Options

```csharp
var result = await ModalService.Show<LargeFormModal, FormData>(
    parameters: new Dictionary<string, object> { ["FormId"] = 456 },
    configure: options =>
    {
        options.CloseOnBackdropClick = true;
        options.CssClass = "max-w-3xl"; // wider modal
    });
```

## Backdrop Behavior Defaults

| Modal Type | CloseOnBackdropClick |
|------------|---------------------|
| Info | `true` |
| Warning | `true` |
| Error | `true` |
| Confirm | `false` |
| Custom | `false` (configurable) |
