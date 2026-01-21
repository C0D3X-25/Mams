using System.Windows.Media;

namespace Mams_App.src.views.globalView;

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
}

