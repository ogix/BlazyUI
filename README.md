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
<ModalProvider />
<ToastProvider />
```

## Available Components

### Layout & Structure
- **Card** - Container with header, body, and actions
- **Accordion** - Collapsible content sections
- **Collapse** - Single collapsible panel
- **Divider** - Visual separator
- **Drawer** - Off-canvas sidebar
- **Join** - Group elements together
- **NavBar** - Top navigation bar
- **Tabs** - Tabbed content panels

### Form Inputs
- **TextInput** - Text, email, password inputs
- **Textarea** - Multi-line text input
- **Select** - Dropdown selection
- **Checkbox** - Boolean input
- **Radio** - Single selection from options
- **Toggle** - Switch input
- **Range** - Slider input
- **Rating** - Star rating input
- **FileInput** - File upload input
- **Fieldset** - Form field grouping
- **Label** - Form field labels

### Visual & Feedback
- **Alert** - Contextual messages
- **Badge** - Status indicators
- **Button** - Interactive buttons
- **Loading** - Loading spinners
- **Progress** - Progress bars
- **Skeleton** - Loading placeholders
- **Stats** - Statistical displays
- **Status** - Status dot indicators
- **Tooltip** - Hover information
- **Kbd** - Keyboard key styling
- **Indicator** - Notification badges
- **Timeline** - Chronological events

### Dialogs & Overlays
- **Modal** - Dialog windows (via `IModalService`)
- **Toast** - Toast notifications (via `IToastService`)

### Navigation
- **Menu** - Vertical menu with items and dropdowns

## Usage Examples

### Button Component

```razor
<Button Color="ButtonColor.Primary">Primary</Button>
<Button Color="ButtonColor.Secondary" Style="ButtonStyle.Outline">Outline</Button>
<Button Color="ButtonColor.Accent" Size="ButtonSize.Large">Large</Button>
<Button Loading="true">Loading...</Button>
```

### Form with Validation

```razor
<EditForm Model="model" OnValidSubmit="HandleSubmit">
    <DataAnnotationsValidator />

    <Fieldset Legend="User Information">
        <Label Text="Email">
            <TextInput @bind-Value="model.Email" Type="email" placeholder="Enter email" />
            <ValidationMessage For="@(() => model.Email)" />
        </Label>

        <Label Text="Password">
            <TextInput @bind-Value="model.Password" Type="password" />
            <ValidationMessage For="@(() => model.Password)" />
        </Label>
    </Fieldset>

    <Button type="submit" Color="ButtonColor.Primary">Submit</Button>
</EditForm>
```

### Modal Service

```razor
@inject IModalService ModalService

<Button OnClick="ShowConfirm">Delete Item</Button>

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
@inject IToastService ToastService

<Button OnClick="ShowToast">Show Toast</Button>

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
<Button Class="rounded-full shadow-lg" Color="ButtonColor.Primary">
    Custom Styled
</Button>

<Card Class="bg-gradient-to-r from-purple-500 to-pink-500">
    <CardBody>
        <p class="text-white">Gradient card</p>
    </CardBody>
</Card>
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
