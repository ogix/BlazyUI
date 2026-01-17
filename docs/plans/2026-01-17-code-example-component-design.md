# CodeExample Component Design

## Overview

A wrapper component for the Demo application that displays component examples with toggleable source code, syntax highlighting, and copy-to-clipboard functionality.

## Requirements

- Display rendered component example
- Toggle button to show/hide source code
- Syntax highlighting for Razor/HTML code
- Copy to clipboard button
- Lives in `BlazyUI.Demo` project (documentation-only component)

## Component API

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment` | required | The rendered example content |
| `Code` | `string` | required | The source code to display |
| `Language` | `string` | `"html"` | Language for syntax highlighting (html, razor, csharp) |
| `Title` | `string?` | `null` | Optional title above the example |
| `DefaultExpanded` | `bool` | `false` | Whether code is visible by default |

### Usage Example

```razor
<CodeExample Code="@primaryButtonCode" Title="Primary Button">
    <Button Color="ButtonColor.Primary">Primary</Button>
</CodeExample>

@code {
    private string primaryButtonCode = @"<Button Color=""ButtonColor.Primary"">Primary</Button>";
}
```

## File Structure

```
src/BlazyUI.Demo/
├── Components/
│   └── CodeExample.razor       # Main component
├── wwwroot/
│   ├── js/
│   │   └── code-highlight.js   # JS interop for Highlight.js
│   └── css/
│       └── code-example.css    # Component styles (optional)
└── Pages/
    └── Buttons.razor           # Updated to use CodeExample
```

## Implementation Details

### 1. Highlight.js Integration

**Option A: CDN (Recommended for simplicity)**
Add to `App.razor` or `index.html`:
```html
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/styles/github-dark.min.css">
<script src="https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/highlight.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/languages/xml.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/languages/csharp.min.js"></script>
```

**Option B: npm package**
```bash
npm install highlight.js
```
Then import in CSS/JS build.

### 2. JavaScript Interop

```javascript
// wwwroot/js/code-highlight.js
window.codeHighlight = {
    highlight: function(elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            hljs.highlightElement(element);
        }
    },
    copyToClipboard: async function(text) {
        try {
            await navigator.clipboard.writeText(text);
            return true;
        } catch (err) {
            console.error('Failed to copy:', err);
            return false;
        }
    }
};
```

### 3. CodeExample Component

```razor
@inject IJSRuntime JSRuntime

<div class="code-example border border-base-300 rounded-lg overflow-hidden mb-4">
    @if (!string.IsNullOrEmpty(Title))
    {
        <div class="px-4 py-2 bg-base-200 border-b border-base-300 font-medium">
            @Title
        </div>
    }

    <div class="p-4 bg-base-100">
        @ChildContent
    </div>

    <div class="flex justify-end gap-2 px-4 py-2 bg-base-200 border-t border-base-300">
        <button class="btn btn-sm btn-ghost" @onclick="ToggleCode">
            @(showCode ? "Hide Code" : "Show Code")
            <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 ml-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 20l4-16m4 4l4 4-4 4M6 16l-4-4 4-4" />
            </svg>
        </button>
        @if (showCode)
        {
            <button class="btn btn-sm btn-ghost" @onclick="CopyCode">
                @(copied ? "Copied!" : "Copy")
                <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 ml-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
                </svg>
            </button>
        }
    </div>

    @if (showCode)
    {
        <div class="border-t border-base-300">
            <pre class="m-0 p-4 overflow-x-auto"><code id="@codeId" class="language-@Language">@Code</code></pre>
        </div>
    }
</div>

@code {
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public string Code { get; set; } = "";
    [Parameter] public string Language { get; set; } = "html";
    [Parameter] public string? Title { get; set; }
    [Parameter] public bool DefaultExpanded { get; set; } = false;

    private bool showCode;
    private bool copied;
    private string codeId = $"code-{Guid.NewGuid():N}";

    protected override void OnInitialized()
    {
        showCode = DefaultExpanded;
    }

    private async Task ToggleCode()
    {
        showCode = !showCode;
        if (showCode)
        {
            await Task.Delay(1); // Wait for DOM update
            await JSRuntime.InvokeVoidAsync("codeHighlight.highlight", codeId);
        }
    }

    private async Task CopyCode()
    {
        var success = await JSRuntime.InvokeAsync<bool>("codeHighlight.copyToClipboard", Code);
        if (success)
        {
            copied = true;
            StateHasChanged();
            await Task.Delay(2000);
            copied = false;
            StateHasChanged();
        }
    }
}
```

### 4. Updated Buttons.razor Example

```razor
@page "/buttons"

<PageTitle>Buttons</PageTitle>

<h1 class="text-3xl font-bold mb-6">Button Component</h1>

<section class="mb-8">
    <h2 class="text-xl font-semibold mb-4">Basic</h2>
    <CodeExample Code="@basicCode">
        <div class="flex flex-wrap gap-2">
            <Button>Default</Button>
            <Button Color="ButtonColor.Neutral">Neutral</Button>
            <Button Color="ButtonColor.Primary">Primary</Button>
            <Button Color="ButtonColor.Secondary">Secondary</Button>
            <Button Color="ButtonColor.Accent">Accent</Button>
            <Button Color="ButtonColor.Info">Info</Button>
            <Button Color="ButtonColor.Success">Success</Button>
            <Button Color="ButtonColor.Warning">Warning</Button>
            <Button Color="ButtonColor.Error">Error</Button>
        </div>
    </CodeExample>
</section>

@code {
    private string basicCode = @"<Button>Default</Button>
<Button Color=""ButtonColor.Neutral"">Neutral</Button>
<Button Color=""ButtonColor.Primary"">Primary</Button>
<Button Color=""ButtonColor.Secondary"">Secondary</Button>
<Button Color=""ButtonColor.Accent"">Accent</Button>
<Button Color=""ButtonColor.Info"">Info</Button>
<Button Color=""ButtonColor.Success"">Success</Button>
<Button Color=""ButtonColor.Warning"">Warning</Button>
<Button Color=""ButtonColor.Error"">Error</Button>";
}
```

## Styling Considerations

### Theme Integration

Highlight.js themes should align with DaisyUI themes:
- Light themes: `github` or `atom-one-light`
- Dark themes: `github-dark` or `atom-one-dark`

Consider dynamically switching Highlight.js theme based on DaisyUI theme.

### Code Block Styling

```css
/* Optional: Custom styling */
.code-example pre {
    background-color: var(--fallback-b2, oklch(var(--b2)));
    margin: 0;
}

.code-example code {
    font-family: 'Fira Code', 'JetBrains Mono', monospace;
    font-size: 0.875rem;
    line-height: 1.5;
}
```

## Implementation Steps

1. **Add Highlight.js** - Add CDN links to App.razor (or install via npm)
2. **Create JS interop** - Add code-highlight.js to wwwroot/js
3. **Create CodeExample component** - Add CodeExample.razor to Components folder
4. **Register JS file** - Add script reference to App.razor
5. **Update Buttons.razor** - Wrap examples with CodeExample component
6. **Test** - Verify toggle, highlighting, and copy functionality

## Future Enhancements

- Theme-aware syntax highlighting (switch between light/dark)
- Support for multiple code tabs (Razor, C#, CSS)
- Editable code playground (Monaco editor integration)
- Live preview updates as code changes

## Dependencies

- Highlight.js 11.x (via CDN or npm)
- No additional NuGet packages required
