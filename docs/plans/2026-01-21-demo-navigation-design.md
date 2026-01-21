# Demo App Navigation Redesign

## Overview

Update the BlazyUI demo app navigation to provide a cleaner, more professional structure with proper documentation pages.

## Navigation Structure

```
Home                    (top-level link)
Getting Started         (top-level link - new page)
Components              (collapsible MenuDropdown, open by default)
  ├─ Actions
  │    Button, Modal
  ├─ Data Display
  │    Accordion, Badge, Indicator, Status, Card, Collapse, Kbd, Stat, QuickGrid, Timeline
  ├─ Data Input
  │    Checkbox, FileInput, Radio, Range, Rating, Select, TextInput, Textarea, Toggle, Fieldset, Label
  ├─ Navigation
  │    NavBar, Tabs, Menu
  ├─ Layout
  │    Divider, Drawer, Join
  └─ Feedback
       Alert, Loading, Progress, Skeleton, Toast, Tooltip
```

## Getting Started Page Content

1. **Introduction** - Brief description of BlazyUI (Blazor + DaisyUI v5 + Tailwind CSS v4)

2. **Installation**
   - Add NuGet package: `dotnet add package BlazyUI`
   - Add CSS reference to `index.html` or `App.razor`
   - Add `@using BlazyUI` to `_Imports.razor`

3. **First Component Example**
   - Simple Button example with code snippet
   - Uses existing `CodeExample` component

## Files to Change

**New file:**
- `src/BlazyUI.Demo/Pages/GettingStarted.razor`

**Files to modify:**
- `src/BlazyUI.Demo/Layout/NavMenu.razor` - Restructure with two-level navigation

**Files to delete:**
- `src/BlazyUI.Demo/Pages/Counter.razor`
- `src/BlazyUI.Demo/Pages/Weather.razor`
