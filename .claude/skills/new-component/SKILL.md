---
name: new-component
description: Scaffold a new BlazyUI component with folder structure, razor file, enums, and demo page
disable-model-invocation: true
---

# New Component Scaffolding

Create a new BlazyUI component following the established project patterns.

## Usage

```
/new-component <ComponentName>
```

Example: `/new-component Avatar`

## Workflow

### 1. Gather Information

Before scaffolding, ask the user:
- What DaisyUI component is this wrapping? (fetch docs from https://daisyui.com/components/{name}/)
- Does it need Color, Size, and/or Style enums?
- Is it a form input component (should inherit from `BlazyInputBase<T>`)?

### 2. Create Component Folder

Create folder at: `src/BlazyUI/Components/{ComponentName}/`

### 3. Create Enum Files (as needed)

#### Color Enum Pattern (`{ComponentName}Color.cs`)

```csharp
namespace BlazyUI;

/// <summary>
/// Color variants for the {ComponentName} component.
/// </summary>
public enum {ComponentName}Color
{
    /// <summary>No color modifier (default styling).</summary>
    Default,
    /// <summary>Neutral color.</summary>
    Neutral,
    /// <summary>Primary theme color.</summary>
    Primary,
    /// <summary>Secondary theme color.</summary>
    Secondary,
    /// <summary>Accent theme color.</summary>
    Accent,
    /// <summary>Info color (typically blue).</summary>
    Info,
    /// <summary>Success color (typically green).</summary>
    Success,
    /// <summary>Warning color (typically yellow/orange).</summary>
    Warning,
    /// <summary>Error color (typically red).</summary>
    Error
}
```

#### Size Enum Pattern (`{ComponentName}Size.cs`)

```csharp
namespace BlazyUI;

/// <summary>
/// Size variants for the {ComponentName} component.
/// </summary>
public enum {ComponentName}Size
{
    /// <summary>Extra small ({component}-xs).</summary>
    ExtraSmall,
    /// <summary>Small ({component}-sm).</summary>
    Small,
    /// <summary>Medium ({component}-md). Default.</summary>
    Medium,
    /// <summary>Large ({component}-lg).</summary>
    Large,
    /// <summary>Extra large ({component}-xl).</summary>
    ExtraLarge
}
```

#### Style Enum Pattern (`{ComponentName}Style.cs`)

```csharp
namespace BlazyUI;

/// <summary>
/// Style variants for the {ComponentName} component.
/// </summary>
public enum {ComponentName}Style
{
    /// <summary>No style modifier (default styling).</summary>
    Default,
    // Add component-specific styles based on DaisyUI docs
}
```

### 4. Create Component Razor File

#### For Non-Input Components (`{ComponentName}.razor`)

```razor
@namespace BlazyUI
@inherits BlazyComponentBase

<{htmlElement} class="@CssClass"
        @attributes="AdditionalAttributes">
    @ChildContent
</{htmlElement}>

@code {
    /// <summary>
    /// The color variant of the {component}.
    /// </summary>
    [Parameter]
    public {ComponentName}Color Color { get; set; } = {ComponentName}Color.Default;

    /// <summary>
    /// The size of the {component}.
    /// </summary>
    [Parameter]
    public {ComponentName}Size Size { get; set; } = {ComponentName}Size.Medium;

    /// <summary>
    /// The content to display inside the {component}.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string CssClass => MergeClasses(
        "{daisyui-class}",
        ColorClass,
        SizeClass
    );

    private string? ColorClass => Color switch
    {
        {ComponentName}Color.Default => null,
        {ComponentName}Color.Neutral => "{daisyui-class}-neutral",
        {ComponentName}Color.Primary => "{daisyui-class}-primary",
        {ComponentName}Color.Secondary => "{daisyui-class}-secondary",
        {ComponentName}Color.Accent => "{daisyui-class}-accent",
        {ComponentName}Color.Info => "{daisyui-class}-info",
        {ComponentName}Color.Success => "{daisyui-class}-success",
        {ComponentName}Color.Warning => "{daisyui-class}-warning",
        {ComponentName}Color.Error => "{daisyui-class}-error",
        _ => throw new ArgumentOutOfRangeException(nameof(Color), Color, null)
    };

    private string? SizeClass => Size switch
    {
        {ComponentName}Size.ExtraSmall => "{daisyui-class}-xs",
        {ComponentName}Size.Small => "{daisyui-class}-sm",
        {ComponentName}Size.Medium => null, // Default size, no class needed
        {ComponentName}Size.Large => "{daisyui-class}-lg",
        {ComponentName}Size.ExtraLarge => "{daisyui-class}-xl",
        _ => throw new ArgumentOutOfRangeException(nameof(Size), Size, null)
    };
}
```

#### For Input Components

Inherit from `BlazyInputBase<T>` instead. See `src/BlazyUI/Components/TextInput/TextInput.razor` for pattern.

### 5. Create Demo Page

Create at: `src/BlazyUI.Demo/Pages/{ComponentName}s.razor`

```razor
@page "/{componentnames}"

<PageTitle>{ComponentName}s</PageTitle>

<h1 class="text-3xl font-bold mb-6">{ComponentName} Component</h1>

<section class="mb-8">
    <h2 class="text-xl font-semibold mb-4">Basic</h2>
    <CodeExample Code="@basicCode">
        <div class="flex flex-wrap gap-2">
            <{ComponentName}>Default</{ComponentName}>
            <{ComponentName} Color="{ComponentName}Color.Primary">Primary</{ComponentName}>
            <!-- Add more examples -->
        </div>
    </CodeExample>
</section>

<section class="mb-8">
    <h2 class="text-xl font-semibold mb-4">Colors</h2>
    <CodeExample Code="@colorsCode">
        <div class="flex flex-wrap gap-2">
            <{ComponentName} Color="{ComponentName}Color.Neutral">Neutral</{ComponentName}>
            <{ComponentName} Color="{ComponentName}Color.Primary">Primary</{ComponentName}>
            <{ComponentName} Color="{ComponentName}Color.Secondary">Secondary</{ComponentName}>
            <{ComponentName} Color="{ComponentName}Color.Accent">Accent</{ComponentName}>
            <{ComponentName} Color="{ComponentName}Color.Info">Info</{ComponentName}>
            <{ComponentName} Color="{ComponentName}Color.Success">Success</{ComponentName}>
            <{ComponentName} Color="{ComponentName}Color.Warning">Warning</{ComponentName}>
            <{ComponentName} Color="{ComponentName}Color.Error">Error</{ComponentName}>
        </div>
    </CodeExample>
</section>

<section class="mb-8">
    <h2 class="text-xl font-semibold mb-4">Sizes</h2>
    <CodeExample Code="@sizesCode">
        <div class="flex flex-wrap items-center gap-2">
            <{ComponentName} Size="{ComponentName}Size.ExtraSmall">XS</{ComponentName}>
            <{ComponentName} Size="{ComponentName}Size.Small">Small</{ComponentName}>
            <{ComponentName}>Medium</{ComponentName}>
            <{ComponentName} Size="{ComponentName}Size.Large">Large</{ComponentName}>
            <{ComponentName} Size="{ComponentName}Size.ExtraLarge">XL</{ComponentName}>
        </div>
    </CodeExample>
</section>

@code {
    private const string basicCode = @"<{ComponentName}>Default</{ComponentName}>
<{ComponentName} Color=""{ComponentName}Color.Primary"">Primary</{ComponentName}>";

    private const string colorsCode = @"<{ComponentName} Color=""{ComponentName}Color.Neutral"">Neutral</{ComponentName}>
<{ComponentName} Color=""{ComponentName}Color.Primary"">Primary</{ComponentName}>
<{ComponentName} Color=""{ComponentName}Color.Secondary"">Secondary</{ComponentName}>
<{ComponentName} Color=""{ComponentName}Color.Accent"">Accent</{ComponentName}>
<{ComponentName} Color=""{ComponentName}Color.Info"">Info</{ComponentName}>
<{ComponentName} Color=""{ComponentName}Color.Success"">Success</{ComponentName}>
<{ComponentName} Color=""{ComponentName}Color.Warning"">Warning</{ComponentName}>
<{ComponentName} Color=""{ComponentName}Color.Error"">Error</{ComponentName}>";

    private const string sizesCode = @"<{ComponentName} Size=""{ComponentName}Size.ExtraSmall"">XS</{ComponentName}>
<{ComponentName} Size=""{ComponentName}Size.Small"">Small</{ComponentName}>
<{ComponentName}>Medium</{ComponentName}>
<{ComponentName} Size=""{ComponentName}Size.Large"">Large</{ComponentName}>
<{ComponentName} Size=""{ComponentName}Size.ExtraLarge"">XL</{ComponentName}>";
}
```

### 6. Add Navigation Link

Add the component to the sidebar navigation in `src/BlazyUI.Demo/Layout/MainLayout.razor`.

## Reference Files

When creating components, reference these existing files for patterns:
- Simple component: `src/BlazyUI/Components/Badge/Badge.razor`
- Complex component: `src/BlazyUI/Components/Button/Button.razor`
- Input component: `src/BlazyUI/Components/TextInput/TextInput.razor`
- Base class: `src/BlazyUI/Components/BlazyComponentBase.cs`
- Input base: `src/BlazyUI/Components/BlazyInputBase.cs`
- Demo page: `src/BlazyUI.Demo/Pages/Buttons.razor`

## Checklist

After scaffolding, verify:
- [ ] Component folder created with all necessary files
- [ ] Razor file inherits correct base class
- [ ] All enums have XML documentation
- [ ] Switch expressions throw `ArgumentOutOfRangeException` for unhandled values
- [ ] Demo page has `@page` directive with correct route
- [ ] Demo page uses `CodeExample` component for all examples
- [ ] Navigation link added to MainLayout
- [ ] `dotnet build` succeeds
