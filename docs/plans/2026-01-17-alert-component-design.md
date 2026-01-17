# Alert Component Design

## Overview

A strongly-typed Blazor alert component wrapping DaisyUI's alert classes with full feature support.

## Decisions

| Decision | Choice |
|----------|--------|
| Element type | `<div role="alert">` |
| Feature scope | Colors, styles, layout |
| API style | Strongly-typed enums |
| Icon support | Optional `Icon` RenderFragment |
| Title/description | Users structure via ChildContent |
| Actions | Users include in ChildContent |
| Dismissible | No built-in (users handle visibility) |

## Enums

```csharp
public enum AlertColor
{
    None,       // No color modifier (default gray)
    Info,       // alert-info
    Success,    // alert-success
    Warning,    // alert-warning
    Error       // alert-error
}

public enum AlertStyle
{
    None,       // Solid fill (default)
    Outline,    // alert-outline
    Dash,       // alert-dash
    Soft        // alert-soft
}

public enum AlertLayout
{
    None,       // Default layout
    Vertical,   // alert-vertical
    Horizontal  // alert-horizontal
}
```

## Parameters

```csharp
// Appearance
[Parameter] public AlertColor Color { get; set; } = AlertColor.None;
[Parameter] public AlertStyle Style { get; set; } = AlertStyle.None;
[Parameter] public AlertLayout Layout { get; set; } = AlertLayout.None;

// Content
[Parameter] public RenderFragment? Icon { get; set; }
[Parameter] public RenderFragment? ChildContent { get; set; }

// Inherited from BlazyComponentBase
[Parameter] public string? Class { get; set; }
[Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }
```

## File Structure

```
src/BlazyUI/
├── Alert/
│   ├── Alert.razor
│   ├── AlertColor.cs
│   ├── AlertStyle.cs
│   └── AlertLayout.cs
├── Button/
│   └── ...
└── BlazyComponentBase.cs
```

## Implementation

**Alert.razor:**
```razor
@namespace BlazyUI
@inherits BlazyComponentBase

<div role="alert" class="@CssClass" @attributes="AdditionalAttributes">
    @Icon
    @ChildContent
</div>

@code {
    [Parameter] public AlertColor Color { get; set; } = AlertColor.None;
    [Parameter] public AlertStyle Style { get; set; } = AlertStyle.None;
    [Parameter] public AlertLayout Layout { get; set; } = AlertLayout.None;
    [Parameter] public RenderFragment? Icon { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private string CssClass => MergeClasses(
        "alert",
        ColorClass,
        StyleClass,
        LayoutClass
    );

    private string? ColorClass => Color switch
    {
        AlertColor.Info => "alert-info",
        AlertColor.Success => "alert-success",
        AlertColor.Warning => "alert-warning",
        AlertColor.Error => "alert-error",
        _ => null
    };

    private string? StyleClass => Style switch
    {
        AlertStyle.Outline => "alert-outline",
        AlertStyle.Dash => "alert-dash",
        AlertStyle.Soft => "alert-soft",
        _ => null
    };

    private string? LayoutClass => Layout switch
    {
        AlertLayout.Vertical => "alert-vertical",
        AlertLayout.Horizontal => "alert-horizontal",
        _ => null
    };
}
```

## Usage Examples

```razor
@* Basic *@
<Alert>This is a simple alert message.</Alert>

@* With color *@
<Alert Color="AlertColor.Success">Operation completed!</Alert>

@* With icon *@
<Alert Color="AlertColor.Warning">
    <Icon>
        <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
        </svg>
    </Icon>
    <span>Warning: Check your input!</span>
</Alert>

@* Soft style *@
<Alert Style="AlertStyle.Soft" Color="AlertColor.Info">
    Important information here.
</Alert>

@* Outline error *@
<Alert Style="AlertStyle.Outline" Color="AlertColor.Error">
    Something went wrong!
</Alert>

@* Responsive layout via Class parameter *@
<Alert Color="AlertColor.Info" Class="alert-vertical sm:alert-horizontal">
    Vertical on mobile, horizontal on desktop.
</Alert>

@* With custom classes *@
<Alert Color="AlertColor.Success" Class="shadow-lg">
    Success with shadow!
</Alert>
```
