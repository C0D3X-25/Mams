//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace Project_Mams.src.imp;

//public class PanelManager {

//    private readonly Panel _m_container_panel;
//    private readonly Form _m_form;
//    private readonly Dictionary<string, UserControl> _m_panels = new();

//    public PanelManager(Panel container_panel, Form form) {
//        _m_container_panel = container_panel;
//        _m_form = form;
//    }

//    public void addPanel(string name, UserControl panel) {
//        panel.Dock = DockStyle.Fill;
//        panel.Visible = false;
//        _m_panels[name] = panel;
//        _m_container_panel.Controls.Add(panel);
//    }

//    public void showPanel(string name) {
//        foreach (var panel in _m_panels.Values) {
//            panel.Visible = false;
//        }

//        if (_m_panels.TryGetValue(name, out var target_panel)) {
//            target_panel.Visible = true;
//            target_panel.BringToFront();
//        }
//    }
//}
