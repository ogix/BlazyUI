# Phase 1: Foundation & Restructure Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Restructure component folders and create BlazyInputBase<T> for form input components.

**Architecture:** Move all existing components into a `Components/` subfolder for better organization, keeping the `BlazyUI` namespace unchanged. Create a new `BlazyInputBase<T>` class that inherits from Blazor's `InputBase<T>` and provides TwMerge integration with automatic validation color support.

**Tech Stack:** .NET 10.0, Blazor, TailwindMerge.NET

---

## Task 1: Create Components Directory Structure

**Files:**
- Create: `src/BlazyUI/Components/` (directory)

**Step 1: Create the Components directory**

```bash
mkdir -p src/BlazyUI/Components
```

**Step 2: Verify directory exists**

```bash
ls src/BlazyUI/Components
```

Expected: Empty directory exists

**Step 3: Commit**

```bash
git add src/BlazyUI/Components/.gitkeep 2>/dev/null || true
git commit --allow-empty -m "chore: create Components directory structure"
```

---

## Task 2: Move Alert Component

**Files:**
- Move: `src/BlazyUI/Alert/` → `src/BlazyUI/Components/Alert/`

**Step 1: Move the Alert folder**

```bash
git mv src/BlazyUI/Alert src/BlazyUI/Components/Alert
```

**Step 2: Build to verify**

```bash
dotnet build
```

Expected: Build succeeded with 0 errors

**Step 3: Commit**

```bash
git add -A
git commit -m "refactor: move Alert component to Components folder"
```

---

## Task 3: Move Button Component

**Files:**
- Move: `src/BlazyUI/Button/` → `src/BlazyUI/Components/Button/`

**Step 1: Move the Button folder**

```bash
git mv src/BlazyUI/Button src/BlazyUI/Components/Button
```

**Step 2: Build to verify**

```bash
dotnet build
```

Expected: Build succeeded with 0 errors

**Step 3: Commit**

```bash
git add -A
git commit -m "refactor: move Button component to Components folder"
```

---

## Task 4: Move Loading Component

**Files:**
- Move: `src/BlazyUI/Loading/` → `src/BlazyUI/Components/Loading/`

**Step 1: Move the Loading folder**

```bash
git mv src/BlazyUI/Loading src/BlazyUI/Components/Loading
```

**Step 2: Build to verify**

```bash
dotnet build
```

Expected: Build succeeded with 0 errors

**Step 3: Commit**

```bash
git add -A
git commit -m "refactor: move Loading component to Components folder"
```

---

## Task 5: Move NavBar Component

**Files:**
- Move: `src/BlazyUI/NavBar/` → `src/BlazyUI/Components/NavBar/`

**Step 1: Move the NavBar folder**

```bash
git mv src/BlazyUI/NavBar src/BlazyUI/Components/NavBar
```

**Step 2: Build to verify**

```bash
dotnet build
```

Expected: Build succeeded with 0 errors

**Step 3: Commit**

```bash
git add -A
git commit -m "refactor: move NavBar component to Components folder"
```

---

## Task 6: Update Tailwind CSS Source Directive

**Files:**
- Modify: `src/BlazyUI.Demo/wwwroot/css/app.css`

The Tailwind CSS `@source` directive needs to be updated to scan the new Components folder location.

**Step 1: Check current @source directive**

```bash
grep "@source" src/BlazyUI.Demo/wwwroot/css/app.css
```

**Step 2: Update the @source path if it references the old component location**

If it shows something like `@source "../../../BlazyUI/**/*.razor"`, update it to ensure it still finds components in the new location. The `**/*.razor` glob should still work since Components is a subdirectory.

**Step 3: Build CSS and verify**

```bash
cd src/BlazyUI.Demo && npm run build:css
```

Expected: CSS builds successfully

**Step 4: Commit if changes were made**

```bash
git add -A
git diff --cached --quiet || git commit -m "chore: update Tailwind source directive for Components folder"
```

---

## Task 7: Create BlazyInputBase<T> Base Class

**Files:**
- Create: `src/BlazyUI/BlazyInputBase.cs`

**Step 1: Create the BlazyInputBase.cs file**

