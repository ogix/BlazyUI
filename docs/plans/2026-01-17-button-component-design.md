# Button Component Design

## Overview

A strongly-typed Blazor button component wrapping DaisyUI's button classes with full feature support.

## Decisions

| Decision | Choice |
|----------|--------|
| Element type | Always `<button>` |
| Feature scope | Full: colors, sizes, styles, modifiers, loading |
| API style | Strongly-typed enums |
| Loading behavior | Visual spinner + auto-disable |

## Enums

```csharp
public enum ButtonColor
{
    None,       // No color modifier (default)
    Neutral,
    Primary,
    Secondary,
    Accent,
    Info,
    Success,
    Warning,
    Error
}

public enum ButtonSize
{
    None,       // Default medium size
    ExtraSmall, // btn-xs
    Small,      // btn-sm
    Large,      // btn-lg
    ExtraLarge  // btn-xl
}

public enum ButtonStyle
{
    None,       // Solid fill (default)
    Outline,
    Soft,
    Ghost,
    Link
}
```

## Parameters

```csharp
// Appearance
[Parameter] public ButtonColor Color { get; set; } = ButtonColor.None;
[Parameter] public ButtonSize Size { get; set; } = ButtonSize.None;
[Parameter] public ButtonStyle Style { get; set; } = ButtonStyle.None;

// Modifiers
[Parameter] public bool Wide { get; set; }
[Parameter] public bool Block { get; set; }
[Parameter] public bool Square { get; set; }
[Parameter] public bool Circle { get; set; }

// State
[Parameter] public bool Loading { get; set; }
[Parameter] public bool Disabled { get; set; }

// Content & events
[Parameter] public RenderFragment? ChildContent { get; set; }
[Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

// Inherited from BlazyComponentBase
[Parameter] public string? Class { get; set; }
[Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }
```

## File Structure

```
src/BlazyUI/
├── BlazyComponentBase.cs      # Update MergeClasses to params
├── Button/
│   ├── Button.razor
│   ├── ButtonColor.cs
│   ├── ButtonSize.cs
│   └── ButtonStyle.cs
└── _Imports.razor
```

## Implementation Notes

- `IsDisabled => Disabled || Loading` - auto-disable when loading
- `MergeClasses` in base class needs update to accept `params string?[]`
- Consumer's `Class` parameter merges last, allowing overrides

## Usage Examples

```razor
@* Basic *@
<Button>Click me</Button>

@* Styled *@
<Button Color="ButtonColor.Primary" Size="ButtonSize.Large">
    Submit
</Button>

@* Outline with loading *@
<Button Style="ButtonStyle.Outline" Color="ButtonColor.Secondary" Loading="@isLoading">
    Save
</Button>

@* Icon button *@
<Button Circle Color="ButtonColor.Primary">
    <svg>...</svg>
</Button>

@* With custom classes *@
<Button Color="ButtonColor.Primary" Class="my-custom-class">
    Custom
</Button>
```
