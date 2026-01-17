# Loading Component Design

## Overview

Display an animated loading indicator with configurable type, size, and color. Wraps DaisyUI's loading component with a strongly-typed Blazor API.

## Decisions

### Element Type
Use `<span>` element per DaisyUI convention.

### Accessibility
Include `role="status"` and `aria-label="Loading"` by default. Consumers can override via `AdditionalAttributes`.

### Enum Defaults
- **LoadingType**: No `None` value. Default to `Spinner` (most universally recognized).
- **LoadingSize**: No `None` value. Default to `Medium`.
- **LoadingColor**: Include `None` value (default). `None` means no color class applied, inherits parent text color.

### Error Handling
Switch expressions throw `ArgumentOutOfRangeException` for unhandled enum values to catch missing cases when enums are extended.

## Enums

### LoadingType
```csharp
public enum LoadingType
{
    Spinner,    // loading-spinner (default)
    Dots,       // loading-dots
    Ring,       // loading-ring
    Ball,       // loading-ball
    Bars,       // loading-bars
    Infinity    // loading-infinity
}
```

### LoadingSize
```csharp
public enum LoadingSize
{
    ExtraSmall, // loading-xs
    Small,      // loading-sm
    Medium,     // loading-md (default)
    Large,      // loading-lg
    ExtraLarge  // loading-xl
}
```

### LoadingColor
```csharp
public enum LoadingColor
{
    None,       // No class, inherits parent color (default)
    Primary,    // text-primary
    Secondary,  // text-secondary
    Accent,     // text-accent
    Neutral,    // text-neutral
    Info,       // text-info
    Success,    // text-success
    Warning,    // text-warning
    Error       // text-error
}
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Type | LoadingType | Spinner | Animation type |
| Size | LoadingSize | Medium | Size of the indicator |
| Color | LoadingColor | None | Color (None inherits parent) |
| Class | string? | null | Additional CSS classes (inherited) |
| AdditionalAttributes | Dictionary | null | HTML attributes (inherited) |

## File Structure

```
src/BlazyUI/
└── Loading/
    ├── Loading.razor
    ├── LoadingType.cs
    ├── LoadingSize.cs
    └── LoadingColor.cs
```

## Implementation

### Loading.razor
```razor
@namespace BlazyUI
@inherits BlazyComponentBase

<span role="status" aria-label="Loading" class="@CssClass" @attributes="AdditionalAttributes"></span>

@code {
    /// <summary>
    /// The type of loading animation to display.
    /// </summary>
    [Parameter] public LoadingType Type { get; set; } = LoadingType.Spinner;

    /// <summary>
    /// The size of the loading indicator.
    /// </summary>
    [Parameter] public LoadingSize Size { get; set; } = LoadingSize.Medium;

    /// <summary>
    /// The color of the loading indicator. None inherits from parent.
    /// </summary>
    [Parameter] public LoadingColor Color { get; set; } = LoadingColor.None;

    private string CssClass => MergeClasses("loading", TypeClass, SizeClass, ColorClass);

    private string TypeClass => Type switch
    {
        LoadingType.Spinner => "loading-spinner",
        LoadingType.Dots => "loading-dots",
        LoadingType.Ring => "loading-ring",
        LoadingType.Ball => "loading-ball",
        LoadingType.Bars => "loading-bars",
        LoadingType.Infinity => "loading-infinity",
        _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, "Unknown loading type")
    };

    private string SizeClass => Size switch
    {
        LoadingSize.ExtraSmall => "loading-xs",
        LoadingSize.Small => "loading-sm",
        LoadingSize.Medium => "loading-md",
        LoadingSize.Large => "loading-lg",
        LoadingSize.ExtraLarge => "loading-xl",
        _ => throw new ArgumentOutOfRangeException(nameof(Size), Size, "Unknown loading size")
    };

    private string? ColorClass => Color switch
    {
        LoadingColor.None => null,
        LoadingColor.Primary => "text-primary",
        LoadingColor.Secondary => "text-secondary",
        LoadingColor.Accent => "text-accent",
        LoadingColor.Neutral => "text-neutral",
        LoadingColor.Info => "text-info",
        LoadingColor.Success => "text-success",
        LoadingColor.Warning => "text-warning",
        LoadingColor.Error => "text-error",
        _ => throw new ArgumentOutOfRangeException(nameof(Color), Color, "Unknown loading color")
    };
}
```

## Usage Examples

```razor
<!-- Default: Spinner, Medium, inherits color -->
<Loading />

<!-- Explicit type -->
<Loading Type="LoadingType.Dots" />
<Loading Type="LoadingType.Ring" />
<Loading Type="LoadingType.Ball" />
<Loading Type="LoadingType.Bars" />
<Loading Type="LoadingType.Infinity" />

<!-- Different sizes -->
<Loading Size="LoadingSize.ExtraSmall" />
<Loading Size="LoadingSize.Small" />
<Loading Size="LoadingSize.Large" />
<Loading Size="LoadingSize.ExtraLarge" />

<!-- With color -->
<Loading Color="LoadingColor.Primary" />
<Loading Type="LoadingType.Bars" Color="LoadingColor.Success" />

<!-- Custom aria-label -->
<Loading aria-label="Saving your changes" />

<!-- Combined with custom class -->
<Loading Type="LoadingType.Infinity" Class="my-4" />
```

## Demo Page

Create `src/BlazyUI.Demo/Pages/Loadings.razor` showcasing:
- All 6 loading types
- All 5 sizes
- All color options
- Accessibility example with custom aria-label