```csharp
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using TailwindMerge;

namespace BlazyUI;

/// <summary>
/// Base class for form input components that integrate with Blazor's EditForm validation.
/// Provides TwMerge integration and automatic validation color support.
/// </summary>
/// <typeparam name="T">The type of value the input handles.</typeparam>
public abstract class BlazyInputBase<T> : InputBase<T>
{
    [Inject]
    protected TwMerge TwMerge { get; set; } = default!;

    /// <summary>
    /// Additional CSS classes to apply to the component.
    /// These classes are merged with the component's default classes using TailwindMerge.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Returns the DaisyUI error class for validation state (e.g., "input-error", "select-error").
    /// Override this in derived components to return the appropriate error class.
    /// </summary>
    protected abstract string GetErrorColorClass();

    /// <summary>
    /// Gets the validation color class based on the current validation state.
    /// Returns the error class when there are validation messages, null otherwise.
    /// </summary>
    protected string? ValidationColorClass
    {
        get
        {
            if (EditContext is null)
                return null;

            var hasMessages = EditContext.GetValidationMessages(FieldIdentifier).Any();
            return hasMessages ? GetErrorColorClass() : null;
        }
    }

    /// <summary>
    /// Merges the provided CSS classes using TailwindMerge.
    /// Includes validation color class and the Class parameter for consumer overrides.
    /// Later classes override earlier ones when there are conflicts.
    /// </summary>
    /// <param name="classes">CSS classes to merge. Null values are ignored.</param>
    /// <returns>The merged CSS class string.</returns>
    protected string MergeClasses(params string?[] classes)
    {
        var allClasses = classes
            .Append(ValidationColorClass)
            .Append(Class)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .ToArray();
        return TwMerge.Merge(allClasses) ?? string.Empty;
    }
}
```

**Step 2: Build to verify**

```bash
dotnet build
```

Expected: Build succeeded with 0 errors

**Step 3: Commit**

```bash
git add src/BlazyUI/BlazyInputBase.cs
git commit -m "feat: add BlazyInputBase<T> for form input components

Provides:
- InputBase<T> integration for EditForm validation
- TwMerge injection for class merging
- Automatic validation color support via GetErrorColorClass()
- MergeClasses() helper with validation state"
```

---

## Task 8: Update _Imports.razor

**Files:**
- Modify: `src/BlazyUI/_Imports.razor`

**Step 1: Add Forms namespace import**

Update `_Imports.razor` to include the Forms namespace needed for InputBase:

```razor
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.Forms
@using BlazyUI
```

**Step 2: Build to verify**

```bash
dotnet build
```

Expected: Build succeeded with 0 errors

**Step 3: Commit**

```bash
git add src/BlazyUI/_Imports.razor
git commit -m "chore: add Forms namespace to _Imports.razor"
```

---

## Task 9: Final Verification

**Step 1: Clean and rebuild entire solution**

```bash
dotnet clean && dotnet build
```

Expected: Build succeeded with 0 errors, 0 warnings

**Step 2: Verify folder structure**

```bash
find src/BlazyUI -type f \( -name "*.cs" -o -name "*.razor" \) | grep -v obj | sort
```

Expected output:
```
src/BlazyUI/_Imports.razor
src/BlazyUI/BlazyComponentBase.cs
src/BlazyUI/BlazyInputBase.cs
src/BlazyUI/Components/Alert/Alert.razor
src/BlazyUI/Components/Alert/AlertColor.cs
src/BlazyUI/Components/Alert/AlertLayout.cs
src/BlazyUI/Components/Alert/AlertStyle.cs
src/BlazyUI/Components/Button/Button.razor
src/BlazyUI/Components/Button/ButtonColor.cs
src/BlazyUI/Components/Button/ButtonSize.cs
src/BlazyUI/Components/Button/ButtonStyle.cs
src/BlazyUI/Components/Loading/Loading.razor
src/BlazyUI/Components/Loading/LoadingColor.cs
src/BlazyUI/Components/Loading/LoadingSize.cs
src/BlazyUI/Components/Loading/LoadingType.cs
src/BlazyUI/Components/NavBar/NavBar.razor
src/BlazyUI/Components/NavBar/NavBarCenter.razor
src/BlazyUI/Components/NavBar/NavBarColor.cs
src/BlazyUI/Components/NavBar/NavBarEnd.razor
src/BlazyUI/Components/NavBar/NavBarStart.razor
```

**Step 3: Run the demo app to verify components still work**

```bash
cd src/BlazyUI.Demo && dotnet run
```

Visit http://localhost:5000 (or the displayed port) and verify:
- Button examples render correctly
- Alert examples render correctly
- Loading examples render correctly
- NavBar examples render correctly

**Step 4: Stop the demo app**

Press Ctrl+C to stop the app.

---

## Summary

After completing all tasks, the project will have:

1. **Restructured folder layout:**
   - All components under `src/BlazyUI/Components/`
   - Base classes at `src/BlazyUI/` root

2. **New BlazyInputBase<T>:**
   - Inherits from `InputBase<T>` for EditForm integration
   - Provides `TwMerge` injection
   - Automatic validation color support
   - `MergeClasses()` helper method

3. **Ready for Phase 2:** Form input components can now inherit from `BlazyInputBase<T>`
