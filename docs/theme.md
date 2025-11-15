# Theme Guide for Off-Season Challenge Tracking App

## Design Philosophy

The app's design emphasizes energy, motivation, and modern aesthetics. The theme is built around distinctive typography, a cohesive color palette, and thoughtful use of motion and depth.

## Typography

### Font Families

The app uses three distinct font families to create visual hierarchy and interest:

1. **Bricolage Grotesque** (Display/Headings)
   - Weights: 200, 400, 700, 800
   - Usage: Headlines, titles, brand elements
   - Characteristics: Geometric sans-serif with distinctive character, excellent for large display text
   - CSS Variable: `--font-display`

2. **IBM Plex Sans** (Body Text)
   - Weights: 300, 400, 600, 700
   - Usage: Body text, UI elements, buttons, forms
   - Characteristics: Clean, technical, highly readable
   - CSS Variable: `--font-body`

3. **Fira Code** (Monospace)
   - Weights: 400, 600, 700
   - Usage: Navigation elements, code-like UI components
   - Characteristics: Programming font with ligatures, technical aesthetic
   - CSS Variable: `--font-mono`

### Typography Principles

- **High Contrast**: Use extreme weight differences (200 vs 800) rather than subtle variations
- **Size Jumps**: Use 3x+ size differences between hierarchy levels, not 1.5x
- **Letter Spacing**: Negative letter spacing (-0.03em to -0.04em) for large display text
- **Line Height**: Tighter for headlines (0.95), more generous for body text (1.6-1.7)

## Color Palette

### Base Colors

- **Primary Background**: `#0f172a` (Deep slate blue)
- **Secondary Background**: `#1e293b` (Slate)
- **Tertiary Background**: `#334155` (Lighter slate)

### Accent Colors

- **Primary Accent (Orange)**: `#f97316`
  - Light: `#fb923c`
  - Dark: `#ea580c`
  - Usage: Primary actions, highlights, energetic elements
  - Psychology: Energetic, motivating, warm

- **Secondary Accent (Cyan)**: `#06b6d4`
  - Light: `#22d3ee`
  - Dark: `#0891b2`
  - Usage: Secondary actions, links, complementary highlights
  - Psychology: Cool, modern, refreshing

### Text Colors

- **Primary Text**: `#f8fafc` (Warm white)
- **Secondary Text**: `#cbd5e1` (Light gray)
- **Tertiary Text**: `#94a3b8` (Muted gray)

### Border Colors

- **Primary Border**: `rgba(249, 115, 22, 0.2)` (Orange, 20% opacity)
- **Secondary Border**: `rgba(6, 182, 212, 0.2)` (Cyan, 20% opacity)
- **Subtle Border**: `rgba(248, 250, 252, 0.1)` (White, 10% opacity)

## Design Patterns

### Backgrounds

- **Layered Gradients**: Use radial gradients with orange and cyan for depth
- **Grid Patterns**: Subtle animated grid overlays for texture
- **Glass Morphism**: Semi-transparent cards with backdrop blur
- **Orb Effects**: Large blurred gradient orbs for atmospheric depth

### Components

- **Cards**: Semi-transparent backgrounds (`rgba(30, 41, 59, 0.4)`) with subtle borders
- **Buttons**: Gradient backgrounds with strong shadows on hover
- **Forms**: Dark inputs with orange focus states
- **Typography**: Gradient text for headlines using orange-to-cyan

### Motion

- **Staggered Animations**: Sequential reveals for list items
- **Hover Effects**: Transform and shadow changes (translateY, box-shadow)
- **Page Load**: Fade-in-up animations with delays
- **Grid Drift**: Subtle animated background patterns

## CSS Variables

All theme values are defined as CSS variables in `:root` for easy maintenance:

```css
--color-bg-primary: #0f172a;
--color-bg-secondary: #1e293b;
--color-accent-primary: #f97316;
--color-accent-secondary: #06b6d4;
--color-text-primary: #f8fafc;
--font-display: 'Bricolage Grotesque', sans-serif;
--font-body: 'IBM Plex Sans', sans-serif;
--font-mono: 'Fira Code', monospace;
```

## Avoiding Generic Patterns

The theme intentionally avoids:

- ❌ Generic fonts (Inter, Roboto, Open Sans, Lato)
- ❌ Purple gradients on white backgrounds
- ❌ Excessive centered layouts
- ❌ Uniform rounded corners everywhere
- ❌ Timid color palettes with evenly-distributed colors
- ❌ Predictable component patterns

Instead, it embraces:

- ✅ Distinctive, high-quality fonts
- ✅ Dominant colors with sharp accents
- ✅ Asymmetric, dynamic layouts
- ✅ Varied border radius (6px, 8px, 12px, 16px)
- ✅ Bold color choices (orange + cyan)
- ✅ Creative, context-specific design

## Implementation Notes

- All CSS is centralized in `clientapp/src/index.css`
- No inline styles except where absolutely necessary
- Use CSS variables for all color and font references
- Maintain consistent spacing using rem units
- Use clamp() for responsive typography
