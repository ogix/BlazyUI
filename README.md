# BlazyUI

**A Blazor component library wrapping DaisyUI v5 with Tailwind CSS v4**

[![NuGet](https://img.shields.io/nuget/v/BlazyUI.svg)](https://www.nuget.org/packages/BlazyUI)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)
![DaisyUI](https://img.shields.io/badge/DaisyUI-v5-5A0EF8)
![Tailwind CSS](https://img.shields.io/badge/Tailwind%20CSS-v4-38BDF8)
![License](https://img.shields.io/badge/License-MIT-green)

## Overview

BlazyUI provides strongly-typed Blazor components built on top of [DaisyUI](https://daisyui.com/) v5 and [Tailwind CSS](https://tailwindcss.com/) v4. It brings the beautiful, themeable components of DaisyUI to your Blazor applications with full C# type safety, IntelliSense support, and seamless EditForm integration.

## Features

- **34+ Strongly-Typed Components** - Full IntelliSense and compile-time safety
- **Light/Dark Theme Support** - Built-in DaisyUI theming with easy switching
- **EditForm Integration** - Input components work seamlessly with Blazor validation
- **TailwindMerge** - Intelligent class conflict resolution for custom styling
- **Modal & Toast Services** - Programmatic dialogs and notifications
- **Customizable** - Override styles with the `Class` parameter on any component

## Quick Start

### 1. Install the Package

```bash
dotnet add package BlazyUI
```

### 2. Register Services

In your `Program.cs`:

```csharp
using BlazyUI;

builder.Services.AddBlazyUI();
```

### 3. Add Imports

In your `_Imports.razor`:

```razor
@using BlazyUI
```

### 4. Setup CSS

BlazyUI requires Tailwind CSS v4 and DaisyUI v5. Install the npm packages and configure your CSS:

```bash
npm install -D tailwindcss@^4 daisyui@^5
```

In your main CSS file:

```css
@import "tailwindcss";
@plugin "daisyui";

/* Optional: Configure themes */
@plugin "daisyui" {
  themes: light --default, dark --prefersdark;
}

/* Scan your project and BlazyUI components */
@source "../Components/**/*.razor";
@source "../bin/blazyui/Components";
```

### 5. Add Providers

In your `MainLayout.razor` or `App.razor`:

```razor
<BlazyModalProvider />
<BlazyToastProvider />
```

## Available Components

### Layout & Structure
- **BlazyCard** - Container with header, body, and actions
- **BlazyAccordion** - Collapsible content sections
- **BlazyCollapse** - Single collapsible panel
- **BlazyDivider** - Visual separator
- **BlazyDrawer** - Off-canvas sidebar
- **BlazyJoin** - Group elements together
- **BlazyNavBar** - Top navigation bar
- **BlazyTabs** - Tabbed content panels

### Form Inputs
- **BlazyTextInput** - Text, email, password inputs
- **BlazyTextarea** - Multi-line text input
- **BlazySelect** - Dropdown selection
- **BlazyCheckbox** - Boolean input
- **BlazyRadio** - Single selection from options
- **BlazyToggle** - Switch input
- **BlazyRange** - Slider input
- **BlazyRating** - Star rating input
- **BlazyFileInput** - File upload input
- **BlazyFieldset** - Form field grouping
- **BlazyLabel** - Form field labels

### Visual & Feedback
- **BlazyAlert** - Contextual messages
- **BlazyBadge** - Status indicators
- **BlazyButton** - Interactive buttons
- **BlazyLoading** - Loading spinners
- **BlazyProgress** - Progress bars
- **BlazySkeleton** - Loading placeholders
- **BlazyStats** - Statistical displays
- **BlazyStatus** - Status dot indicators
- **BlazyTooltip** - Hover information
- **BlazyKbd** - Keyboard key styling
- **BlazyIndicator** - Notification badges
- **BlazyTimeline** - Chronological events

### Dialogs & Overlays
- **BlazyModalProvider** - Dialog windows (via `IBlazyModalService`)
- **BlazyToastProvider** - Toast notifications (via `IBlazyToastService`)

### Navigation
- **BlazyMenu** - Vertical menu with items and dropdowns

## Usage Examples

### Button Component

```razor
<BlazyButton Color="BlazyButtonColor.Primary">Primary</BlazyButton>
<BlazyButton Color="BlazyButtonColor.Secondary" Style="BlazyButtonStyle.Outline">Outline</BlazyButton>
<BlazyButton Color="BlazyButtonColor.Accent" Size="BlazyButtonSize.Large">Large</BlazyButton>
<BlazyButton Loading="true">Loading...</BlazyButton>
```

### Form with Validation

```razor
<EditForm Model="model" OnValidSubmit="HandleSubmit">
    <DataAnnotationsValidator />

    <BlazyFieldset Legend="User Information">
        <BlazyLabel Text="Email">
            <BlazyTextInput @bind-Value="model.Email" Type="email" placeholder="Enter email" />
            <ValidationMessage For="@(() => model.Email)" />
        </BlazyLabel>

        <BlazyLabel Text="Password">
            <BlazyTextInput @bind-Value="model.Password" Type="password" />
            <ValidationMessage For="@(() => model.Password)" />
        </BlazyLabel>
    </BlazyFieldset>

    <BlazyButton type="submit" Color="BlazyButtonColor.Primary">Submit</BlazyButton>
</EditForm>
```

### Modal Service

```razor
@inject IBlazyModalService ModalService

<BlazyButton OnClick="ShowConfirm">Delete Item</BlazyButton>

@code {
    private async Task ShowConfirm()
    {
        var confirmed = await ModalService.Confirm(
            "Delete Item",
            "Are you sure you want to delete this item?"
        );

        if (confirmed)
        {
            // User confirmed
        }
    }
}
```

### Toast Notifications

```razor
@inject IBlazyToastService ToastService

<BlazyButton OnClick="ShowToast">Show Toast</BlazyButton>

@code {
    private void ShowToast()
    {
        ToastService.Success("Item saved successfully!");
        ToastService.Error("Something went wrong");
        ToastService.Info("Here's some information");
    }
}
```

### Custom Styling

Override component styles using the `Class` parameter. TailwindMerge handles class conflicts automatically:

```razor
<BlazyButton Class="rounded-full shadow-lg" Color="BlazyButtonColor.Primary">
    Custom Styled
</BlazyButton>

<BlazyCard Class="bg-gradient-to-r from-purple-500 to-pink-500">
    <BlazyCardBody>
        <p class="text-white">Gradient card</p>
    </BlazyCardBody>
</BlazyCard>
```

## Development Setup

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (for Tailwind CSS)

### Build and Run

```bash
# Clone the repository
git clone https://github.com/your-username/BlazyUI.git
cd BlazyUI

# Build the solution
dotnet build

# Run the demo app
cd src/BlazyUI.Demo
npm install
dotnet run
```

### Development with CSS Watch

For the best development experience, run CSS watch and Blazor hot reload in separate terminals:

```bash
# Terminal 1: Watch CSS changes
cd src/BlazyUI.Demo
npm run watch:css

# Terminal 2: Run with hot reload
dotnet watch --project src/BlazyUI.Demo
```

## Project Structure

```
BlazyUI/
├── src/
│   ├── BlazyUI/                    # Component library (RCL)
│   │   ├── Components/             # UI components and base classes
│   │   └── Extensions/             # Service extensions
│   └── BlazyUI.Demo/               # Demo application
│       └── Pages/                  # Component showcases
├── docs/
│   └── plans/                      # Design documents
└── README.md
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [DaisyUI](https://daisyui.com/) - The beautiful component library this project wraps
- [Tailwind CSS](https://tailwindcss.com/) - The utility-first CSS framework
- [TailwindMerge.NET](https://github.com/AXBoosting/TailwindMerge.NET) - For intelligent class merging
