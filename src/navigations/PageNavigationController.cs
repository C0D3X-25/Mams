using Mams.src.resumes;
using System.Windows.Controls;

namespace Mams.src.navigations;

public class PageNavigationController {

    private readonly Frame? _m_frame;

    public PageNavigationController(Frame frame) {
        _m_frame = frame;
        navigateToHomePage();
    }

    public void navigateToHomePage() {
        _m_frame?.Navigate(new ResumePage());
    }

    public void navigateTo(Page page) {
        _m_frame?.Navigate(page);
    }
}
