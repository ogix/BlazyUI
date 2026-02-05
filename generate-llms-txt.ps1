<#
.SYNOPSIS
    Generates llms.txt for BlazyUI following the llmstxt.org specification.

.DESCRIPTION
    This script discovers components in the BlazyUI library and generates llms.txt
    that helps LLMs understand the project structure. The file is output to the
    demo app's wwwroot folder so it's served at /llms.txt.

.EXAMPLE
    .\generate-llms-txt.ps1

.EXAMPLE
    .\generate-llms-txt.ps1 -Verbose
#>

[CmdletBinding()]
param()

# Configuration
$RepoUrl = "https://raw.githubusercontent.com/ogix/BlazyUI/refs/heads/main"
$ComponentsPath = "src/BlazyUI/Components"
$OutputPath = "src/BlazyUI.Demo/wwwroot/llms.txt"

# Component descriptions - brief explanations for each component
$ComponentDescriptions = @{
    "Accordion"      = "Collapsible content sections with expandable panels"
    "Alert"          = "Contextual feedback messages with color variants"
    "Badge"          = "Small status indicators and labels"
    "Button"         = "Interactive buttons with colors, sizes, and states"
    "Card"           = "Container with header, body, and action sections"
    "Checkbox"       = "Boolean input for forms with validation support"
    "DateInput"      = "Date/time input supporting DateTime, DateOnly, TimeOnly types"
    "Collapse"       = "Single collapsible panel for show/hide content"
    "Divider"        = "Visual separator between content sections"
    "Drawer"         = "Off-canvas sidebar navigation panel"
    "Fieldset"       = "Form field grouping with legend support"
    "FieldValidator" = "Validation message display for form fields"
    "FileInput"      = "File upload input with drag-and-drop"
    "FormField"      = "Complete form field with label, input, and validation"
    "Indicator"      = "Notification badge overlay for elements"
    "Join"           = "Group elements together with connected styling"
    "Kbd"            = "Keyboard key styling for shortcuts"
    "Label"          = "Form field labels with required indicator"
    "Loading"        = "Loading spinners and skeleton animations"
    "Menu"           = "Vertical menu with items and nested dropdowns"
    "Modal"          = "Dialog windows via IModalService"
    "NavBar"         = "Top navigation bar with responsive layout"
    "Progress"       = "Progress bars with value indication"
    "Radio"          = "Single selection from radio button options"
    "Range"          = "Slider input for numeric range selection"
    "Rating"         = "Star rating input component"
    "Select"         = "Dropdown selection with validation support"
    "Skeleton"       = "Loading placeholder animations"
    "Stat"           = "Statistical displays with title and value"
    "Status"         = "Status dot indicators"
    "Tabs"           = "Tabbed content panels with multiple styles"
    "Textarea"       = "Multi-line text input with validation"
    "TextInput"      = "Text, email, password inputs with validation"
    "Timeline"       = "Chronological event display"
    "Toast"          = "Toast notifications via IToastService"
    "Toggle"         = "Switch/toggle input for boolean values"
    "Tooltip"        = "Hover information tooltips"
}

# Component categories for organization
$ComponentCategories = @{
    "Layout" = @("Card", "Accordion", "Collapse", "Divider", "Drawer", "Join", "NavBar", "Tabs", "Timeline")
    "Form Inputs" = @("TextInput", "Textarea", "Select", "Checkbox", "Radio", "Toggle", "Range", "Rating", "FileInput", "DateInput", "Fieldset", "FormField", "Label", "FieldValidator")
    "Visual/Feedback" = @("Alert", "Badge", "Button", "Loading", "Progress", "Skeleton", "Stat", "Status", "Tooltip", "Kbd", "Indicator")
    "Dialogs/Overlays" = @("Modal", "Toast")
    "Navigation" = @("Menu")
}

# Folders to exclude from component discovery
$ExcludeFolders = @("Icons", "obj", "bin")

function Get-Components {
    <#
    .SYNOPSIS
        Discovers components from the Components directory
    #>
    [CmdletBinding()]
    param()

    $componentsDir = Join-Path $PSScriptRoot $ComponentsPath

    if (-not (Test-Path $componentsDir)) {
        Write-Error "Components directory not found: $componentsDir"
        return @()
    }

    $components = Get-ChildItem -Path $componentsDir -Directory |
        Where-Object { $_.Name -notin $ExcludeFolders } |
        ForEach-Object { $_.Name } |
        Sort-Object

    Write-Verbose "Discovered $($components.Count) components"
    return $components
}

function Get-ComponentsByCategory {
    <#
    .SYNOPSIS
        Returns components organized by category
    #>
    [CmdletBinding()]
    param(
        [string[]]$DiscoveredComponents
    )

    $categorized = @{}
    $uncategorized = @()

    foreach ($category in $ComponentCategories.Keys) {
        $categorized[$category] = @()
    }

    foreach ($component in $DiscoveredComponents) {
        $found = $false
        foreach ($category in $ComponentCategories.Keys) {
            if ($component -in $ComponentCategories[$category]) {
                $categorized[$category] += $component
                $found = $true
                break
            }
        }
        if (-not $found) {
            $uncategorized += $component
            Write-Verbose "Uncategorized component: $component"
        }
    }

    if ($uncategorized.Count -gt 0) {
        $categorized["Other"] = $uncategorized
    }

    return $categorized
}

