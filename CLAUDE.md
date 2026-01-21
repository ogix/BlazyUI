# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

BlazyUI is a Blazor component library wrapping DaisyUI v5 with Tailwind CSS v4. It targets .NET 10.0 and provides strongly-typed Blazor components.

## Build Commands

```bash
# Build the entire solution
dotnet build

# Run the demo app (from src/BlazyUI.Demo)
cd src/BlazyUI.Demo
dotnet run

# CSS development (requires two terminals)
# Terminal 1: Watch CSS changes
cd src/BlazyUI.Demo && npm run watch:css

# Terminal 2: Run Blazor app with hot reload
dotnet watch --project src/BlazyUI.Demo
```

## Architecture

**Projects:**
- `src/BlazyUI/` - Razor Class Library containing reusable components
- `src/BlazyUI.Demo/` - Blazor WebAssembly demo site showcasing components

**Component Pattern:**
Each component lives in its own folder under `Components/`:
```
src/BlazyUI/Components/ComponentName/
├── ComponentName.razor      # Component markup and code
├── ComponentNameColor.cs    # Enum for color parameter
├── ComponentNameStyle.cs    # Enum for style parameter (if applicable)
└── ComponentNameSize.cs     # Enum for size parameter (if applicable)
```

**Base Classes:**
- `BlazyComponentBase` - For non-input components (Button, Alert, etc.)
  - `TwMerge` injection for intelligent Tailwind class conflict resolution
  - `Class` parameter for consumer CSS overrides
  - `AdditionalAttributes` for capturing unmatched HTML attributes
  - `MergeClasses()` helper method

- `BlazyInputBase<T>` - For form input components (TextInput, Select, etc.)
  - Inherits from Blazor's `InputBase<T>` for EditForm validation integration
  - `TwMerge` injection for class merging
  - `Class` parameter for consumer CSS overrides
  - Abstract `GetErrorColorClass()` for validation styling
  - `MergeClasses()` helper with automatic validation color support

**Enum Conventions:**
- Most enums include a `Default` value for default/inherited behavior
- Some enums (like LoadingType, LoadingSize) have explicit first values without `Default`
- Switch expressions should throw `ArgumentOutOfRangeException` for unhandled values

**CSS Setup:**
Tailwind CSS is configured in `src/BlazyUI.Demo/wwwroot/css/app.css` using:
- Tailwind CSS v4 with `@import "tailwindcss"`
- DaisyUI v5 as a plugin with light/dark theme support
- `@source` directives to scan component files for class usage

**Service Registration:**
Consumers must register BlazyUI services in `Program.cs`:
```csharp
builder.Services.AddBlazyUI();
```
This registers TailwindMerge, IModalService, and IToastService.

## Design Documents

New components should have a design document in `docs/plans/YYYY-MM-DD-<component>-design.md` before implementation.

## Git

Never commit changes unless explicitly asked by the user.

## Testing

Always stop the demo app process after testing is complete.
