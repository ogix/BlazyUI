# Tailwind v4 & DaisyUI v5 Setup Design

## Overview

Setup Tailwind CSS v4 and DaisyUI v5 for BlazyUI component library with a WASM demo site for GitHub Pages hosting.

## Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Demo hosting | Blazor WASM Standalone | GitHub Pages requires static files |
| CSS build | npm + Tailwind CLI | Standard approach, proper CSS purging |
| npm location | BlazyUI.Demo project | Consumers set up their own Tailwind for customization |
| Build integration | MSBuild for Release + watch script for dev | Best developer experience |
| Class customization | `Class` parameter + `AdditionalAttributes` | Most flexible, common Blazor pattern |
| Class merging | TailwindMerge.NET via `BlazyComponentBase` | Intelligent class conflict resolution |

## Project Structure

```
BlazyUI/
├── src/
│   ├── BlazyUI/                    # Razor Class Library
│   │   ├── Components/             # DaisyUI components
│   │   ├── BlazyComponentBase.cs   # Base component with TailwindMerge
│   │   ├── BlazyUI.csproj
│   │   └── _Imports.razor
│   │
│   └── BlazyUI.Demo/               # Blazor WASM Standalone
│       ├── wwwroot/
│       │   ├── css/
│       │   │   └── app.css         # Tailwind input file
│       │   └── index.html
│       ├── Components/
│       │   └── Pages/              # Demo pages
│       ├── package.json            # npm dependencies
│       └── BlazyUI.Demo.csproj     # With MSBuild targets
│
└── BlazyUI.slnx
```

## Configuration Files

### package.json

```json
{
  "devDependencies": {
    "tailwindcss": "^4",
    "daisyui": "^5"
  },
  "scripts": {
    "build:css": "npx @tailwindcss/cli -i ./wwwroot/css/app.css -o ./wwwroot/css/app.min.css --minify",
    "watch:css": "npx @tailwindcss/cli -i ./wwwroot/css/app.css -o ./wwwroot/css/app.min.css --watch"
  }
}
```

### wwwroot/css/app.css

```css
@import "tailwindcss";
@plugin "daisyui";

@source "../../BlazyUI/**/*.razor";
@source "../Components/**/*.razor";
```

## MSBuild Integration

```xml
<Target Name="CheckNpmInstall" BeforeTargets="BuildTailwind">
  <Exec Command="npm install" WorkingDirectory="$(MSBuildProjectDirectory)"
        Condition="!Exists('node_modules')" />
</Target>

<Target Name="BuildTailwind" BeforeTargets="Build"
        Condition="'$(Configuration)' == 'Release' OR !Exists('wwwroot/css/app.min.css')">
  <Exec Command="npm run build:css" WorkingDirectory="$(MSBuildProjectDirectory)" />
</Target>
```

## Base Component Pattern

```csharp
using Microsoft.AspNetCore.Components;
using TailwindMerge;

namespace BlazyUI;

public abstract class BlazyComponentBase : ComponentBase
{
    [Parameter] public string? Class { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    protected string MergeClasses(string? defaultClasses, string? additionalClasses = null)
    {
        return TwMerge.Merge(defaultClasses, additionalClasses, Class);
    }
}
```

## Development Workflow

```bash
# Terminal 1: CSS watcher
cd src/BlazyUI.Demo
npm run watch:css

# Terminal 2: Blazor app
dotnet watch --project src/BlazyUI.Demo
```

## Production Build

```bash
dotnet build -c Release
dotnet publish -c Release
```