function Get-ComponentDescription {
    <#
    .SYNOPSIS
        Gets the description for a component
    #>
    param([string]$ComponentName)

    if ($ComponentDescriptions.ContainsKey($ComponentName)) {
        return $ComponentDescriptions[$ComponentName]
    }
    return "UI component"
}

function Get-ComponentUrl {
    <#
    .SYNOPSIS
        Generates the raw GitHub URL for a component's main file
    #>
    param([string]$ComponentName)

    return "$RepoUrl/$ComponentsPath/$ComponentName/$ComponentName.razor"
}

function Build-LlmsTxt {
    <#
    .SYNOPSIS
        Builds the llms.txt content
    #>
    [CmdletBinding()]
    param()

    $components = Get-Components
    $categorized = Get-ComponentsByCategory -DiscoveredComponents $components

    $content = @"
# BlazyUI

> A Blazor component library wrapping DaisyUI v5 with Tailwind CSS v4. Provides 34+ strongly-typed components with full IntelliSense, EditForm validation integration, TailwindMerge for class conflict resolution, and programmatic Modal/Toast services.

## Documentation

- [README]($RepoUrl/README.md): Installation guide, usage examples, and component overview
- [CLAUDE.md]($RepoUrl/CLAUDE.md): AI instructions, architecture details, and development patterns

## Core Architecture

- [BlazyComponentBase]($RepoUrl/$ComponentsPath/BlazyComponentBase.cs): Base class for non-input components (TwMerge, Class parameter, AdditionalAttributes)
- [BlazyInputBase]($RepoUrl/$ComponentsPath/BlazyInputBase.cs): Base class for form inputs (EditForm validation, error styling)
- [ExpressionFormatter]($RepoUrl/$ComponentsPath/ExpressionFormatter.cs): Expression-based field name generation for form inputs
- [FieldIdGenerator]($RepoUrl/$ComponentsPath/FieldIdGenerator.cs): Generates unique field IDs from expressions

## Services

- [IModalService]($RepoUrl/$ComponentsPath/Modal/IModalService.cs): Modal dialog API (Show, Confirm, Alert methods)
- [IToastService]($RepoUrl/$ComponentsPath/Toast/IToastService.cs): Toast notification API (Success, Error, Info, Warning)
- [ServiceCollectionExtensions]($RepoUrl/src/BlazyUI/Extensions/ServiceCollectionExtensions.cs): AddBlazyUI() service registration
"@

    # Add categorized components
    $categoryOrder = @("Layout", "Form Inputs", "Visual/Feedback", "Dialogs/Overlays", "Navigation", "Other")

    foreach ($category in $categoryOrder) {
        if ($categorized.ContainsKey($category) -and $categorized[$category].Count -gt 0) {
            $content += "`n`n## Components - $category`n`n"

            foreach ($component in ($categorized[$category] | Sort-Object)) {
                $url = Get-ComponentUrl -ComponentName $component
                $description = Get-ComponentDescription -ComponentName $component
                $content += "- [$component]($url): $description`n"
            }
        }
    }

    # Add Optional section with design documents
    $content += "`n## Optional`n`n"
    $content += "Design documents and implementation plans for deeper understanding:`n`n"

    $designDocsPath = Join-Path $PSScriptRoot "docs/plans"
    if (Test-Path $designDocsPath) {
        $designDocs = Get-ChildItem -Path $designDocsPath -Filter "*.md" | Sort-Object Name

        foreach ($doc in $designDocs) {
            $docName = $doc.BaseName
            $docUrl = "$RepoUrl/docs/plans/$($doc.Name)"

            # Extract a brief title from the filename
            $title = $docName -replace '^\d{4}-\d{2}-\d{2}-', '' -replace '-', ' '
            $title = (Get-Culture).TextInfo.ToTitleCase($title)

            $content += "- [$title]($docUrl)`n"
        }
    }

    return $content.TrimEnd()
}

# Main execution
Write-Verbose "Starting llms.txt generation..."
Write-Verbose "Repository URL: $RepoUrl"

# Generate llms.txt
$llmsTxt = Build-LlmsTxt
$llmsTxtPath = Join-Path $PSScriptRoot $OutputPath
$llmsTxt | Out-File -FilePath $llmsTxtPath -Encoding utf8 -NoNewline
Write-Host "Generated: $llmsTxtPath" -ForegroundColor Green

# Summary
$components = Get-Components
Write-Host "`nSummary:" -ForegroundColor Cyan
Write-Host "  Components discovered: $($components.Count)"
Write-Host "  Output: $OutputPath"
Write-Host "  Served at: /llms.txt"
