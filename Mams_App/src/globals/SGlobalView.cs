using System.Windows.Media;

namespace Mams_App.src.globals;

/// <summary>
/// Static class containing all global view configurations including sizes and colors.
/// Use these values for consistent sizing and theming across all Windows, Pages, and Views.
/// </summary>
public static class SGlobalView
{
    #region Size - Main Window

    /// <summary>Default width for the main window when opened.</summary>
    public const double MAIN_WINDOW_WIDTH = 1224;

    /// <summary>Default height for the main window when opened.</summary>
    public const double MAIN_WINDOW_HEIGHT = 700;

    /// <summary>Minimum width for the main window.</summary>
    public const double MAIN_WINDOW_MIN_WIDTH = 1224;

    /// <summary>Minimum height for the main window.</summary>
    public const double MAIN_WINDOW_MIN_HEIGHT = 600;

    #endregion

    #region Size - Settings Window

    /// <summary>Default width for the settings window when opened.</summary>
    public const double SETTINGS_WINDOW_WIDTH = 600;

    /// <summary>Default height for the settings window when opened.</summary>
    public const double SETTINGS_WINDOW_HEIGHT = 500;

    /// <summary>Minimum width for the settings window.</summary>
    public const double SETTINGS_WINDOW_MIN_WIDTH = 500;

    /// <summary>Minimum height for the settings window.</summary>
    public const double SETTINGS_WINDOW_MIN_HEIGHT = 400;

    #endregion

    #region Size - Launcher Window

    /// <summary>Default width for the launcher window.</summary>
    public const double LAUNCHER_WINDOW_WIDTH = 450;

    /// <summary>Default height for the launcher window.</summary>
    public const double LAUNCHER_WINDOW_HEIGHT = 500;

    #endregion

    #region Size - Pages (Designer dimensions)

    /// <summary>Default design width for pages (matches main window content area).</summary>
    public const double PAGE_DESIGN_WIDTH = 1224;

    /// <summary>Default design height for pages.</summary>
    public const double PAGE_DESIGN_HEIGHT = 600;

    #endregion

    #region Size - User Controls (Designer dimensions)

    /// <summary>Design width for header user control.</summary>
    public const double UC_HEADER_DESIGN_WIDTH = 1224;

    /// <summary>Design height for header user control.</summary>
    public const double UC_HEADER_DESIGN_HEIGHT = 50;

    /// <summary>Design width for footer user control.</summary>
    public const double UC_FOOTER_DESIGN_WIDTH = 1224;

    /// <summary>Design height for footer user control.</summary>
    public const double UC_FOOTER_DESIGN_HEIGHT = 25;

    /// <summary>Design width for menu user control.</summary>
    public const double UC_MENU_DESIGN_WIDTH = 200;

    /// <summary>Design height for menu user control.</summary>
    public const double UC_MENU_DESIGN_HEIGHT = 600;

    /// <summary>Design width for label textbox user control.</summary>
    public const double UC_LABEL_TEXTBOX_DESIGN_WIDTH = 450;

    /// <summary>Design height for label textbox user control.</summary>
    public const double UC_LABEL_TEXTBOX_DESIGN_HEIGHT = 50;

    #endregion

    #region Color - Brushes

    private static SolidColorBrush CreateBrush(byte r, byte g, byte b) => new(Color.FromRgb(r, g, b));

    // Page colors
    public static SolidColorBrush PAGE_FRAME_COLOR { get; set; } = CreateBrush(0x52, 0x14, 0x3d);         // #52143D - Dark Purple
    public static SolidColorBrush PAGE_BODY_COLOR { get; set; } = CreateBrush(0xec, 0xb3, 0xff);          // #ECB3FF - Light Purple
    public static SolidColorBrush PAGE_BUTTON_COLOR_1 { get; set; } = CreateBrush(0xdc, 0x6f, 0xb8);      // #DC6FB8 - Pink
    public static SolidColorBrush PAGE_BUTTON_COLOR_2 { get; set; } = CreateBrush(0xdd, 0x58, 0xb1);      // #DD58B1 - Hot Pink
    public static SolidColorBrush PAGE_BUTTON_TEXT_COLOR { get; set; } = CreateBrush(0x00, 0x00, 0x00);   // #000000 - Black

    // Header colors
    public static SolidColorBrush HEADER_TOP_COLOR { get; set; } = CreateBrush(0xa5, 0x00, 0xa5);                  // #A500A5 - Purple
    public static SolidColorBrush HEADER_TITLE_COLOR { get; set; } = CreateBrush(0x00, 0x00, 0x00);                // #000000 - Black
    public static SolidColorBrush HEADER_BUTTON_BACKGROUND_COLOR { get; set; } = CreateBrush(0xdc, 0x6f, 0xb8);    // #DC6FB8 - Pink
    public static SolidColorBrush HEADER_BUTTON_TEXT_COLOR { get; set; } = CreateBrush(0x00, 0x00, 0x00);          // #000000 - Black

    // Menu colors
    public static SolidColorBrush MENU_BACKGROUND_COLOR { get; set; } = CreateBrush(0xa5, 0x00, 0xa5);    // #A500A5 - Purple
    public static SolidColorBrush MENU_BUTTON_COLOR { get; set; } = CreateBrush(0xdc, 0x6f, 0xb8);        // #DC6FB8 - Pink
    public static SolidColorBrush MENU_BUTTON_TEXT_COLOR { get; set; } = CreateBrush(0x00, 0x00, 0x00);   // #000000 - Black

