using Mams.views.pages;
using System.Windows.Controls;

namespace Mams.src.ctrl;

public class PageNavigationController {

    private readonly Frame? _m_frame;

    public PageNavigationController(Frame frame) {
        _m_frame = frame;
        navigateToHomePage();
    }

    public void navigateToHomePage() {
        _m_frame?.Navigate(new TemplatePage());
    }

    public void navigateToPage(Page page) {
        _m_frame?.Navigate(page);
    }
}
