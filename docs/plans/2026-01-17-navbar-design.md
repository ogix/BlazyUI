# NavBar Component Design

## Overview

A navigation bar component wrapping DaisyUI's navbar with three positioning zones (start, center, end) for flexible content layout.

## Components

```
src/BlazyUI/NavBar/
├── NavBar.razor           # Main container (<nav class="navbar">)
├── NavBarStart.razor      # Left section (<div class="navbar-start">)
├── NavBarCenter.razor     # Center section (<div class="navbar-center">)
├── NavBarEnd.razor        # Right section (<div class="navbar-end">)
└── NavBarColor.cs         # Enum for color variants
```

## Usage

```razor
<NavBar Color="NavBarColor.Primary" Sticky Shadow>
    <NavBarStart>
        <a class="btn btn-ghost text-xl">BlazyUI</a>
    </NavBarStart>
    <NavBarCenter>
        <a class="btn btn-ghost">Home</a>
        <a class="btn btn-ghost">About</a>
    </NavBarCenter>
    <NavBarEnd>
        <Button>Login</Button>
    </NavBarEnd>
</NavBar>
```

## NavBar Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Color` | `NavBarColor` | `None` | Background color variant |
| `Sticky` | `bool` | `false` | Adds `sticky top-0 z-50` for scroll-fixed positioning |
| `Shadow` | `bool` | `false` | Adds `shadow-sm` for subtle bottom shadow |
| `ChildContent` | `RenderFragment` | - | Content (NavBarStart, NavBarCenter, NavBarEnd) |
| `Class` | `string?` | `null` | Additional CSS classes (inherited from base) |
| `AdditionalAttributes` | `Dictionary` | - | Unmatched HTML attributes (inherited from base) |

## NavBarColor Enum

```csharp
public enum NavBarColor
{
    None,      // default (bg-base-100)
    Neutral,   // bg-neutral text-neutral-content
    Primary,   // bg-primary text-primary-content
    Secondary, // bg-secondary text-secondary-content
    Accent     // bg-accent text-accent-content
}
```

## Child Components

NavBarStart, NavBarCenter, and NavBarEnd share the same structure:

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment` | - | Content to display in this section |
| `Class` | `string?` | `null` | Additional CSS classes |
| `AdditionalAttributes` | `Dictionary` | - | Unmatched HTML attributes |

## CSS Class Logic

### NavBar

```csharp
private string CssClass => MergeClasses(
    "navbar",
    ColorClass,
    Sticky ? "sticky top-0 z-50" : null,
    Shadow ? "shadow-sm" : null
);

private string? ColorClass => Color switch
{
    NavBarColor.Neutral => "bg-neutral text-neutral-content",
    NavBarColor.Primary => "bg-primary text-primary-content",
    NavBarColor.Secondary => "bg-secondary text-secondary-content",
    NavBarColor.Accent => "bg-accent text-accent-content",
    _ => null
};
```

### Child Components

```csharp
// NavBarStart
private string CssClass => MergeClasses("navbar-start");

// NavBarCenter
private string CssClass => MergeClasses("navbar-center");

// NavBarEnd
private string CssClass => MergeClasses("navbar-end");
```

## HTML Output

```html
<nav class="navbar bg-primary text-primary-content sticky top-0 z-50 shadow-sm">
    <div class="navbar-start">...</div>
    <div class="navbar-center">...</div>
    <div class="navbar-end">...</div>
</nav>
```

## Demo Page

The demo page will showcase:

1. Basic navbar with logo and navigation links
2. All color variants (None, Neutral, Primary, Secondary, Accent)
3. Sticky navbar with scroll behavior
4. Shadow option comparison
5. Responsive pattern with dropdown menu for mobile
