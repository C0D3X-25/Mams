using Mams.src.resumes;
using System.Windows.Controls;

namespace Mams.src.navigations;

public static class PageNavigationController {

    private static Frame? _m_frame;

    public static void Initialize(Frame frame) {
        _m_frame = frame;
        navigateToHomePage();
    }

    public static void navigateToHomePage() {
        _m_frame?.Navigate(new ResumePage());
    }

    public static void navigateTo(Page page) {
        _m_frame?.Navigate(page);
    }
}
