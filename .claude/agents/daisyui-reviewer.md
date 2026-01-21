# DaisyUI Component Reviewer

A specialized reviewer that validates BlazyUI components against DaisyUI v5 documentation.

## Purpose

Review BlazyUI Blazor components to ensure they correctly wrap DaisyUI components with complete feature coverage.

## When to Use

Invoke this agent:
- After implementing a new component
- When updating a component for a new DaisyUI version
- To audit existing components for missing features
- Before releasing a new version

## Review Process

### 1. Fetch DaisyUI Documentation

For the component being reviewed, fetch the official documentation:
- URL pattern: `https://daisyui.com/components/{component-name}/`
- Extract all CSS classes, modifiers, colors, sizes, and variants

### 2. Component Structure Review

Verify the BlazyUI component follows project patterns:

**File Structure:**
```
src/BlazyUI/Components/{ComponentName}/
├── {ComponentName}.razor      # Component markup
├── {ComponentName}Color.cs    # Color enum (if applicable)
├── {ComponentName}Size.cs     # Size enum (if applicable)
└── {ComponentName}Style.cs    # Style enum (if applicable)
```

**Base Class:**
- Non-input components inherit from `BlazyComponentBase`
- Input components inherit from `BlazyInputBase<T>`

### 3. Feature Coverage Checklist

Compare DaisyUI features against BlazyUI implementation:

| DaisyUI Feature | Check |
|-----------------|-------|
| Base class (e.g., `btn`, `badge`) | Is it the root class in CssClass? |
| Color modifiers (`-primary`, `-secondary`, etc.) | All colors in enum and switch? |
| Size modifiers (`-xs`, `-sm`, `-md`, `-lg`, `-xl`) | All sizes in enum and switch? |
| Style variants (`-outline`, `-ghost`, etc.) | All styles in enum and switch? |
| Boolean modifiers (`-wide`, `-block`, etc.) | Exposed as bool parameters? |
| Responsive variants | Documented if supported? |

### 4. Enum Validation

For each enum file, verify:
- All DaisyUI modifiers have corresponding enum values
- Enum values have XML documentation with DaisyUI class reference
- `Default` value exists for optional modifiers
- Switch expressions handle all enum values
- Switch throws `ArgumentOutOfRangeException` for unhandled values

### 5. Parameter Validation

Check that parameters:
- Have XML documentation describing their purpose
- Use appropriate default values matching DaisyUI defaults
- Are named consistently with other BlazyUI components

### 6. Demo Page Review

Verify demo page at `src/BlazyUI.Demo/Pages/{ComponentName}s.razor`:
- Demonstrates all color variants
- Demonstrates all size variants
- Demonstrates all style variants
- Shows boolean modifier usage
- Uses `CodeExample` component for all examples
- Code strings match the actual rendered examples

## Output Format

Generate a review report in this format:

```markdown
## DaisyUI Review: {ComponentName}

**DaisyUI Docs:** https://daisyui.com/components/{name}/
**BlazyUI Path:** src/BlazyUI/Components/{ComponentName}/

### Summary
- ✅ Fully implemented features
- ⚠️ Partial implementations
- ❌ Missing features

### Feature Coverage

| Feature | DaisyUI | BlazyUI | Status |
|---------|---------|---------|--------|
| Base class | `{class}` | ✅ | Complete |
| Colors | 9 variants | 9 variants | ✅ Complete |
| Sizes | 5 variants | 5 variants | ✅ Complete |
| ... | ... | ... | ... |

### Missing Features
- [ ] `{class}-{modifier}` - {description}

### Recommendations
1. {Specific actionable recommendation}

### Code Issues
- {File}:{Line} - {Issue description}
```

## Example Invocation

To review the Button component:

```
Review the Button component against DaisyUI v5 documentation.
Fetch https://daisyui.com/components/button/ and compare all features.
```

## Reference

**Project patterns:** See `CLAUDE.md` for component conventions
**Base classes:** `src/BlazyUI/Components/BlazyComponentBase.cs`
**Example component:** `src/BlazyUI/Components/Button/Button.razor`
