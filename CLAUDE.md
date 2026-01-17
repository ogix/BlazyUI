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
Each component lives in its own folder with the component and its enums:
```
src/BlazyUI/ComponentName/
├── ComponentName.razor    # Component markup and code
├── ComponentNameVariant.cs  # Enum for variant parameter
└── ComponentNameSize.cs     # Enum for size parameter (if applicable)
```

**Base Class:**
All components inherit from `BlazyComponentBase` which provides:
- `TwMerge` injection for intelligent Tailwind class conflict resolution
- `Class` parameter for consumer CSS overrides
- `AdditionalAttributes` for capturing unmatched HTML attributes
- `MergeClasses()` helper method

**Enum Conventions:**
- Most enums include a `None` value for default/inherited behavior
- Some enums (like LoadingType, LoadingSize) have explicit defaults without `None`
- Switch expressions should throw `ArgumentOutOfRangeException` for unhandled values

**CSS Setup:**
Tailwind CSS is configured in `src/BlazyUI.Demo/wwwroot/css/app.css` using:
- Tailwind CSS v4 with `@import "tailwindcss"`
- DaisyUI v5 as a plugin with light/dark theme support
- `@source` directives to scan component files for class usage

## Design Documents

New components should have a design document in `docs/plans/YYYY-MM-DD-<component>-design.md` before implementation.

## Git

Never commit changes unless explicitly asked by the user.

## Testing

Always stop the demo app process after testing is complete.
