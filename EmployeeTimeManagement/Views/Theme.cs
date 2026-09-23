using System.Drawing;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    // The look a themed button takes, named for what the button means rather than for what
    // it looks like. Outline roles carry their meaning in the border and only fill on hover;
    // solid roles carry it in the fill, and are kept for the confirming button of a dialog.
    public enum ButtonRole
    {
        Primary,
        Dark,
        Danger,
        Ok,
        DangerSolid,
        OkSolid,
        Ghost
    }

    // The one place the redesign's colours, fonts and button looks are defined. Every
    // restyled screen reads them from here, so no other file names a colour of its own.
    public static class Theme
    {
        // The five colours of the design canvas.
        public static readonly Color Charcoal = Color.FromArgb(30, 26, 24);
        public static readonly Color Cream = Color.FromArgb(250, 244, 234);
        public static readonly Color Red = Color.FromArgb(184, 18, 28);
        public static readonly Color Mustard = Color.FromArgb(245, 184, 0);
        public static readonly Color Green = Color.FromArgb(29, 106, 59);

        // Body text on a cream or white surface.
        public static readonly Color Ink = Charcoal;

        // Second-rank text on cream: a job title, a date, a count. Blended far enough
        // towards charcoal to still read under harsh back-office lighting.
        public static readonly Color InkMuted = Color.FromArgb(114, 109, 104);

        // A field that stopped a save, and the line that explains why. Both keep the
        // colours the editor already used, so that only this file names them.
        public static readonly Color ErrorField = Color.FromArgb(255, 235, 238);
        public static readonly Color ErrorText = Color.Firebrick;

        // Text on the charcoal header, where cream itself is the first rank.
        public static readonly Color OnDark = Cream;
        public static readonly Color OnDarkMuted = Color.FromArgb(188, 183, 175);

        // Panels that sit on the cream page, and the hairline that separates them.
        public static readonly Color Surface = Color.White;
        public static readonly Color Border = Color.FromArgb(226, 216, 202);

        // Text on a solid red or green fill.
        public static readonly Color OnSolid = Color.White;

        // One flat, colourless look for every disabled button, so a User reads
        // "not available now" rather than "a different kind of button".
        public static readonly Color DisabledBack = Color.FromArgb(232, 226, 216);
        public static readonly Color DisabledFore = Color.FromArgb(150, 143, 134);
        public static readonly Color DisabledBorder = Color.FromArgb(214, 206, 194);

        private const string FontFamily = "Segoe UI";

        // Segoe UI throughout: it ships with Windows, so no font files have to be bundled.
        public static readonly Font TitleFont = new Font(FontFamily, 15F, FontStyle.Bold);
        public static readonly Font HeadingFont = new Font(FontFamily, 11F, FontStyle.Bold);
        public static readonly Font BodyFont = new Font(FontFamily, 9.75F, FontStyle.Regular);
        public static readonly Font ButtonFont = new Font(FontFamily, 9.75F, FontStyle.Bold);
        public static readonly Font SmallFont = new Font(FontFamily, 8.25F, FontStyle.Regular);

        // Gives a button one of the roles above: flat, a colour change on hover, a visible
        // focus ring and a distinct disabled look, with nothing that moves or fades. Call it
        // once per button, since each call subscribes its own handlers.
        public static void StyleButton(Button button, ButtonRole role)
        {
            ButtonPalette palette = PaletteFor(role);

            button.FlatStyle = FlatStyle.Flat;
            button.Font = ButtonFont;
            button.UseVisualStyleBackColor = false;
            button.FlatAppearance.BorderSize = palette.BorderSize;
            button.FlatAppearance.MouseOverBackColor = palette.HoverBack;
            button.FlatAppearance.MouseDownBackColor = palette.HoverBack;

            // Applied now as well as on every later change, so that a button which is
            // already disabled when it is styled still looks disabled.
            ApplyLook(button, palette);
            button.EnabledChanged += (sender, e) => ApplyLook(button, palette);

            button.Paint += (sender, e) =>
            {
                if (button.Focused && button.Enabled)
                {
                    DrawFocusRing(e.Graphics, button.ClientSize, palette.Focus);
                }
            };

            // The ring is painted over the button rather than being part of its border, so
            // the button has to be told to repaint when focus arrives and when it leaves.
            button.GotFocus += (sender, e) => button.Invalidate();
            button.LostFocus += (sender, e) => button.Invalidate();
        }

        // Puts an icon in front of a button's caption, so that the caption itself stays in the
        // designer file and the glyph, written as an escape, is the only thing added here.
        // Escaping it keeps every source file plain ASCII, beyond the reach of a code page.
        public static void PrefixIcon(Button button, string icon)
        {
            button.Text = icon + "  " + button.Text;
        }

        // Wires up a filter tab: flat, bold, and a focus ring, with no handler that has to be
        // renewed when the chosen tab changes. Call it once per tab; call SelectTab after it
        // and on every later change to say which tab is the chosen one.
        public static void StyleTab(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.Font = ButtonFont;
            button.UseVisualStyleBackColor = false;
            button.FlatAppearance.BorderSize = 1;

            SelectTab(button, false);

            button.Paint += (sender, e) =>
            {
                if (button.Focused && button.Enabled)
                {
                    // A chosen tab is charcoal, so its ring has to be the colour that reads
                    // against charcoal rather than the one used on the pale tabs beside it.
                    Color ring = button.BackColor == Charcoal ? Mustard : Charcoal;
                    DrawFocusRing(e.Graphics, button.ClientSize, ring);
                }
            };

            button.GotFocus += (sender, e) => button.Invalidate();
            button.LostFocus += (sender, e) => button.Invalidate();
        }

        // Colours a filter tab for whether it is the chosen one: the chosen tab is filled
        // charcoal, the rest stay pale and outlined so only one group reads as selected.
        public static void SelectTab(Button button, bool selected)
        {
            button.BackColor = selected ? Charcoal : Surface;
            button.ForeColor = selected ? OnDark : InkMuted;
            button.FlatAppearance.BorderColor = selected ? Charcoal : Border;
            button.FlatAppearance.MouseOverBackColor = selected ? Charcoal : Cream;
            button.FlatAppearance.MouseDownBackColor = selected ? Charcoal : Cream;
        }

        // Colours a button for the state it is in, since a disabled flat button otherwise
        // keeps whatever colours it was last given and goes on reading as available
        private static void ApplyLook(Button button, ButtonPalette palette)
        {
            button.BackColor = button.Enabled ? palette.Back : DisabledBack;
            button.ForeColor = button.Enabled ? palette.Fore : DisabledFore;
            button.FlatAppearance.BorderColor = button.Enabled ? palette.BorderColour : DisabledBorder;
        }

        // Draws the two-pixel ring that shows which control the keyboard is on
        private static void DrawFocusRing(Graphics graphics, Size size, Color colour)
        {
            using (var pen = new Pen(colour, 2F))
            {
                graphics.DrawRectangle(pen, 3, 3, size.Width - 7, size.Height - 7);
            }
        }

        // The colours one role resolves to, in every state a button can be in
        private sealed class ButtonPalette
        {
            public Color Back;
            public Color Fore;
            public Color BorderColour;
            public Color HoverBack;
            public Color Focus;
            public int BorderSize;
        }

        // Resolves a role to its colours. Primary is the default, so an unknown role still
        // produces a button a User can read rather than an invisible one.
        private static ButtonPalette PaletteFor(ButtonRole role)
        {
            switch (role)
            {
                case ButtonRole.Dark:
                    return new ButtonPalette
                    {
                        Back = Charcoal,
                        Fore = Cream,
                        BorderColour = Charcoal,
                        HoverBack = Color.FromArgb(58, 50, 46),
                        Focus = Mustard,
                        BorderSize = 1
                    };

                case ButtonRole.Danger:
                    return new ButtonPalette
                    {
                        Back = Cream,
                        Fore = Red,
                        BorderColour = Red,
                        HoverBack = Color.FromArgb(249, 226, 227),
                        Focus = Charcoal,
                        BorderSize = 1
                    };

                case ButtonRole.Ok:
                    return new ButtonPalette
                    {
                        Back = Cream,
                        Fore = Green,
                        BorderColour = Green,
                        HoverBack = Color.FromArgb(226, 240, 231),
                        Focus = Charcoal,
                        BorderSize = 1
                    };

                case ButtonRole.DangerSolid:
                    return new ButtonPalette
                    {
                        Back = Red,
                        Fore = OnSolid,
                        BorderColour = Red,
                        HoverBack = Color.FromArgb(152, 14, 23),
                        Focus = Mustard,
                        BorderSize = 1
                    };

                case ButtonRole.OkSolid:
                    return new ButtonPalette
                    {
                        Back = Green,
                        Fore = OnSolid,
                        BorderColour = Green,
                        HoverBack = Color.FromArgb(22, 84, 46),
                        Focus = Mustard,
                        BorderSize = 1
                    };

                case ButtonRole.Ghost:
                    return new ButtonPalette
                    {
                        Back = Cream,
                        Fore = Ink,
                        BorderColour = Cream,
                        HoverBack = Color.FromArgb(240, 232, 219),
                        Focus = Charcoal,
                        BorderSize = 0
                    };

                case ButtonRole.Primary:
                default:
                    return new ButtonPalette
                    {
                        Back = Mustard,
                        Fore = Charcoal,
                        BorderColour = Mustard,
                        HoverBack = Color.FromArgb(219, 164, 0),
                        Focus = Charcoal,
                        BorderSize = 1
                    };
            }
        }
    }
}