    // Footer colors
    public static SolidColorBrush FOOTER_BACKGROUND_COLOR { get; set; } = CreateBrush(0xa5, 0x00, 0xa5);  // #A500A5 - Purple

    // Text colors
    public static SolidColorBrush DEFAULT_TEXT_COLOR { get; set; } = CreateBrush(0x00, 0x00, 0x00);       // #000000 - Black
    public static SolidColorBrush NEGATIVE_VALUE_COLOR { get; set; } = CreateBrush(0xff, 0x00, 0x00);     // #FF0000 - Red

    // Action button colors
    public static SolidColorBrush DELETE_BUTTON_COLOR { get; set; } = CreateBrush(0xdc, 0x6f, 0xb8);      // #DC6FB8 - Pink
    public static SolidColorBrush DELETE_BUTTON_TEXT_COLOR { get; set; } = CreateBrush(0x00, 0x00, 0x00); // #000000 - Black
    public static SolidColorBrush DELETE_ROW_HIGHLIGHT_COLOR { get; set; } = CreateBrush(0xff, 0xcd, 0xd2); // #FFCDD2 - Light Red
    public static SolidColorBrush DELETE_ROW_BORDER_COLOR { get; set; } = CreateBrush(0xef, 0x53, 0x50);   // #EF5350 - Red
    public static SolidColorBrush RESTORE_BUTTON_COLOR { get; set; } = CreateBrush(0xdc, 0x6f, 0xb8);     // #DC6FB8 - Pink

    #endregion

    #region Button Styles Colors

    // Primary Button (Main action - Pink)
    public static SolidColorBrush BUTTON_PRIMARY_BACKGROUND { get; set; } = CreateBrush(0xdc, 0x6f, 0xb8);         // #DC6FB8 - Pink
    public static SolidColorBrush BUTTON_PRIMARY_BACKGROUND_HOVER { get; set; } = CreateBrush(0xe8, 0x8f, 0xc8);   // #E88FC8 - Light Pink
    public static SolidColorBrush BUTTON_PRIMARY_BACKGROUND_PRESSED { get; set; } = CreateBrush(0xc5, 0x5a, 0xa0); // #C55AA0 - Dark Pink
    public static SolidColorBrush BUTTON_PRIMARY_FOREGROUND { get; set; } = CreateBrush(0x00, 0x00, 0x00);         // #000000 - Black

    // Secondary Button (Cancel, neutral actions - Hot Pink)
    public static SolidColorBrush BUTTON_SECONDARY_BACKGROUND { get; set; } = CreateBrush(0xdd, 0x58, 0xb1);         // #DD58B1 - Hot Pink
    public static SolidColorBrush BUTTON_SECONDARY_BACKGROUND_HOVER { get; set; } = CreateBrush(0xe5, 0x7c, 0xc4);   // #E57CC4 - Light Hot Pink
    public static SolidColorBrush BUTTON_SECONDARY_BACKGROUND_PRESSED { get; set; } = CreateBrush(0xc4, 0x45, 0x9a); // #C4459A - Dark Hot Pink
    public static SolidColorBrush BUTTON_SECONDARY_FOREGROUND { get; set; } = CreateBrush(0x00, 0x00, 0x00);         // #000000 - Black

    // Danger Button (Delete, destructive actions - Red)
    public static SolidColorBrush BUTTON_DANGER_BACKGROUND { get; set; } = CreateBrush(0xef, 0x53, 0x50);         // #EF5350 - Red
    public static SolidColorBrush BUTTON_DANGER_BACKGROUND_HOVER { get; set; } = CreateBrush(0xf4, 0x7b, 0x79);   // #F47B79 - Light Red
    public static SolidColorBrush BUTTON_DANGER_BACKGROUND_PRESSED { get; set; } = CreateBrush(0xd3, 0x3a, 0x37); // #D33A37 - Dark Red
    public static SolidColorBrush BUTTON_DANGER_FOREGROUND { get; set; } = CreateBrush(0xff, 0xff, 0xff);         // #FFFFFF - White

    // Navigation Button Active State (Current page - Golden/Yellow)
    public static SolidColorBrush BUTTON_NAV_ACTIVE_BACKGROUND { get; set; } = CreateBrush(0xff, 0xc1, 0x07);         // #FFC107 - Amber/Gold
    public static SolidColorBrush BUTTON_NAV_ACTIVE_BACKGROUND_HOVER { get; set; } = CreateBrush(0xff, 0xd5, 0x4f);   // #FFD54F - Light Amber
    public static SolidColorBrush BUTTON_NAV_ACTIVE_BACKGROUND_PRESSED { get; set; } = CreateBrush(0xff, 0xa0, 0x00); // #FFA000 - Dark Amber
    public static SolidColorBrush BUTTON_NAV_ACTIVE_FOREGROUND { get; set; } = CreateBrush(0x00, 0x00, 0x00);         // #000000 - Black
    public static SolidColorBrush BEE_ICON_COLOR { get; set; } = CreateBrush(0x00, 0x00, 0x00);                       // #000000 - Black (inactive bee)
    public static SolidColorBrush BEE_ICON_ACTIVE_COLOR { get; set; } = CreateBrush(0xff, 0xeb, 0x3b);                // #FFEB3B - Yellow (active bee)

    #endregion
}
