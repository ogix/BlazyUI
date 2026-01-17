# TextInput Component Design

## Overview

A strongly-typed Blazor text input component wrapping DaisyUI's input classes with EditForm validation integration.

## Decisions

| Decision | Choice |
|----------|--------|
| Base class | `BlazyInputBase<string>` |
| Type parameter | String (flexible, matches HTML) |
| Style modifier | `bool Ghost` (only one style in DaisyUI) |
| Validation | Automatic `input-error` class via base class |

## Enums

```csharp
public enum TextInputColor
{
    Default,    // No color modifier
    Neutral,
    Primary,
    Secondary,
    Accent,
    Info,
    Success,
    Warning,
    Error
}

public enum TextInputSize
{
    Default,    // Medium (no class needed)
    ExtraSmall, // input-xs
    Small,      // input-sm
    Medium,     // input-md (explicit)
    Large,      // input-lg
    ExtraLarge  // input-xl
}
```

## Parameters

```csharp
// Input type
[Parameter] public string Type { get; set; } = "text";

// Appearance
[Parameter] public TextInputSize Size { get; set; } = TextInputSize.Default;
[Parameter] public TextInputColor Color { get; set; } = TextInputColor.Default;
[Parameter] public bool Ghost { get; set; }

// Inherited from BlazyInputBase<T>
[Parameter] public string? Class { get; set; }

// Inherited from InputBase<T>
[Parameter] public Expression<Func<string>>? ValueExpression { get; set; }
[Parameter] public string? Value { get; set; }
[Parameter] public EventCallback<string> ValueChanged { get; set; }
[Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }
```

## File Structure

```
src/BlazyUI/Components/TextInput/
├── TextInput.razor
├── TextInputColor.cs
└── TextInputSize.cs
```

## Implementation

```razor
@namespace BlazyUI
@inherits BlazyInputBase<string>

<input type="@Type"
       class="@CssClass"
       value="@CurrentValue"
       @onchange="OnChange"
       @attributes="AdditionalAttributes" />

@code {
    [Parameter] public string Type { get; set; } = "text";
    [Parameter] public TextInputSize Size { get; set; } = TextInputSize.Default;
    [Parameter] public TextInputColor Color { get; set; } = TextInputColor.Default;
    [Parameter] public bool Ghost { get; set; }

    protected override string GetErrorColorClass() => "input-error";

    private string CssClass => MergeClasses(
        "input",
        SizeClass,
        ColorClass,
        Ghost ? "input-ghost" : null
    );

    private void OnChange(ChangeEventArgs e)
        => CurrentValue = e.Value?.ToString();

    private string? SizeClass => Size switch
    {
        TextInputSize.Default => null,
        TextInputSize.ExtraSmall => "input-xs",
        TextInputSize.Small => "input-sm",
        TextInputSize.Medium => "input-md",
        TextInputSize.Large => "input-lg",
        TextInputSize.ExtraLarge => "input-xl",
        _ => throw new ArgumentOutOfRangeException(nameof(Size))
    };

    private string? ColorClass => Color switch
    {
        TextInputColor.Default => null,
        TextInputColor.Neutral => "input-neutral",
        TextInputColor.Primary => "input-primary",
        TextInputColor.Secondary => "input-secondary",
        TextInputColor.Accent => "input-accent",
        TextInputColor.Info => "input-info",
        TextInputColor.Success => "input-success",
        TextInputColor.Warning => "input-warning",
        TextInputColor.Error => "input-error",
        _ => throw new ArgumentOutOfRangeException(nameof(Color))
    };
}
```

## Usage Examples

```razor
@* Basic *@
<TextInput @bind-Value="name" />
<TextInput Type="password" @bind-Value="password" />
<TextInput Type="email" @bind-Value="email" placeholder="you@example.com" />

@* Styled *@
<TextInput @bind-Value="search" Color="TextInputColor.Primary" Size="TextInputSize.Large" />
<TextInput @bind-Value="code" Ghost Size="TextInputSize.Small" />

@* With EditForm validation *@
<EditForm Model="@model" OnValidSubmit="@Submit">
    <DataAnnotationsValidator />

    <TextInput @bind-Value="model.Email" Type="email" />
    <ValidationMessage For="@(() => model.Email)" />

    <TextInput @bind-Value="model.Password" Type="password" />
    <ValidationMessage For="@(() => model.Password)" />

    <Button Type="submit" Color="ButtonColor.Primary">Sign In</Button>
</EditForm>
```

## Validation Behavior

When used inside an `EditForm`, the component automatically:
1. Integrates with `EditContext` via `InputBase<T>`
2. Checks for validation messages on the bound field
3. Applies `input-error` class when validation fails (red border)
4. Removes error styling when validation passes

No manual wiring needed - just use `@bind-Value` and add a `DataAnnotationsValidator`.
